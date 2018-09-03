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
	[ServiceContract(Namespace = "Lx.Service.WcfContract", Name = "IOrderService")]
	public partial interface IOrderService
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
		FeedBackResponse<bool> PayOrderAdd(OrderMqMessage orderItme);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<bool>> PayOrderAddAsync(OrderMqMessage orderItme);
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		FeedBackResponse<IList<TAutoTransfer>> GetTautoTransferInfoByMerchantCode(string merchantCode,PageInfoParam pageInfo,int? status);

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
		Task<FeedBackResponse<IList<TAutoTransfer>>> GetTautoTransferInfoByMerchantCodeAsync(string merchantCode,PageInfoParam pageInfo,int? status);
 
	} 

	/// <summary>
	/// </summary>		
	public partial class OrderServiceClient : ClientBase<IOrderService>, IOrderService
	{ 
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OrderServiceClient() :
            base()
        {
        }
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OrderServiceClient(string endpointConfigurationName) :
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
		public FeedBackResponse<bool> PayOrderAdd(OrderMqMessage orderItme)
		{
            return this.Channel.PayOrderAdd(orderItme);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<bool>> PayOrderAddAsync(OrderMqMessage orderItme)
		{
            return this.Channel.PayOrderAddAsync(orderItme);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public FeedBackResponse<IList<TAutoTransfer>> GetTautoTransferInfoByMerchantCode(string merchantCode,PageInfoParam pageInfo,int? status)
		{
            return this.Channel.GetTautoTransferInfoByMerchantCode(merchantCode,pageInfo,status);
		}

		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public Task<FeedBackResponse<IList<TAutoTransfer>>> GetTautoTransferInfoByMerchantCodeAsync(string merchantCode,PageInfoParam pageInfo,int? status)
		{
            return this.Channel.GetTautoTransferInfoByMerchantCodeAsync(merchantCode,pageInfo,status);
		}
	} 

	/// <summary>
	/// </summary>
	public partial class OrderServiceClientProxy : ClientProxyBase<OrderServiceClient>
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
		public static FeedBackResponse<bool> PayOrderAdd(OrderMqMessage orderItme, string endPontName = "")
		{
            return CallMethod(x=> x.PayOrderAdd(orderItme), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<bool>> PayOrderAddAsync(OrderMqMessage orderItme, string endPontName = "")
		{
            return CallMethodAsync(x=> x.PayOrderAddAsync(orderItme), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static FeedBackResponse<IList<TAutoTransfer>> GetTautoTransferInfoByMerchantCode(string merchantCode,PageInfoParam pageInfo,int? status, string endPontName = "")
		{
            return CallMethod(x=> x.GetTautoTransferInfoByMerchantCode(merchantCode,pageInfo,status), endPontName);
		}
		/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static Task<FeedBackResponse<IList<TAutoTransfer>>> GetTautoTransferInfoByMerchantCodeAsync(string merchantCode,PageInfoParam pageInfo,int? status, string endPontName = "")
		{
            return CallMethodAsync(x=> x.GetTautoTransferInfoByMerchantCodeAsync(merchantCode,pageInfo,status), endPontName);
		}
	} 
}
