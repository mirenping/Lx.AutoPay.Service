using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Lx.Common.Models.Var;
using Lx.Service.WcfHelper;
using Lx.Common.Models.DataBase;
using Lx.Common.Models.Para;

namespace Lx.Service.WcfService
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                 Namespace = "Lx.Service.WcfService",
                 Name = "OrderService")]
    public class OrderService : IOrderService
    {
        /// <summary>
        /// 功能描述：测试服务
        /// </summary>
        /// <param name="strParam"></param>
        /// <returns></returns>
        public FeedBackResponse<string> TestService(string strParam)
        {
            strParam = strParam ?? "";
            return new FeedBackResponse<string>() { ResultData = strParam + "OrderService is ok!" };
        }

        /// <summary>
        /// 功能描述:代付下单
        /// </summary>
        /// <param name="orderItme">订单消息队列实体</param>
        /// <returns></returns>
        public FeedBackResponse<bool> PayOrderAdd(OrderMqMessage orderItme)
        {
            return OrderServiceHelper.PayOrderAdd(orderItme);
        }


        /// <summary>
        /// 功能描述:获得代付订单列表
        /// </summary>
        /// <param name="merchantCode">商户号</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="status">订单状态</param>
        /// <returns></returns>
        public FeedBackResponse<IList<TAutoTransfer>> GetTautoTransferInfoByMerchantCode(string merchantCode, PageInfoParam pageInfo, int? status=null)
        {
            return OrderServiceHelper.GetTautoTransferInfoByMerchantCode(merchantCode, pageInfo, status);
        }
    }
}
