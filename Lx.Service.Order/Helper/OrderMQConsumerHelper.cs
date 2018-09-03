using Lx.Common.Helper;
using Lx.Common.Interface;
using Lx.Common.Models.Var;
using Lx.Service.Order.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lx.Common.Models.Para;

namespace Lx.Service.Order.Helper
{
    public class OrderMQConsumerHelper
    {
        #region  变量定义  
        //消息消费者
        private static IMessageReceiver m_messageReceiver = null;

        #endregion


        #region  方法定义  

        /// <summary>
        /// 功能:初始化
        /// </summary>
        public static void Initalize()
        {
            try
            {
                m_messageReceiver = MessageQueueHelper.GetMessageQueueFromPool().GetMessageReceiver(QueueName.AutoPayQueue.ToString(), "");
                m_messageReceiver.Received += m_messageReceiver_Received;
                m_messageReceiver.Start();
                Console.WriteLine("订单服务已启动!");
            }
            catch (Exception ex)
            {
                string strMessage = $"初始化,原因:{SysLogHelper.GetErrorLogInfo(ex, true)}";
                SysLogHelper.LogMessage("Lx.Service.Initalize", strMessage);
            }
        }

        /// <summary>
        /// 功能:释放资源
        /// </summary>
        public static void Release()
        {
            if (null != m_messageReceiver)
            {
                m_messageReceiver.Dispose();
            }
        }

        /// <summary>
        /// 接收代付订单
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">消息参数</param>
        private static void m_messageReceiver_Received(object sender, MessageEventArgs e)
        {
            var orderModel = e.Message as OrderMqMessage;
            try
            {
                if(null!=orderModel)
                {
                    PayOrderService.AddPayOrder(orderModel);
                }
              

            }
            catch (Exception ex)
            {
                string strMessage = $"接收代付订单失败,原因:{SysLogHelper.GetErrorLogInfo(ex, true)}";
                SysLogHelper.LogMessage("Lx.Service.m_messageReceiver_Received", strMessage);
            }
        }

        #endregion
    }
}
