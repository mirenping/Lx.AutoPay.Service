using Lx.Common.Features.Helper;
using Lx.Common.Models.DataBase;
using Lx.Common.Models.Para;
using Lx.Common.Models.Var;
using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfHelper
{
    /// <summary>
    /// 代付订单处理类
    /// </summary>
    public class OrderServiceHelper
    {
        /// <summary>
        /// 功能描述:代付下单
        /// </summary>
        /// <param name="orderItme">订单消息队列实体</param>
        /// <returns></returns>
        public static FeedBackResponse<bool> PayOrderAdd(OrderMqMessage orderItme)
        {
            return PayOrderMqHelper.SendPayOrderMessageToMq(orderItme);
        }

        /// <summary>
        /// 功能描述:获取代付订单列表
        /// </summary>
        /// <param name="merchantCode">商户号</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="status">订单状态</param>
        /// <returns></returns>
        public static FeedBackResponse<IList<TAutoTransfer>> GetTautoTransferInfoByMerchantCode(string merchantCode, PageInfoParam pageInfo, int? status=null)
        {
            var fbrReturn = new FeedBackResponse<IList<TAutoTransfer>>();
            var strSqlBase = new StringBuilder(200);
            var strSqlCout = new StringBuilder(200);
            //参数字典
            var parames = new Dictionary<string, object>();
            strSqlBase.Append("SELECT FId,FMerchantCode,FMerchantOrderNumber,FMerchantAddTime,FAmount,FStatus,FHolders,FBankName,FCardNo,FOrderNumber,FAddTime from tautotransfer where FMerchantCode=@FMerchantCode");
            strSqlCout.Append("select count(1) from tautotransfer where FMerchantCode=@FMerchantCode");
            parames.Add("FMerchantCode", merchantCode);
            if(null!= status)
            {
                strSqlBase.Append(" and FStatus=@FStatus");
                strSqlCout.Append(" and FStatus=@FStatus");
                parames.Add("FStatus",status.Value);
            }
            var sql = new StringBuilder(DBDataSourceHelper.GetPageSql(strSqlBase.ToString(), pageInfo));
            sql.Append(";");
            sql.Append(strSqlCout);
            var data = DBDataSourceHelper.GetOrmPageData<TAutoTransfer>(sql.ToString(), parames: parames);
            fbrReturn.ResultData = data.Item1;
            fbrReturn.TotalCount = data.Item2;
            return fbrReturn;
        }
    }
}
