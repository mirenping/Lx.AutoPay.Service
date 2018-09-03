using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.Common
{
    /// <summary>
    ///  功能描述:提供泛型的服务主机
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceHost<T> : ServiceHost where T : class
    {

        #region   变量定义  

        //服务的名称
        private string m_serviceName = "";

        #endregion

        #region   构造函数  

        /// <summary>
        /// 服务主机构造函数
        /// </summary>
        public ServiceHost()
            : base(typeof(T))
        {
            m_serviceName = typeof(T).Name;
        }

        /// <summary>
        /// 服务主机构造函数
        /// </summary>
        /// <param name="baseAddresses">String类型的数组,包含承载服务的基址</param>
        public ServiceHost(params string[] baseAddresses)
            : base(typeof(T), ConvertBaseAddress(baseAddresses))
        {
            m_serviceName = typeof(T).Name;
        }

        /// <summary>
        /// 服务主机构造函数
        /// </summary>       
        /// <param name="baseAddresses">System.Uri类型的数组,包含承载服务的基址</param>
        public ServiceHost(params System.Uri[] baseAddresses)
            : base(typeof(T), baseAddresses)
        {
            m_serviceName = typeof(T).Name;
        }

        #endregion

        #region   属性定义  

        /// <summary>
        /// 是否启用元数据交换
        /// </summary>
        public bool EnableMetadataExchange
        {
            get
            {
                ServiceMetadataBehavior metadataBehavior = null;

                //查找元数据交换服务行为
                metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (null == metadataBehavior)
                {
                    return false;
                }

                return metadataBehavior.HttpGetEnabled;
            }

            set
            {
                if (State == CommunicationState.Opened)
                {
                    throw new InvalidOperationException("服务已经打开,无法启用元数据交换!");
                }

                ServiceMetadataBehavior metadataBehavior = null;

                ///查找元数据交换服务行为
                metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
                if (value == true)
                {
                    if (null == metadataBehavior)
                    {
                        metadataBehavior = new ServiceMetadataBehavior();
                        metadataBehavior.HttpGetEnabled = true;
                        Description.Behaviors.Add(metadataBehavior);
                    }
                    else
                    {
                        metadataBehavior.HttpGetEnabled = true;
                    }

                    //添加元数据交换终结点
                    if (HasMexServiceEndPoint() == false)
                    {
                        AddAllMexEndPoint();
                    }
                }
                else
                {
                    if (null != metadataBehavior)
                    {
                        metadataBehavior.HttpGetEnabled = false;
                    }
                }
            }
        }

        #endregion

        #region   重写方法  

        /// <summary>
        /// 功能:服务主机被打开时
        /// </summary>
        protected override void OnOpening()
        {
            //验证队列是否存在并添加
            foreach (ServiceEndpoint sEndPoint in Description.Endpoints)
            {
                VerifyQueue(sEndPoint);
            }
           
            base.OnOpening();
        }

        /// <summary>
        /// 功能:服务主机被打开后
        /// </summary>
        protected override void OnOpened()
        {
            string strLogValue =$"服务({ m_serviceName})已经启动!";
         
            base.OnOpened();
        }

        /// <summary>
        /// 功能:服务主机被关闭时
        /// </summary>
        protected override void OnClosing()
        {
            string strLogValue = $"服务({ m_serviceName})已经关闭!";
         
            base.OnClosing();
        }

        /// <summary>
        /// 功能:添加元数据交换终结点
        /// </summary>
        private void AddAllMexEndPoint()
        {
            foreach (Uri baseAddress in BaseAddresses)
            {
                BindingElement beTemp = null;
                switch (baseAddress.Scheme.Trim().ToLower())
                {
                    case "net.tcp":
                        beTemp = new TcpTransportBindingElement();
                        break;
                    case "net.pipe":
                        beTemp = new NamedPipeTransportBindingElement();
                        break;
                    case "http":
                        beTemp = new HttpTransportBindingElement();
                        break;
                    case "https":
                        beTemp = new HttpsTransportBindingElement();
                        break;
                }

                if (null != beTemp)
                {
                    Binding bdTemp = new CustomBinding(beTemp);
                    AddServiceEndpoint(typeof(IMetadataExchange), bdTemp, "Mex");
                }
            }
        }

        /// <summary>
        /// 功能:判断是否有元数据交换终结点
        /// </summary>
        /// <returns></returns>
        public bool HasMexServiceEndPoint()
        {
            foreach (ServiceEndpoint endPoint in Description.Endpoints)
            {
                if (endPoint.Contract.ContractType == typeof(IMetadataExchange))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 功能:获取队列名称
        /// </summary>
        /// <param name="sEndPointUri">队列服务终结点地址</param>
        private string GetQueueName(string strQueuePath)
        {
            string strReturn = "";

            //最后一个'/'项的索引
            int lastIndex = strQueuePath.LastIndexOf('/');

            if (lastIndex != -1)
            {
                strReturn = strQueuePath.Substring(lastIndex + 1);
            }

            return strReturn;
        }

        /// <summary>
        /// 功能:验证队列并安装队列
        /// </summary>
        /// <param name="sEndPoint">服务终节点</param>
        private void VerifyQueue(ServiceEndpoint sEndPoint)
        {
            //如果是队列绑定
            if (sEndPoint.Binding is NetMsmqBinding)
            {
                //绑定的队列地址
                string strQueuePath = sEndPoint.Address.Uri.AbsoluteUri;

                if (!string.IsNullOrEmpty(strQueuePath))
                {
                    //获取队列名称
                    string strQueueName = GetQueueName(strQueuePath);

                    if (!string.IsNullOrEmpty(strQueueName))
                    {
                        //创建队列
                        CreateQueue(strQueueName, strQueuePath.ToLower().Contains("private"));
                    }
                }
            }
        }

        /// <summary>
        /// 功能:创建队列
        /// </summary>
        /// <param name="strQueueName">队列名称</param>
        /// <param name="isPrivate">是否是私有队列</param>
        private void CreateQueue(string strQueueName, bool isPrivate)
        {
            string strQueuePath = "";

            if (isPrivate)
            {
                strQueuePath = $@".\private$\{strQueueName}";
            }
            else
            {
                strQueuePath = $@".\{strQueueName}";
            }

            if (MessageQueue.Exists(strQueuePath) == false)
            {
                MessageQueue mqTemp = null;

                //创建队列
                mqTemp = MessageQueue.Create(strQueuePath, true);

                //设置队列访问权限
                mqTemp.SetPermissions("Administrators", MessageQueueAccessRights.FullControl);
            }
        }

        /// <summary>
        /// 功能:转换Sring地址到Uri地址
        /// </summary>
        /// <param name="baseAddresses">字符串地址集合</param>
        /// <returns>Uri地址集合</returns>
        private static Uri[] ConvertBaseAddress(string[] baseAddresses)
        {
            Uri[] returnValue = null;

            if (null != baseAddresses && baseAddresses.Length > 0)
            {
                Converter<string, Uri> addressConverter = delegate (string strAddress)
                {
                    return new Uri(strAddress);
                };

                returnValue = Array.ConvertAll(baseAddresses, addressConverter);
            }

            return returnValue;
        }

        #endregion
    }
}
