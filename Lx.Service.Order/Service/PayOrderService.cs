using Lx.Common.Features.Helper;
using Lx.Common.Helper;
using Lx.Common.Models.DataBase;
using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.Order.Service
{
    /// <summary>
    /// 代付订单处理
    /// </summary>
    public class PayOrderService
    {
        public static void AddPayOrder(OrderMqMessage item)
        {
            var orderModel = new TAutoTransfer
            {
                FAmount = item.FAmount,
                FBankName = item.FBankName,
                FCardNo = item.FCardNo,
                FHolders = item.FHolders,
                FMerchantAddTime = Convert.ToDateTime(item.FMerchantAddTime),
                FMerchantCode = item.FMerchantCode,
                FMerchantOrderNumber = item.FMerchantOrderNumber,
                FOrderNumber = CreateNoHelper.CreatPayOrderNo(),
                FAddTime = DateTime.Now,
                FStatus = 0
            };
            try
            {
                DBDataSourceHelper.AddOrmData(orderModel);
            }
            catch (Exception ex)
            {
            
                throw new Exception("PayOrderService.AddPayOrder--消息队列执行出错" + ex.Message);
            }


        }
    }
}
