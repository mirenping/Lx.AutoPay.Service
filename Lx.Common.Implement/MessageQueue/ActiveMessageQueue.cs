using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Lx.Common.Implement.Extension;
using Lx.Common.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Implement.MessageQueue
{
    /// <summary>
    ///     功能：消息通道
    /// </summary>
    public class ActiveMessageQueueChannel
    {
        #region   属性定义  

        /// <summary>
        ///     连接URL
        /// </summary>
        /// <example>
        ///     tcp://activemqhost:61616
        /// </example>
        public string Uri { get; set; }

        /// <summary>
        ///     brocker 用户名, 不填用户名，则不使用用户名密码进行验证
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     brocker 密码
        /// </summary>
        public string Password { get; set; }

        #endregion
    }

    /// <summary>
    /// 功能：ActiveMQ 消息队列包装
    /// </summary>
    public class ActiveMessageQueue : IMessageQueue, IComponent, IDisposable
    {
        #region  常量定义 

        /// <summary>
        ///     消息完成类名变量
        /// </summary>
        public const string ExternPropertyNameMessageTypeName = "_messageType";

        #endregion

        #region  变量定义

        /// <summary>
        ///     mq 连接实例
        /// </summary>
        private readonly Dictionary<string, IConnection> m_connections = new Dictionary<string, IConnection>();

        /// <summary>
        ///     通道分发器
        /// </summary>
        private IActiveMqChannelDeliver m_channelDeliver;

        /// <summary>
        ///     接收者分发器
        /// </summary>
        private IActiveMQReceiverDeliver m_receiverDeliver;

        #endregion

        #region 属性定义 

        /// <summary>
        /// 所有的通道
        /// </summary>
        public Dictionary<string, ActiveMessageQueueChannel> Channels { get; set; }

        /// <summary>
        ///  通道分发器
        /// </summary>
        public JArray ChannelDelivers { get; set; }

        /// <summary>
        ///     接收者分发器
        /// </summary>
        public JArray ReceiverDelivers { get; set; }

        /// <summary>
        ///     默认队列名
        /// </summary>
        public string DefaultQueueName { get; set; }

        /// <summary>
        ///     是否启用异步
        /// </summary>
        public bool IsAsync { get; set; }

        #endregion

        #region 构造函数

        /// <summary>
        ///     功能：构建一个 <see cref="ActiveMessageQueue" /> 对象
        /// </summary>
        public ActiveMessageQueue()
        {
            Channels = new Dictionary<string, ActiveMessageQueueChannel>();
        }

        #endregion

        #region 方法定义 

        /// <summary>
        ///     功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            foreach (var connection in m_connections.Values)
            {
                using (connection)
                {
                }
            }
        }

        /// <summary>
        ///   功能：获取一个消息发布者
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public IMessagePublisher GetMessagePublisher(string queue)
        {
            return new ActiveMessageQueueMessagePublishers(this, "queue://" + queue);
        }

        /// <summary>
        ///  功能：获取一个广播消息发布者
        /// </summary>
        /// <param name="queue">队列名称</param>
        /// <returns></returns>
        public IBroadcastPublisher GetBroadcastPublisher(string queue)
        {
            return new ActiveMessageQueueMessagePublishers(this, "topic://" + queue);
        }

        /// <summary>
        ///  功能：获取一个消息接收者对象
        /// </summary>
        /// <param name="queue">列表名称</param>
        /// <param name="receiverId">接收器名称</param>
        /// <returns></returns>
        public IMessageReceiver GetMessageReceiver(string queue, string receiverId)
        {
            return new ActiveMessageQueueMessageReceivers(this, "queue://" + queue, receiverId);
        }

        /// <summary>
        ///     功能：获取一个广播消息接收器
        /// </summary>
        /// <param name="broadcastName">通道名称</param>
        /// <param name="receiverId">接收器名称</param>
        /// <returns></returns>
        public IBroadcastReceiver GetBroadcastReceiver(string broadcastName, string receiverId)
        {
            return new ActiveMessageQueueMessageReceivers(this, "topic://" + broadcastName, receiverId);
        }

        /// <summary>
        /// 功能：获取内部连接
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IConnection> GetConnections()
        {
            return m_connections.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// 功能：获取队列分发器
        /// </summary>
        /// <returns></returns>
        public IActiveMqChannelDeliver GetChannelDeliver()
        {
            return m_channelDeliver ?? new DefaultChannelDeliver();
        }

        /// <summary>
        /// 功能：设置队列分发器
        /// </summary>
        /// <param name="channelDeliver"></param>
        public void SetChannelDeliver(IActiveMqChannelDeliver channelDeliver)
        {
            m_channelDeliver = channelDeliver;
        }

        /// <summary>
        /// 功能：获取接收者分发器
        /// </summary>
        /// <returns></returns>
        public IActiveMQReceiverDeliver GetReceiverDeliver()
        {
            return m_receiverDeliver ?? new DefaultReceiverDeliver();
        }

        /// <summary>
        ///  功能：设置接收者分发器
        /// </summary>
        /// <param name="receiverDeliver"></param>
        public void SetReceiverDeliver(IActiveMQReceiverDeliver receiverDeliver)
        {
            m_receiverDeliver = receiverDeliver;
        }

        /// <summary>
        /// 功能：获取默认队列名
        /// </summary>
        /// <returns></returns>
        public string GetDefaultChannelName()
        {
            if (string.IsNullOrWhiteSpace(DefaultQueueName))
                return Channels.FirstOrDefault().Key;
            return DefaultQueueName;
        }

        #endregion

        #region 实现接口 IComponent

        /// <summary>
        ///     功能：初始化组件
        /// </summary>
        void IComponent.Init()
        {
            foreach (var kv in Channels)
            {
                var channel = kv.Value;
                var connecturi = new Uri(channel.Uri);
                var connection = default(IConnection);
                var connectionFactory = new ConnectionFactory(connecturi);
                if (string.IsNullOrEmpty(channel.UserName))
                {
                    connection = connectionFactory.CreateConnection();
                }
                else
                {
                    connection = connectionFactory.CreateConnection(channel.UserName, channel.Password ?? "");
                }
                connection.Start();
                m_connections[kv.Key] = connection;
                var conn = connection as Connection;
                if (conn != null && IsAsync)
                {
                    conn.AsyncSend = true;
                    conn.AsyncClose = true;
                    conn.SendAcksAsync = true;
                }
                if (ChannelDelivers != null)
                {
                    m_channelDeliver = new CompositeChannelDeliver();
                    foreach (JObject deliver in ChannelDelivers)
                    {
                        ((CompositeChannelDeliver)m_channelDeliver).Add(CreateChannelDeliver(deliver));
                    }
                }
                if (ReceiverDelivers != null)
                {
                    m_receiverDeliver = new CompositeReceiverDeliver();
                    foreach (JObject deliver in ReceiverDelivers)
                    {
                        ((CompositeReceiverDeliver)m_receiverDeliver).Add(CreateReceiverDeliver(deliver));
                    }
                }
            }
        }

        /// <summary>
        /// 功能：创建一个消息分发器
        /// </summary>
        /// <param name="channelDeliver">分发器配置</param>
        /// <returns></returns>
        private IActiveMqChannelDeliver CreateChannelDeliver(JObject channelDeliver)
        {
            var typeName = channelDeliver["_type"].ToString();
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new Exception("无法解析类别: " + typeName);
            }
            return (IActiveMqChannelDeliver)JsonConvert.DeserializeObject(channelDeliver.ToString(), type);
        }

        /// <summary>
        ///  功能：创建一个接收者分发器
        /// </summary>
        /// <param name="channelDeliver">接收者分发器参数</param>
        /// <returns></returns>
        private IActiveMQReceiverDeliver CreateReceiverDeliver(JObject channelDeliver)
        {
            var typeName = channelDeliver["_type"].ToString();
            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new Exception("无法解析类别: " + typeName);
            }
            return (IActiveMQReceiverDeliver)JsonConvert.DeserializeObject(channelDeliver.ToString(), type);
        }

        /// <summary>
        /// 功能：混合接收者分发器，将多个接收者分发器混合到一起
        /// </summary>
        public class CompositeReceiverDeliver : IActiveMQReceiverDeliver
        {
            #region 变量定义  

            /// <summary>
            ///     接收者分发器集合
            /// </summary>
            private readonly List<IActiveMQReceiverDeliver> m_delivers = new List<IActiveMQReceiverDeliver>();

            #endregion

            #region 方法定义

            /// <summary>
            ///     功能：获取消息接收者的 Id
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public string GetReceiverId(object target)
            {
                foreach (var channelDeliver in m_delivers)
                {
                    var r = channelDeliver.GetReceiverId(target);
                    if (r != null)
                    {
                        return r;
                    }
                }
                return null;
            }

            /// <summary>
            ///     功能：添加一个消息接收者分发器
            /// </summary>
            /// <param name="deliver">消息接收者分发器</param>
            public void Add(IActiveMQReceiverDeliver deliver)
            {
                m_delivers.Add(deliver);
            }

            #endregion
        }

        /// <summary>
        ///     功能：混合通道分发器，将多个通道分发器混合到一起
        /// </summary>
        public class CompositeChannelDeliver : IActiveMqChannelDeliver
        {
            #region  变量定义 

            /// <summary>
            ///     消息分发器集合
            /// </summary>
            private readonly List<IActiveMqChannelDeliver> m_channelDelivers = new List<IActiveMqChannelDeliver>();

            #endregion

            #region  方法定义

            /// <summary>
            ///  功能：获取队列的名称
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public string GetChannelName(object target)
            {
                foreach (var channelDeliver in m_channelDelivers)
                {
                    var r = channelDeliver.GetChannelName(target);
                    if (r != null)
                    {
                        return r;
                    }
                }
                return null;
            }

            /// <summary>
            ///  功能：添加一个消息通道分发器
            /// </summary>
            /// <param name="channelDeliver">消息通道分发器</param>
            public void Add(IActiveMqChannelDeliver channelDeliver)
            {
                m_channelDelivers.Add(channelDeliver);
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// 功能：默认接收者分发器，不标记接收者属性
    /// </summary>
    internal class DefaultReceiverDeliver : IActiveMQReceiverDeliver
    {
        #region 方法定义 

        /// <summary>
        ///  功能：获取消息接收者的 Id
        /// </summary>
        /// <param name="target">消息</param>
        /// <returns></returns>
        public string GetReceiverId(object target)
        {
            return null;
        }

        #endregion
    }

    /// <summary>
    ///  功能：默认通道分发器，发送给第一个通道
    /// </summary>
    internal class DefaultChannelDeliver : IActiveMqChannelDeliver
    {
        #region 方法定义 

        /// <summary>
        ///  功能：获取队列的名称
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public string GetChannelName(object target)
        {
            return null;
        }

        #endregion
    }

    /// <summary>
    /// 功能：ActiveMQ 消息发送器
    /// </summary>
    internal class ActiveMessageQueueMessagePublisher : IDisposable
    {
        #region  构造函数 

        /// <summary>
        /// 功能：创建一个 Publisher 实例
        /// </summary>
        /// <param name="connection">mq 连接</param>
        /// <param name="queue">队列名称</param>
        public ActiveMessageQueueMessagePublisher(IConnection connection, string queue)
        {
            m_session = connection.CreateSession();
            IDestination destination = m_session.GetDestination(queue);
            m_producer = m_session.CreateProducer(destination);
            m_producer.DeliveryMode = MsgDeliveryMode.Persistent;
        }

        #endregion

        #region 方法定义

        /// <summary>
        /// 功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///  功能：发送一个消息，将消息发入队列
        /// </summary>
        /// <param name="target">被发送的消息</param>
        /// <param name="receiverId">接收者Id</param>
        public void Put(object target, string receiverId)
        {
            var request = BuildMessage(target, receiverId);
            m_producer.Send(request);
        }

        /// <summary>
        ///   功能：发送一个消息，将消息发入队列 (异步方式)
        /// </summary>
        /// <param name="target">被发送的消息</param>
        /// <param name="receiverId">接收者Id</param>
        public Task PutAsync(object target, string receiverId)
        {
            var request = BuildMessage(target, receiverId);
            return Task.Run(() => m_producer.Send(request));
        }

        /// <summary>
        /// 功能：构建一个消息对象
        /// </summary>
        /// <param name="target">实际要传输的内容</param>
        /// <param name="receiverId">接收者ID</param>
        /// <returns></returns>
        private ITextMessage BuildMessage(object target, string receiverId)
        {
            var messageStr = JsonConvert.SerializeObject(target);
            var request = m_session.CreateTextMessage(messageStr);
            request.NMSDeliveryMode = MsgDeliveryMode.Persistent;

            // 为 实现消息接口的对象做特殊处理
            IIdentityMessage identityMessage = target as IIdentityMessage;
            if (identityMessage != null)
            {
                request.NMSCorrelationID = identityMessage.MessageId;
            }
            request.Properties[ActiveMessageQueue.ExternPropertyNameMessageTypeName] =
                target.GetType().AssemblyQualifiedName;
            if (!string.IsNullOrWhiteSpace(receiverId))
            {
                request.Properties["receiverid"] = receiverId;
            }
            request.Text = JsonConvert.SerializeObject(target);
            return request;
        }

        /// <summary>
        ///     功能：回收资源
        /// </summary>
        /// <param name="disposing">是否使用 Dispose 回收</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            using (m_session)
            using (m_producer)
            {
            }
        }

        #endregion

        /// <summary>
        ///     功能：允许对象在“垃圾回收”回收之前尝试释放资源并执行其他清理操作。
        /// </summary>
        ~ActiveMessageQueueMessagePublisher()
        {
            Dispose(false);
        }

        #region  字段变量 

        /// <summary>
        ///     ActiveMQ 消息生产者
        /// </summary>
        private readonly IMessageProducer m_producer;

        /// <summary>
        ///     会话实例
        /// </summary>
        private readonly ISession m_session;

        #endregion
    }

    /// <summary>
    ///     功能：ActiveMQ 消息发送者集合
    /// </summary>
    internal class ActiveMessageQueueMessagePublishers : IMessagePublisher, IBroadcastPublisher
    {
        #region 变量定义

        /// <summary>
        ///     通道分发器
        /// </summary>
        private readonly IActiveMqChannelDeliver m_channelDeliver;

        /// <summary>
        ///     消息队列
        /// </summary>
        private readonly ActiveMessageQueue m_messageQueue;

        #endregion

        #region 构造函数

        /// <summary>
        ///     功能：创建一个 Publisher 实例
        /// </summary>
        /// <param name="messageQueue">mq 连接</param>
        /// <param name="queueName">队列名称</param>
        public ActiveMessageQueueMessagePublishers(ActiveMessageQueue messageQueue, string queueName)
        {
            m_messageQueue = messageQueue;
            m_publishers = new Dictionary<string, ActiveMessageQueueMessagePublisher>(StringComparer.OrdinalIgnoreCase);
            m_channelDeliver = messageQueue.GetChannelDeliver();
            m_receiverDeliver = messageQueue.GetReceiverDeliver();
            foreach (var connectionPair in messageQueue.GetConnections())
            {
                var publisher = new ActiveMessageQueueMessagePublisher(connectionPair.Value, queueName);
                m_publishers[connectionPair.Key] = publisher;
            }
        }

        #endregion

        #region  方法定义 

        /// <summary>
        ///     功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///  功能：发送一个消息，将消息发入队列
        /// </summary>
        /// <param name="target"></param>
        public void Put(object target)
        {
            string channelName = m_channelDeliver.GetChannelName(target);

            var publisher = GetPublisher(channelName);
            string recceiverId = m_receiverDeliver.GetReceiverId(target);
            publisher.Put(target, recceiverId);
        }

        /// <summary>
        /// 功能：发送一个消息，将消息发入队列
        /// </summary>
        /// <param name="target"></param>
        public Task PutAsync(object target)
        {
            string channelName = m_channelDeliver.GetChannelName(target);

            var publisher = GetPublisher(channelName);
            string recceiverId = m_receiverDeliver.GetReceiverId(target);
            return publisher.PutAsync(target, recceiverId);
        }

        /// <summary>
        /// 功能：获取指定通道名的消息发送器
        /// </summary>
        /// <param name="channelName">消息通道名称</param>
        /// <returns></returns>
        private ActiveMessageQueueMessagePublisher GetPublisher(string channelName)
        {
            if (string.IsNullOrWhiteSpace(channelName))
            {
                channelName = m_messageQueue.GetDefaultChannelName();
            }
            ActiveMessageQueueMessagePublisher publisher;
            if (m_publishers.TryGetValue(channelName, out publisher))
            {
                return publisher;
            }

            throw new Exception("未能找到指定队列: " + channelName);
        }

        /// <summary>
        ///     功能：回收资源
        /// </summary>
        /// <param name="disposing">是否使用 Dispose 回收</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            if (m_publishers != null)
            {
                m_publishers.Values.Dispose();
            }
        }

        #endregion

        /// <summary>
        ///     功能：允许对象在“垃圾回收”回收之前尝试释放资源并执行其他清理操作。
        /// </summary>
        ~ActiveMessageQueueMessagePublishers()
        {
            Dispose(false);
        }

        #region  字段变量 

        /// <summary>
        ///     MQ发布者列表
        /// </summary>
        private readonly IDictionary<string, ActiveMessageQueueMessagePublisher> m_publishers;

        /// <summary>
        ///     消息接收者分发器
        /// </summary>
        private readonly IActiveMQReceiverDeliver m_receiverDeliver;

        #endregion
    }

    /// <summary>
    ///     功能：消息接收者分发器
    /// </summary>
    public interface IActiveMQReceiverDeliver
    {
        #region  方法定义 

        /// <summary>
        ///     功能：获取消息接收者的 Id
        /// </summary>
        /// <param name="target">消息</param>
        /// <returns></returns>
        string GetReceiverId(object target);

        #endregion
    }

    /// <summary>
    ///     功能：消息接收者实现
    /// </summary>
    internal class ActiveMessageQueueMessageReceivers : IMessageReceiver, IBroadcastReceiver
    {
        #region   变量定义  

        /// <summary>
        ///     消息队列
        /// </summary>
        private readonly ActiveMessageQueue m_messageQueue;

        /// <summary>
        ///     队列名称
        /// </summary>
        private readonly string m_queueName;

        /// <summary>
        ///     接收者ID
        /// </summary>
        private readonly string m_receiverId;

        /// <summary>
        ///     消息接收者集合
        /// </summary>
        private List<ActiveMessageQueueMessageReceiver> m_receivers;

        #endregion

        #region  构造函数  

        /// <summary>
        ///     功能：创建一个消息接收者集合
        /// </summary>
        /// <param name="messageQueue"></param>
        /// <param name="queueName"></param>
        /// <param name="receiverId"></param>
        public ActiveMessageQueueMessageReceivers(ActiveMessageQueue messageQueue, string queueName, string receiverId)
        {
            m_messageQueue = messageQueue;
            m_queueName = queueName;
            m_receiverId = receiverId;
        }

        #endregion

        #region 方法定义 

        /// <summary>
        ///     功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     接收消息事件
        /// </summary>
        public event EventHandler<MessageEventArgs> Received = delegate { };

        /// <summary>
        ///     功能：启动监听
        /// </summary>
        public void Start()
        {
            m_receivers = new List<ActiveMessageQueueMessageReceiver>();

            foreach (var connection in m_messageQueue.GetConnections())
            {
                var receiver = new ActiveMessageQueueMessageReceiver(connection.Key, connection.Value, m_queueName,
                    m_receiverId);
                receiver.Received += receiver_Received;
                receiver.Start();
                m_receivers.Add(receiver);
            }
        }

        /// <summary>
        ///     功能：收到消息事件
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">消息参数</param>
        private void receiver_Received(object sender, MessageEventArgs e)
        {
            // 防止两个消息同时从不同 channel 中获取，并执行
            lock (this)
            {
                Received(this, e);
            }
        }

        /// <summary>
        ///     功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="disposing">是否通过 .Dispose 方法释放资源</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            if (m_receivers != null)
            {
                m_receivers.Dispose();
            }
        }

        #endregion

        /// <summary>
        ///     功能：允许对象在“垃圾回收”回收之前尝试释放资源并执行其他清理操作。
        /// </summary>
        ~ActiveMessageQueueMessageReceivers()
        {
            Dispose(false);
        }
    }

    /// <summary>
    /// 功能：消息接收器
    /// </summary>
    internal class ActiveMessageQueueMessageReceiver : IDisposable
    {
        #region  变量定义

        /// <summary>
        ///     队列名称
        /// </summary>
        private readonly string m_channelName;

        /// <summary>
        ///     连接实例
        /// </summary>
        private readonly IConnection m_connection;

        /// <summary>
        ///     消息接收者
        /// </summary>
        private readonly IMessageConsumer m_receiver;

        /// <summary>
        ///     当前会话
        /// </summary>
        private readonly ISession m_session;

        #endregion

        #region  构造函数 

        /// <summary>
        ///   功能：创建一个 ActiveMessageQueueMessageReceiver
        /// </summary>
        /// <param name="channelName">通道名称</param>
        /// <param name="connection"></param>
        /// <param name="queue"></param>
        /// <param name="receiverId"></param>
        public ActiveMessageQueueMessageReceiver(string channelName, IConnection connection, string queue,
            string receiverId)
        {
            m_channelName = channelName;
            m_connection = connection;

            m_session = connection.CreateSession();
            var q = m_session.GetDestination(queue);
            StringBuilder selectorBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(receiverId))
            {
                selectorBuilder.Append("receiverid='" + receiverId.Trim() + "'");
            }
            var selector = selectorBuilder.ToString();
            m_receiver = m_session.CreateConsumer(q, string.IsNullOrEmpty(selector) ? null : selector);
        }
        public void Start()
        {
            m_receiver.Listener += m_receiver_Listener;
        }
        #endregion

        #region 方法定义

        /// <summary>
        ///     功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     功能：处理接收到的消息
        /// </summary>
        /// <param name="message"></param>
        private void m_receiver_Listener(IMessage message)
        {
            ITextMessage textMessage = message as ITextMessage;

            // 内部只处理 ITextMessage，其它消息全部丢弃
            if (textMessage == null)
            {
                return;
            }

            var typeFullName = textMessage.Properties[ActiveMessageQueue.ExternPropertyNameMessageTypeName].ToString();
            var orignalMessageType = Type.GetType(typeFullName);
            if (orignalMessageType == null)
            {
                return;
            }
            var originalMessage = JsonConvert.DeserializeObject(textMessage.Text, orignalMessageType);
            Received(this, new MessageEventArgs(m_channelName, originalMessage));
        }

        /// <summary>
        ///     功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="disposing">是否通过 .Dispose 方法释放资源</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            using (m_session)
            using (m_receiver)
            {
            }
        }

        #endregion

        /// <summary>
        ///     接收到消息事件
        /// </summary>
        public event EventHandler<MessageEventArgs> Received = delegate { };

        /// <summary>
        ///     功能：允许对象在“垃圾回收”回收之前尝试释放资源并执行其他清理操作。
        /// </summary>
        ~ActiveMessageQueueMessageReceiver()
        {
            Dispose(false);
        }

    }

    /// <summary>
    ///     功能：通道分发器
    /// </summary>
    public interface IActiveMqChannelDeliver
    {
        #region  方法定义  

        /// <summary>
        ///     功能：获取队列的名称
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        string GetChannelName(object target);

        #endregion
    }
}
