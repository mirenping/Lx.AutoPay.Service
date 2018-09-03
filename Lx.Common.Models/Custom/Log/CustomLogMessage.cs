using Lx.Common.Models.Var;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.Log
{
    /// <summary>
    /// 功能描述:系统日志模型
    /// </summary>
    [Serializable]
    [BsonIgnoreExtraElements]
    public class CustomLogMessage
    {
        #region   属性定义  

        public string LogSource
        {
            get;
            set;
        }

        public string LogMessage
        {
            get;
            set;
        }

        public string LogAddition
        {
            get;
            set;
        }

        public LogLevel LogLevel
        {
            get;
            set;
        }

        public WriteLogType WriteLogType
        {
            get;
            set;
        }

      
        #endregion
    }
}
