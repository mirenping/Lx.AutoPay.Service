using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfContract
{
    /// <summary>
    /// 功能描述:数据服务操作反馈信息
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [DataContract(Namespace = "Lx.Service.WcfContract",
                 Name = "FeedBackResponse")]
    public class FeedBackResponse<TData> : FeedBackResponseBase
    {
        #region  构造函数  
        /// <summary>
        /// 
        /// </summary>
        public FeedBackResponse()
        {
            ResultData = default(TData);
        }

        #endregion

        #region  属性定义 

        /// <summary>
        /// 结果数据
        /// </summary>
        [DataMember()]
        public TData ResultData
        {
            get;
            set;
        }

        #endregion

    }
    /// <summary>
    /// 数据服务操作反馈信息
    /// </summary>
    [DataContract(Namespace = "Lx.Service.WcfContract",
                  Name = "FeedBackResponse")]
    public class FeedBackResponseBase
    {
        #region  构造函数  

        public FeedBackResponseBase()
        {
            Result = true;
            Message = "";
            MsgCode = "0";
            TotalCount = 0;
            ErrorMessage = "";
        }

        #endregion

        #region   属性定义  

        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember()]
        public bool Result
        {
            get;
            set;
        }

        /// <summary>
        /// 反馈信息
        /// </summary>
        [DataMember()]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 反馈信息代码
        /// </summary>
        [DataMember()]
        public string MsgCode
        {
            get;
            set;
        }

        /// <summary>
        /// 分页查询时的总数
        /// </summary>
        [DataMember()]
        public long TotalCount
        {
            get;
            set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember()]
        public string ErrorMessage
        {
            get;
            set;
        }

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:设置服务端程序错误信息
        /// </summary>
        /// <param name="ex">错误信息</param>       
        public void SetServiceErrorMsg(Exception ex)
        {
            Result = false;
            MsgCode = "Sys001";
            Message = "网络繁忙,请重试!";
            ErrorMessage = "se:" + ex.Message;
        }

        /// <summary>
        /// 功能:设置客户端程序错误信息
        /// </summary>
        /// <param name="ex">错误信息</param>       
        public void SetClientErrorMsg(Exception ex)
        {
            Result = false;
            MsgCode = "Sys002";
            Message = "网络繁忙，请重试!";
            ErrorMessage = "ce:" + ex.Message;
        }

        /// <summary>
        /// 设置数据错误反馈信息代码
        /// </summary>
        /// <param name="strCode">代码</param>
        /// <param name="strMessage">反馈信息</param>
        public void SetMsgCode(string strCode, string strMessage = "")
        {
            Result = false;
            MsgCode = strCode;
            Message = strMessage;
        }

        /// <summary>
        /// 功能:设置成功反馈信息代码
        /// </summary>
        /// <param name="strCode">代码</param>
        /// <param name="strMessage">反馈信息</param>
        public void SetSuccessMsg(string strCode, string strMessage)
        {
            if (string.IsNullOrWhiteSpace(strMessage))
            {
                strMessage = "操作成功!";
            }
            MsgCode = strCode;
            Message = strMessage;
        }

        #endregion
    }
}
