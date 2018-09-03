using Lx.Common.Models.DataBase;
using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfContract
{
    /// <summary>
    /// 功能描述:日志服务
    /// </summary>
    [ServiceContract(Namespace = "Lx.Service.WcfContract",
                     Name = "ILogService")]
    public interface ILogService
    {
        #region  测试  

        /// <summary>
        /// 功能描述:测试服务
        /// </summary>
        /// <param name="strParam"></param>
        /// <returns>返回结果</returns>
        [OperationContract()]
        FeedBackResponse<string> TestService(string strParam);

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:测试日志
        /// </summary>               
        [OperationContract()]
        FeedBackResponse<string> TestLog();

        /// <summary>
        /// 功能:获取日志文件内容
        /// </summary>
        /// <param name="strLogFileName">日志文件名称或路径</param>
        /// <returns>日志文件内容</returns>
        [OperationContract()]
        FeedBackResponse<string> GetLogFileContent(string strLogFileName);

        /// <summary>
        /// 功能:搜索日志文件列表
        /// </summary>       
        /// <param name="strSearchName">搜索名称</param>
        /// <returns>日志文件列表</returns>
        [OperationContract()]
        FeedBackResponse<List<string>> SearchLogFiles(string strSearchName);

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
        [OperationContract()]
        FeedBackResponse<List<TCommonlog>> SearchLogData(string strLevel, string strSource,
                                                            DateTime dtmStartTime, DateTime? dtmEndTime,
                                                            string strMessage, string strAddition);

        /// <summary>
        /// 功能:记录日志消息
        /// </summary>
        /// <param name="writeLogType">写日志类型</param>
        /// <param name="logLevel">日志级别</param>
        /// <param name="logSource">日志的来源</param>
        /// <param name="logValue">需记录的日志值</param>  
        /// <param name="logAddition">日志附加信息</param>
        [OperationContract()]
        FeedBackResponse<string> WriteLog(WriteLogType writeLogType, LogLevel logLevel,
                                         string logSource, string logValue, string logAddition);

        #endregion
    }
}
