using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.Common
{
    /// <summary>
    /// 功能描述:数据消息异常类  
    /// </summary>
    [Serializable]
    public class DataMessageException : Exception
    {
        #region  构造函数  

        public DataMessageException(string msgCode)
        {
            this.MsgCode = msgCode;
        }

        public DataMessageException(string msgCode, params object[] formatArgs)
        {
            this.MsgCode = msgCode;
            this.FormatArgs = formatArgs;
        }

        #endregion

        #region   属性定义  

        /// <summary>
        /// 消息代码
        /// </summary>
        public string MsgCode
        {
            get;
            private set;
        }

        /// <summary>
        /// 消息参数
        /// </summary>
        public object[] FormatArgs
        {
            get;
            private set;
        }

        #endregion
    }
}
