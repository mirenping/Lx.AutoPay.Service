using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.DataBase
{
    /// <summary>
    /// 系统通用日志表
    /// </summary>
    [Serializable]
    [BsonIgnoreExtraElements]
    public class TCommonlog
    {
        #region   构造函数  

        public TCommonlog()
        {

            this.LogDateTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        }

        #endregion

        #region  属性定义  

        /// <summary>
        /// ID
        /// </summary>
        public long LogID { get; set; }

        /// <summary>
        /// 日志级别
        /// </summary>
		public string LogLevel { get; set; }

        /// <summary>
        /// 日志来源
        /// </summary>
		public string LogSource { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
		public string LogMessage { get; set; }

        /// <summary>
        /// 其它信息
        /// </summary>
		public string LogAddition { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
		public DateTime LogDateTime { get; set; }

        #endregion
    }
}
