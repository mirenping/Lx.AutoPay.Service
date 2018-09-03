using Lx.Common.Features.Helper;
using Lx.Common.Helper;
using Lx.Common.Models;
using Lx.Common.Models.DataBase;
using Lx.Common.Models.Var;
using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfHelper
{
   public class LogServiceHelper
    {
        #region 变量定义  

        private static bool m_enableFileLog = false;

        private static bool m_enableDataBaseLog = false;

        #endregion

        #region  构造函数 

        static LogServiceHelper()
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
                SysLogHelper.LogMessage("LogServiceHelper.static", strMessage);
            }
        }

        #endregion

        #region 方法定义  

        /// <summary>
        /// 功能:测试日志
        /// </summary>            
        public static FeedBackResponse<string> TestLog()
        {
            FeedBackResponse<string> fbrReturn = new FeedBackResponse<string>();

            SysLogHelper.LogMessage("LogServiceHelper.TestLog", "Test log data!", LogLevel.Information);
            SysLogHelper.LogServiceMessage("LogServiceHelper.TestLog", "Test log data!", LogLevel.Information);
            fbrReturn.ResultData = "ok";

            return fbrReturn;
        }

        /// <summary>
        /// 功能:获取日志文件内容
        /// </summary>
        /// <param name="strLogFileName">日志文件名称或路径</param>
        /// <returns>日志文件内容</returns>
        public static FeedBackResponse<string> GetLogFileContent(string strLogFileName)
        {
            FeedBackResponse<string> fbrReturn = new FeedBackResponse<string>();

            string strFilePath = System.IO.Path.GetDirectoryName(strLogFileName);
            if (string.IsNullOrWhiteSpace(strFilePath))
            {
                List<string> lstPath = SysLogHelper.GetLogPath();
                if (lstPath.Count == 0)
                {
                    fbrReturn.Message = "获取日志路径失败!";
                }
                else
                {
                    lstPath.ForEach(lp =>
                    {
                        string strLogFilePath = System.IO.Path.Combine(lp, strLogFileName);
                        if (FileHelper.IsFileExist(strLogFilePath))
                        {
                            fbrReturn.ResultData = GetLogContent(strLogFilePath);
                            return;
                        }
                    });
                }
            }
            else
            {
                fbrReturn.ResultData = GetLogContent(strLogFileName);
            }

            return fbrReturn;
        }

        /// <summary>
        /// 功能:搜索日志文件列表
        /// </summary>       
        /// <param name="strSearchName">搜索名称</param>
        /// <returns>日志文件列表</returns>
        public static FeedBackResponse<List<string>> SearchLogFiles(string strSearchName)
        {
            FeedBackResponse<List<string>> fbrReturn = new FeedBackResponse<List<string>>();

            List<string> lstReturn = new List<string>();

            List<string> lstPath = SysLogHelper.GetLogPath();
            lstPath.ForEach(lp =>
            {
                lstReturn.AddRange(System.IO.Directory.GetFiles(lp, "*" + strSearchName + "*"));
            });

            fbrReturn.ResultData = lstReturn;

            return fbrReturn;
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
        public static FeedBackResponse<List<TCommonlog>> SearchLogData(string strLevel, string strSource,
                                                                        DateTime dtmStartTime, DateTime? dtmEndTime,
                                                                        string strMessage, string strAddition)
        {
            var fbrReturn = new FeedBackResponse<List<TCommonlog>>();

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from tcommonlog ");
            strSql.Append("where 1=1 ");

            Dictionary<string, object> dictParameter = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(strLevel))
            {
                strSql.Append(" and lower(LogLevel) like ?LogLevel");
                dictParameter["LogLevel"] = "%" + strLevel.ToLower() + "%";
            }

            if (!string.IsNullOrEmpty(strSource))
            {
                strSql.Append(" and lower(LogSource) like ?LogSource");
                dictParameter["LogSource"] = "%" + strSource.ToLower() + "%";
            }

            if (!string.IsNullOrEmpty(strMessage))
            {
                strSql.Append(" and lower(LogMessage) like ?LogMessage");
                dictParameter["LogMessage"] = "%" + strMessage.ToLower() + "%";
            }

            if (!string.IsNullOrEmpty(strAddition))
            {
                strSql.Append(" and lower(LogAddition) like ?LogAddition");
                dictParameter["LogAddition"] = "%" + strAddition.ToLower() + "%";
            }

            strSql.Append(" and LogDateTime >= ?SLogDateTime");
            dictParameter["SLogDateTime"] = dtmStartTime;

            if (dtmEndTime.HasValue)
            {
                strSql.Append(" and LogDateTime <= ?ELogDateTime");
                dictParameter["ELogDateTime"] = dtmEndTime;
            }

            strSql.Append(" order by LogDateTime desc");

            fbrReturn.ResultData = DBDataSourceHelper.GetOrmCommonData<TCommonlog>(strSql.ToString(), parames: dictParameter);

            return fbrReturn;
        }

        /// <summary>
        /// 功能:写日志到文件      
        /// </summary>
        /// <param name="logLevel">日志级别(不同的级别的日志会存入对应的日志文件)</param>
        /// <param name="logSource">日志的来源</param>
        /// <param name="logValue">需记录的日志值</param>    
        /// <param name="logAddition">日志附加信息</param>
        public static FeedBackResponse<string> WriteFileLog(LogLevel logLevel, string logSource, string logValue, string logAddition)
        {
          var fbrReturn = new FeedBackResponse<string>();

            if (m_enableFileLog)
            {
                SysLogHelper.LogMessage(logSource, logValue, logLevel, WriteLogType.FileLog, logAddition);
            }
            else
            {
                fbrReturn.Message = "记录文件日志开关关闭.";
            }

            return fbrReturn;
        }

        /// <summary>
        /// 功能:写日志到数据库  
        /// </summary>
        /// <param name="logLevel">日志级别(不同的级别的日志会存入对应的日志文件)</param>
        /// <param name="logSource">日志的来源</param>
        /// <param name="logValue">需记录的日志值</param>   
        /// <param name="logAddition">日志附加信息</param>
        public static FeedBackResponse<string> WriteDataBaseLog(LogLevel logLevel, string logSource, string logValue, string logAddition)
        {
            FeedBackResponse<string> fbrReturn = new FeedBackResponse<string>();

            if (m_enableDataBaseLog)
            {
                SysLogHelper.LogMessage(logSource, logValue, logLevel, WriteLogType.DataBaseLog, logAddition);
            }
            else
            {
                fbrReturn.Message = "记录数据日志开关关闭.";
            }

            return fbrReturn;
        }

      
        /// <summary>
        /// 功能:获取日志内容
        /// </summary>
        /// <param name="strLogFilePath">日志文件路径</param>
        /// <returns>日志内容</returns>
        private static string GetLogContent(string strLogFilePath)
        {
            string strReturn = "";

            string strTempLogPath = System.IO.Path.GetDirectoryName(strLogFilePath);
            strTempLogPath = System.IO.Path.Combine(strTempLogPath, "TempLog");

            if (!System.IO.Directory.Exists(strTempLogPath))
            {
                System.IO.Directory.CreateDirectory(strTempLogPath);
            }

            string[] strTempLogFiles = System.IO.Directory.GetFiles(strTempLogPath, "*", System.IO.SearchOption.AllDirectories);
            if (strTempLogFiles.Length > 10)
            {
                System.IO.Directory.Delete(strTempLogPath, true);
                System.IO.Directory.CreateDirectory(strTempLogPath);
            }

            string strTempFilePath = System.IO.Path.Combine(strTempLogPath, GlobalHelper.GetNewGuid() + ".txt");
            System.IO.File.Copy(strLogFilePath, strTempFilePath, true);
            strReturn = FileHelper.ReadFileData(strTempFilePath);

            return strReturn;
        }

        #endregion
    }
}
