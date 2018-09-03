using Lx.Common.Models.DataBase;
using Lx.Common.Models.Para;
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
    /// 功能描述:订单服务
    /// </summary>
    [ServiceContract(Namespace = "Lx.Service.WcfContract",
                     Name = "IOrderService")]
    public interface IOrderService
    {
        #region  测试  
        /// <summary>
        /// 功能描述:测试服务
        /// </summary>
        /// <param name="strParam">监控参数</param>
        /// <returns>返回结果</returns>
        [OperationContract()]
        FeedBackResponse<string> TestService(string strParam);
        #endregion


        #region 方法
        /// <summary>
        /// 功能描述:代付下单
        /// </summary>
        /// <param name="orderItme">订单消息队列实体</param>
        /// <returns></returns>
        [OperationContract()]
        FeedBackResponse<bool> PayOrderAdd(OrderMqMessage orderItme);

        /// <summary>
        /// 功能描述:获得代付订单列表
        /// </summary>
        /// <param name="merchantCode">商户号</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="status">订单状态</param>
        /// <returns></returns>
        [OperationContract()]
        FeedBackResponse<IList<TAutoTransfer>> GetTautoTransferInfoByMerchantCode(string merchantCode, PageInfoParam pageInfo,int? status=null);

        #endregion


    }
}
