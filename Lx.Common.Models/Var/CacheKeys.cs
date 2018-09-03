using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.Var
{
    /// <summary>
    ///   功能：缓存 Key 定义
    /// </summary>
    public static class CacheKeys
    {
        #region  常量定义  

        /// <summary>
        ///     所有缓存的前缀
        /// </summary>
        private const string Prefix = "LxPay:";

        /// <summary>
        ///     消息配置缓存
        /// </summary>
        private const string LxPay_Message = Prefix + "LxPay_Message";

        /// <summary>
        ///     登录用户缓存建
        /// </summary>
        private const string LxPay_UserLogin = Prefix + "LxPay_UserLogin";


        #endregion

        #region   方法定义  

        #region 配置类缓存

        /// <summary>
        ///     功能：获取 BdMessage 的缓存 Key
        /// </summary>
        /// <param name="msgCode">消息代码</param>
        /// <returns></returns>
        public static string MakeBdMessageKey(string msgCode)
        {
            return LxPay_Message + ":" + msgCode;
        }

        #endregion

        #region 用户缓存
        /// <summary>
        ///     功能：登录用户缓存，在修改用户信息 请删除此节点
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public static string MakeUserLoginKey(long userId)
        {
            return string.Concat(LxPay_UserLogin, ":", userId);
        }
        #endregion

        #endregion
    }
}
