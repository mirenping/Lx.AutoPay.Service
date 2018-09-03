using Lx.Common.Helper;
using Lx.Common.Interface;
using Lx.Common.Models.Var;
using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfHelper
{
    /// <summary>
    /// 功能描述代付订单消息队列
    /// </summary>
    public class PayOrderMqHelper
    {
        #region 常量定义

        //生产者
        private readonly static IMessagePublisher m_publisher;

        #endregion


        #region 构造函数
        static PayOrderMqHelper()
        {
            try
            {
                var messageQueue = MessageQueueHelper.GetMessageQueueFromPool();
                m_publisher = messageQueue.GetMessagePublisher(QueueName.AutoPayQueue.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Lx.Service.WcfHelper.SendPayOrderMessageToMq 初始化MQ失败。Ex:=" + ex.Source + ex.Message + ex.StackTrace);

            }
        }
        #endregion
        /// <summary>
        /// 功能描述:添加代付订单到消息队列
        /// </summary>
        /// <param name="orderMessage"></param>
        public static FeedBackResponse<bool> SendPayOrderMessageToMq(OrderMqMessage orderMessage)
        {

            var fbrReturn = new FeedBackResponse<bool>();
            try
            {
                m_publisher.Put(orderMessage);
            }
            catch (Exception ex)
            {

                throw new Exception("Lx.Service.WcfHelper.SendPayOrderMessageToMq 添加代付订单到消息队列中失败。Ex:=" + ex.Source + ex.Message + ex.StackTrace);
            }
            return fbrReturn;
        }
    }
}
