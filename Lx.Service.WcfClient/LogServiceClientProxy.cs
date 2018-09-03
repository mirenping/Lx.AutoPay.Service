	using System;
	using System.Data;
	using System.ServiceModel;
	using System.Threading.Tasks;
	using Lx.Service.WcfClient;
	using Lx.Service.WcfContract;
	using System.Collections.Generic;
	using Lx.Common.Models.Var;
	using Lx.Common.Models.DataBase;
	using Lx.Common.Models.Para;
	
namespace Lx.Service.WcfClient 
{
	/// <summary>
	/// </summary>
	[ServiceContract(Namespace = "Lx.Service.WcfContract", Name = "ILogService")]
	public partial interface ILogService
	{
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		FeedBackResponse<string> TestService(string strParam);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<string>> TestServiceAsync(string strParam);
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		FeedBackResponse<string> TestLog();

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<string>> TestLogAsync();
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		FeedBackResponse<string> GetLogFileContent(string strLogFileName);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<string>> GetLogFileContentAsync(string strLogFileName);
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		FeedBackResponse<List<string>> SearchLogFiles(string strSearchName);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<List<string>>> SearchLogFilesAsync(string strSearchName);
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		FeedBackResponse<List<TCommonlog>> SearchLogData(string strLevel,string strSource,DateTime dtmStartTime,DateTime? dtmEndTime,string strMessage,string strAddition);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<List<TCommonlog>>> SearchLogDataAsync(string strLevel,string strSource,DateTime dtmStartTime,DateTime? dtmEndTime,string strMessage,string strAddition);
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		FeedBackResponse<string> WriteLog(WriteLogType writeLogType,LogLevel logLevel,string logSource,string logValue,string logAddition);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<string>> WriteLogAsync(WriteLogType writeLogType,LogLevel logLevel,string logSource,string logValue,string logAddition);
 
	} 

	/// <summary>
	/// </summary>		
	public partial class LogServiceClient : ClientBase<ILogService>, ILogService
	{ 
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LogServiceClient() :
            base()
        {
        }
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LogServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName )
        {
        }

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FeedBackResponse<string> TestService(string strParam)
		{
            return this.Channel.TestService(strParam);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<string>> TestServiceAsync(string strParam)
		{
            return this.Channel.TestServiceAsync(strParam);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FeedBackResponse<string> TestLog()
		{
            return this.Channel.TestLog();
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<string>> TestLogAsync()
		{
            return this.Channel.TestLogAsync();
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FeedBackResponse<string> GetLogFileContent(string strLogFileName)
		{
            return this.Channel.GetLogFileContent(strLogFileName);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<string>> GetLogFileContentAsync(string strLogFileName)
		{
            return this.Channel.GetLogFileContentAsync(strLogFileName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FeedBackResponse<List<string>> SearchLogFiles(string strSearchName)
		{
            return this.Channel.SearchLogFiles(strSearchName);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<List<string>>> SearchLogFilesAsync(string strSearchName)
		{
            return this.Channel.SearchLogFilesAsync(strSearchName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FeedBackResponse<List<TCommonlog>> SearchLogData(string strLevel,string strSource,DateTime dtmStartTime,DateTime? dtmEndTime,string strMessage,string strAddition)
		{
            return this.Channel.SearchLogData(strLevel,strSource,dtmStartTime,dtmEndTime,strMessage,strAddition);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<List<TCommonlog>>> SearchLogDataAsync(string strLevel,string strSource,DateTime dtmStartTime,DateTime? dtmEndTime,string strMessage,string strAddition)
		{
            return this.Channel.SearchLogDataAsync(strLevel,strSource,dtmStartTime,dtmEndTime,strMessage,strAddition);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FeedBackResponse<string> WriteLog(WriteLogType writeLogType,LogLevel logLevel,string logSource,string logValue,string logAddition)
		{
            return this.Channel.WriteLog(writeLogType,logLevel,logSource,logValue,logAddition);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<string>> WriteLogAsync(WriteLogType writeLogType,LogLevel logLevel,string logSource,string logValue,string logAddition)
		{
            return this.Channel.WriteLogAsync(writeLogType,logLevel,logSource,logValue,logAddition);
		}
	} 

	/// <summary>
	/// </summary>
	public partial class LogServiceClientProxy : ClientProxyBase<LogServiceClient>
	{ 
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static FeedBackResponse<string> TestService(string strParam, string endPontName = "")
		{
            return CallMethod(x=> x.TestService(strParam), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<string>> TestServiceAsync(string strParam, string endPontName = "")
		{
            return CallMethodAsync(x=> x.TestServiceAsync(strParam), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static FeedBackResponse<string> TestLog(string endPontName = "")
		{
            return CallMethod(x=> x.TestLog(), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<string>> TestLogAsync(string endPontName = "")
		{
            return CallMethodAsync(x=> x.TestLogAsync(), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static FeedBackResponse<string> GetLogFileContent(string strLogFileName, string endPontName = "")
		{
            return CallMethod(x=> x.GetLogFileContent(strLogFileName), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<string>> GetLogFileContentAsync(string strLogFileName, string endPontName = "")
		{
            return CallMethodAsync(x=> x.GetLogFileContentAsync(strLogFileName), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static FeedBackResponse<List<string>> SearchLogFiles(string strSearchName, string endPontName = "")
		{
            return CallMethod(x=> x.SearchLogFiles(strSearchName), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<List<string>>> SearchLogFilesAsync(string strSearchName, string endPontName = "")
		{
            return CallMethodAsync(x=> x.SearchLogFilesAsync(strSearchName), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static FeedBackResponse<List<TCommonlog>> SearchLogData(string strLevel,string strSource,DateTime dtmStartTime,DateTime? dtmEndTime,string strMessage,string strAddition, string endPontName = "")
		{
            return CallMethod(x=> x.SearchLogData(strLevel,strSource,dtmStartTime,dtmEndTime,strMessage,strAddition), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<List<TCommonlog>>> SearchLogDataAsync(string strLevel,string strSource,DateTime dtmStartTime,DateTime? dtmEndTime,string strMessage,string strAddition, string endPontName = "")
		{
            return CallMethodAsync(x=> x.SearchLogDataAsync(strLevel,strSource,dtmStartTime,dtmEndTime,strMessage,strAddition), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static FeedBackResponse<string> WriteLog(WriteLogType writeLogType,LogLevel logLevel,string logSource,string logValue,string logAddition, string endPontName = "")
		{
            return CallMethod(x=> x.WriteLog(writeLogType,logLevel,logSource,logValue,logAddition), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<string>> WriteLogAsync(WriteLogType writeLogType,LogLevel logLevel,string logSource,string logValue,string logAddition, string endPontName = "")
		{
            return CallMethodAsync(x=> x.WriteLogAsync(writeLogType,logLevel,logSource,logValue,logAddition), endPontName);
		}
	} 
}
