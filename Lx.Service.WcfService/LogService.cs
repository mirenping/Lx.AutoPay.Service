using Lx.Common.Models.DataBase;
using Lx.Common.Models.Var;
using Lx.Service.WcfContract;
using Lx.Service.WcfHelper;
using Lx.Service.WcfService.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfService
{
    /// <summary>
    /// 功能描述:日志服务
    /// </summary>      
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                     Namespace = "Lx.Service.WcfService",
                     Name = "LogService")]
    [ErrorInterceptor]
    public class LogService : ILogService
    {
        #region 测试  

        /// <summary>
        /// 功能描述: 测试服务
        /// </summary>
        /// <param name="strParam"></param>
        /// <returns>返回结果</returns>       
        public FeedBackResponse<string> TestService(string strParam)
        {
            strParam = strParam ?? "";
            return new FeedBackResponse<string>() { ResultData = strParam + "LogService is ok!" };
        }

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:测试日志
        /// </summary>             
        public FeedBackResponse<string> TestLog()
        {
            return LogServiceHelper.TestLog();
        }

        /// <summary>
        /// 功能:获取日志文件内容
        /// </summary>
        /// <param name="strLogFileName">日志文件名称或路径</param>
        /// <returns>日志文件内容</returns>
        public FeedBackResponse<string> GetLogFileContent(string strLogFileName)
        {
            return LogServiceHelper.GetLogFileContent(strLogFileName);
        }

        /// <summary>
        /// 功能:搜索日志文件列表
        /// </summary>       
        /// <param name="strSearchName">搜索名称</param>
        /// <returns>日志文件列表</returns>
        public FeedBackResponse<List<string>> SearchLogFiles(string strSearchName)
        {
            return LogServiceHelper.SearchLogFiles(strSearchName);
        }

        /// <summary>
        /// 功能:搜索日志数据列表 
        /// </summary>
        /// <param name="strLevel">日志级别</param>
        /// <param name="strSource">日志来源</param>
        /// <param name="dtmStartTime">日志起始时间</param>
        /// <param name="dtmEndTime">日志结束时间</param>
        /// <param name="strMessage">日志信息</param>
        /// <param name="strAddition">附加信息</param>
        /// <returns>日志数据列表 </returns>        
        public FeedBackResponse<List<TCommonlog>> SearchLogData(string strLevel, string strSource,
                                                                    DateTime dtmStartTime, DateTime? dtmEndTime,
                                                                    string strMessage, string strAddition)
        {
            return LogServiceHelper.SearchLogData(strLevel, strSource, dtmStartTime, dtmEndTime, strMessage, strAddition);
        }

        /// <summary>
        /// 功能:记录日志消息
        /// </summary>
        /// <param name="writeLogType">写日志类型</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="logSource">日志的来源</param>
        /// <param name="logValue">需记录的日志值</param>   
        /// <param name="logAddition">日志附加信息</param>      
        public FeedBackResponse<string> WriteLog(WriteLogType writeLogType, LogLevel logLevel,
                                                string logSource, string logValue, string logAddition)
        {
            FeedBackResponse<string> fbrTemp01 = null;
            FeedBackResponse<string> fbrTemp02 = null;
            FeedBackResponse<string> fbrTemp03 = null;
            FeedBackResponse<string> fbrReturn = new FeedBackResponse<string>();

            switch (writeLogType)
            {
                case WriteLogType.FileLog:
                    fbrReturn = LogServiceHelper.WriteFileLog(logLevel, logSource, logValue, logAddition);
                    break;
             
                case WriteLogType.DataBaseLog:
                    fbrReturn = LogServiceHelper.WriteDataBaseLog(logLevel, logSource, logValue, logAddition);
                    break;
             
                case WriteLogType.FileDataBaseLog:
                    fbrTemp01 = LogServiceHelper.WriteFileLog(logLevel, logSource, logValue, logAddition);
                    fbrTemp02 = LogServiceHelper.WriteDataBaseLog(logLevel, logSource, logValue, logAddition);
                    CombineResponse(fbrReturn, fbrTemp01, fbrTemp02, fbrTemp03);
                    break;
             
                case WriteLogType.AllLog:
                    fbrTemp01 = LogServiceHelper.WriteFileLog(logLevel, logSource, logValue, logAddition);
                    fbrTemp02 = LogServiceHelper.WriteDataBaseLog(logLevel, logSource, logValue, logAddition);
                    break;
            }

            return fbrReturn;
        }

        /// <summary>
        /// 功能:组合处理结果
        /// </summary>
        /// <param name="fbrReturn">待返回的结果</param>
        /// <param name="fbrTemp01">相关结果01</param>
        /// <param name="fbrTemp02">相关结果02</param>
        /// <param name="fbrTemp03">相关结果03</param>
        private void CombineResponse(FeedBackResponse<string> fbrReturn,
                                    FeedBackResponse<string> fbrTemp01,
                                    FeedBackResponse<string> fbrTemp02,
                                    FeedBackResponse<string> fbrTemp03)
        {
            bool blnHasError = false;

            if (null != fbrTemp01 && !fbrTemp01.Result)
            {
                blnHasError = true;
                fbrReturn.Message += ";" + fbrTemp01.Message;
            }

            if (null != fbrTemp01 && !fbrTemp01.Result)
            {
                blnHasError = true;
                fbrReturn.Message += ";" + fbrTemp01.Message;
            }

            if (null != fbrTemp01 && !fbrTemp01.Result)
            {
                blnHasError = true;
                fbrReturn.Message += ";" + fbrTemp01.Message;
            }

            if (!blnHasError)
            {
                fbrReturn.Result = true;
            }
        }

        #endregion         
    }
}

