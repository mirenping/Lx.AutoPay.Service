using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.DataBase
{
    [Serializable]
    [BsonIgnoreExtraElements]
   public class TMessage
    {
       

        /// <summary>
        /// 自增长
        /// </summary>
        public int FID { get; set; }

        /// <summary>
        /// 消息Code,唯一
        /// </summary>
		public string FMsgCode { get; set; }

        /// <summary>
        /// 消息信息
        /// </summary>
		public string FMsgInfo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
		public string FRemark { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
		public string FCreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
		public DateTime FCreateDT { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
		public string FUpdateBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
		public DateTime FUpdateDT { get; set; }

      
    }
}
