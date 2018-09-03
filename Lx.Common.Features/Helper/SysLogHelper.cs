using log4net;
using log4net.Appender;
using Lx.Common.Features.Helper;
using Lx.Common.Interface;
using Lx.Common.Models.DataBase;
using Lx.Common.Models.Log;
using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Helper
{

    /// 功能描述:系统日志帮助类 
    public class SysLogHelper
    {
        #region  变量定义  

        //消息发布者
        private static IMessagePublisher m_messagePublisher = null;

        //文件日志记录器
        private static ILog m_fileLog = LogManager.GetLogger("FileLogger");


        //数据库日志记录器
        private static ILog m_dataBaseLog = LogManager.GetLogger("DataBaseLogger");

        #endregion

        #region 构造函数 

        static SysLogHelper()
        {
            try
            {
            }
            catch (Exception ex)
            {
                string strErrorMessage =GetErrorLogInfo(ex, true);
                LogMessage("SysLogHelper.Static", strErrorMessage);
            }
        }

        #endregion

        #region 方法定义

        /// <summary>
        /// 功能:获取日志文件所在路径
        /// </summary>
        /// <returns>日志文件所在路径列表</returns>
        public static List<string> GetLogPath()
        {
            List<string> lstReturn = new List<string>();

            var logRepository = LogManager.GetRepository();
            var logAppenders = logRepository.GetAppenders();
            var fileLogAppenders = logAppenders.Where(la => la.Name.StartsWith("LogFile", StringComparison.InvariantCultureIgnoreCase)).ToList();

            RollingFileAppender rfaTemp = null;
            fileLogAppenders.ForEach(fla =>
            {
                rfaTemp = fla as RollingFileAppender;
                if (null != rfaTemp)
                {
                    if (!string.IsNullOrWhiteSpace(rfaTemp.File))
                    {
                        string strLogPath = System.IO.Path.GetDirectoryName(rfaTemp.File);
                        string strTemp = lstReturn.Where(lp => lp.ToLower() == strLogPath.ToLower()).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(strTemp))
                        {
                            lstReturn.Add(strLogPath);
                        }
                    }
                }
            });

            return lstReturn;
        }

        /// <summary>
        /// 功能:获取详细错误日志信息
        /// </summary>
        /// <param name="ex">异常</param>    
        /// <param name="blnAddStackTrace">是否添加堆栈跟踪信息</param>
        /// <param name="blnAddTime">是否添加时间</param>
        /// <returns>错误日志信息</returns>
        public static string GetErrorLogInfo(Exception ex, bool blnAddStackTrace = false, bool blnAddTime = false)
        {
            if (null == ex)
            {
                return "";
            }

            var tn = " - ";
            var sbTemp = new StringBuilder();

            try
            {
                sbTemp.Append($"[异常原因]:{ex.Message}");
                sbTemp.Append(tn);
                sbTemp.Append($"[异常源]:{ex.Source}" );
                sbTemp.Append(tn);
                sbTemp.Append($"[异常方法]:{ex.TargetSite.ReflectedType.FullName},{ex.TargetSite.ToString()}");

                if (blnAddStackTrace)
                {
                    sbTemp.Append(tn);
                    sbTemp.Append($"[堆栈跟踪]:{ex.StackTrace}");
                }

                if (blnAddTime)
                {
                    sbTemp.Append(tn);
                    sbTemp.Append($"[异常时间]:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                }

                return sbTemp.ToString().Replace("\r\n", "");
            }
            catch (Exception)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 功能:记录日志消息到本地
        /// </summary>
        /// <param name="strSource">消息来源</param>
        /// <param name="strMessage">消息</param>
        /// <param name="strAddition">日志附加信息</param>
        /// <param name="logLevel">消息类型</param>
        /// <param name="writeLogType">输出类型</param>  
        public static void LogMessage(string strSource, string strMessage,
                                      LogLevel logLevel = LogLevel.Error, WriteLogType writeLogType = WriteLogType.FileDataBaseLog, string strAddition = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strMessage))
                {
                    return;
                }

                if (writeLogType == WriteLogType.FileLog)
                {
                    LogToFile(strSource, strMessage, strAddition, logLevel);
                }
               
                else if (writeLogType == WriteLogType.DataBaseLog)
                {
                    LogToDataBase(strSource, strMessage, strAddition, logLevel);
                }
               
                else if (writeLogType == WriteLogType.FileDataBaseLog)
                {
                    LogToFile(strSource, strMessage, strAddition, logLevel);
                    LogToDataBase(strSource, strMessage, strAddition, logLevel);
                }
                else
                {
                    LogToFile(strSource, strMessage, strAddition, logLevel);
                    LogToDataBase(strSource, strMessage, strAddition, logLevel);
                }

                LogToConsole(strSource, strMessage, strAddition, logLevel);
            }
            catch (Exception ex)
            {
                string strErrorMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                LogToFile("SysLogHelper.LogMessage", strErrorMessage, "", LogLevel.Error);
            }
        }

        /// <summary>
        /// 功能:记录日志消息到日志服务
        /// </summary>
        /// <param name="writeLogType">写日志类型</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="strSource">日志的来源</param>
        /// <param name="strMessage">需记录的日志值</param> 
        /// <param name="strAddition">日志附加信息</param>
        public static void LogServiceMessage(string strSource, string strMessage,
                                             LogLevel logLevel = LogLevel.Error, WriteLogType writeLogType = WriteLogType.FileDataBaseLog,
                                              string strAddition = "")
        {
            try
            {
                if (null == m_messagePublisher)
                {
                    m_messagePublisher = MessageQueueHelper.GetMessageQueueFromPool().GetMessagePublisher(QueueName.LxLogQueue.ToString());
                }

                if (null != m_messagePublisher)
                {
                    CustomLogMessage clmTemp = new CustomLogMessage()
                    {
                        LogSource = strSource,
                        LogMessage = strMessage,
                        LogAddition = strAddition,
                        LogLevel = logLevel,
                        WriteLogType = writeLogType
                     
                    };
                    m_messagePublisher.Put(clmTemp);
                }
            }
            catch (Exception ex)
            {
                var strErrorMessage = GetErrorLogInfo(ex, true);
                LogToFile("SysLogHelper.LogServiceMessage", strErrorMessage, "", LogLevel.Error);
            }
        }

        /// <summary>
        /// 功能:记录日志到控制台
        /// </summary>
        /// <param name="strSource">消息来源</param>
        /// <param name="strMessage">消息</param>
        /// <param name="logAddition">日志附加信息</param>
        ///  <param name="logLevel">消息类型</param>
        private static void LogToConsole(string strSource, string strMessage, string logAddition, LogLevel logLevel)
        {
           

            if (strMessage.Length > 180)
            {
                strMessage = strMessage.Substring(0, 180);
            }

          var  strTemp =$"Source:{strSource}--Message:{strMessage}--Addition:{logAddition}--Machine:{GlobalHelper.GetMachineName()},Time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";

            Console.WriteLine(strTemp);
        }


        /// <summary>
        /// 功能:记录日志到文件
        /// </summary>
        /// <param name="strSource">消息来源</param>
        /// <param name="strMessage">消息</param>
        /// <param name="logAddition">日志附加信息</param>
        /// <param name="logLevel">消息类型</param>
        private static void LogToFile(string strSource, string strMessage, string logAddition, LogLevel logLevel)
        {
            var strTemp = $"Source:{strSource}--Message:{strMessage}--Addition:{logAddition}--Machine:{GlobalHelper.GetMachineName()},Time:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";

            if (logLevel == LogLevel.Error)
            {
                m_fileLog.ErrorFormat(strTemp);
            }
            else if (logLevel == LogLevel.Information)
            {
                m_fileLog.InfoFormat(strTemp);
            }
            else
            {
                m_fileLog.ErrorFormat(strTemp);
                m_fileLog.InfoFormat(strTemp);
            }
        }

        /// <summary>
        /// 功能:记录日志到数据库
        /// </summary>
        /// <param name="strSource">消息来源</param>
        /// <param name="strMessage">消息</param>
        /// <param name="logAddition">日志附加信息</param>
        /// <param name="logLevel">消息类型</param>
        private static void LogToDataBase(string strSource, string strMessage, string logAddition, LogLevel logLevel)
        {
            var  clogTemp = new TCommonlog();
            clogTemp.LogSource = strSource;
            clogTemp.LogMessage =$"{strMessage}--Machine:{GlobalHelper.GetMachineName()}";
            clogTemp.LogAddition = logAddition;

            if (logLevel == LogLevel.Error)
            {
                m_dataBaseLog.Error(clogTemp);
            }
            else if (logLevel == LogLevel.Information)
            {
                m_dataBaseLog.Info(clogTemp);
            }
        }

        #endregion
    }
}
