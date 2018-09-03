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
	[ServiceContract(Namespace = "Lx.Service.WcfContract", Name = "IUserService")]
	public partial interface IUserService
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
		FeedBackResponse<TAccounts> GetUserData(long userId);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<TAccounts>> GetUserDataAsync(long userId);
 
	} 

	/// <summary>
	/// </summary>		
	public partial class UserServiceClient : ClientBase<IUserService>, IUserService
	{ 
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UserServiceClient() :
            base()
        {
        }
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public UserServiceClient(string endpointConfigurationName) :
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
		public FeedBackResponse<TAccounts> GetUserData(long userId)
		{
            return this.Channel.GetUserData(userId);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<TAccounts>> GetUserDataAsync(long userId)
		{
            return this.Channel.GetUserDataAsync(userId);
		}
	} 

	/// <summary>
	/// </summary>
	public partial class UserServiceClientProxy : ClientProxyBase<UserServiceClient>
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
		public static FeedBackResponse<TAccounts> GetUserData(long userId, string endPontName = "")
		{
            return CallMethod(x=> x.GetUserData(userId), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<TAccounts>> GetUserDataAsync(long userId, string endPontName = "")
		{
            return CallMethodAsync(x=> x.GetUserDataAsync(userId), endPontName);
		}
	} 
}
