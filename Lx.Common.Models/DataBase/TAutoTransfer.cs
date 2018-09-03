using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.DataBase
{
    /// <summary>
    /// 自动转账表
    /// </summary>
    [Serializable]
    [BsonIgnoreExtraElements]
    public class TAutoTransfer
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
        /// 商户订单时间
        /// </summary>
        public DateTime FMerchantAddTime { get; set; }
        /// <summary>
        /// 提现金额
        /// </summary>
        public decimal FAmount { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int FStatus { get; set; }
        /// <summary>
        /// 银行卡持卡人
        /// </summary>
        public string FHolders { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string FBankName { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string FCardNo { get; set; }

        /// <summary>
        /// 平台订单号
        /// </summary>
        public string FOrderNumber { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public int FOperatorId { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime FAddTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? FUpdateTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string FRemark { get; set; }
    }
}
