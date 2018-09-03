using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.Var
{
    /// <summary>
    /// 功能描述：代付订单 消息队列传送的实体
    /// </summary>
    public class OrderMqMessage
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string FMerchantCode { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string FMerchantOrderNumber { get; set; }
        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal FAmount { get; set; }

        /// <summary>
        /// 会员收款银行卡持卡人
        /// </summary>
        public string FHolders { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string FBankName { get; set; }
        /// <summary>
        /// 会员收款银行卡号
        /// </summary>
        public string FCardNo { get; set; }
        /// <summary>
        /// 商户订单时间
        /// </summary>
        public string FMerchantAddTime { get; set; }

    }
}
