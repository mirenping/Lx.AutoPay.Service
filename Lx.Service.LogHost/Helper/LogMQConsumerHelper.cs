using Lx.Common.Features.Helper;
using Lx.Common.Helper;
using Lx.Common.Interface;
using Lx.Common.Models;
using Lx.Common.Models.Log;
using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.LogHost.Helper
{
    /// <summary>
    /// 功能描述:日志消息队列消费帮助类
    /// </summary>
    public class LogMQConsumerHelper
    {
        #region  变量定义  

        private static bool m_enableFileLog = false;
        private static bool m_enableDataBaseLog = false;

        //消息消费者
        private static IMessageReceiver m_messageReceiver = null;

        #endregion

        #region  构造函数 

        static LogMQConsumerHelper()
        {
            try
            {
                JsonConfigInfo lsConfigInfo = ConfigHelper.LoadFromFile("LogService.json");

                m_enableFileLog = lsConfigInfo.GetValue<bool>("EnableFileLog");
                m_enableDataBaseLog = lsConfigInfo.GetValue<bool>("EnableDataBaseLog");
            }
            catch (Exception ex)
            {
                string strMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                SysLogHelper.LogMessage("LogMQConsumerHelper.static", strMessage);
            }
        }

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:初始化
        /// </summary>
        public static void Initalize()
        {
            try
            {
                m_messageReceiver = MessageQueueHelper.GetMessageQueueFromPool().GetMessageReceiver(QueueName.LxLogQueue.ToString(), "");
                m_messageReceiver.Received += m_messageReceiver_Received;
                m_messageReceiver.Start();
                Console.WriteLine("日志消息队列消费者已启动!");
            }
            catch (Exception ex)
            {
                string strErrorMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                SysLogHelper.LogMessage("LogMQConsumerHelper.Static", strErrorMessage);
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
        /// 接收日志消息
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">消息参数</param>
        private static void m_messageReceiver_Received(object sender, MessageEventArgs e)
        {
            try
            {
                //写入日志服务
                var clmTemp = e.Message as CustomLogMessage;
                if (null != clmTemp)
                {
                    if (m_enableFileLog)
                    {
                        if (clmTemp.WriteLogType == WriteLogType.AllLog ||
                            clmTemp.WriteLogType == WriteLogType.FileDataBaseLog ||
                            clmTemp.WriteLogType == WriteLogType.FileLog)
                        {
                            SysLogHelper.LogMessage("Q:" + clmTemp.LogSource, clmTemp.LogMessage, clmTemp.LogLevel, WriteLogType.FileLog, clmTemp.LogAddition);
                        }
                    }

                    if (m_enableDataBaseLog)
                    {
                        if (clmTemp.WriteLogType == WriteLogType.AllLog ||
                            clmTemp.WriteLogType == WriteLogType.FileDataBaseLog ||
                            clmTemp.WriteLogType == WriteLogType.DataBaseLog)
                        {
                            SysLogHelper.LogMessage("Q:" + clmTemp.LogSource, clmTemp.LogMessage, clmTemp.LogLevel, WriteLogType.DataBaseLog, clmTemp.LogAddition);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("LogMQConsumerHelper.messageReceiver_Received", ex.Message);
            }
        }

        #endregion
    }
}
