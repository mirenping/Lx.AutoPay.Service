using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Helper
{
    /// <summary>
    /// 功能描述：唯一编号生成器 生成订单编号
    /// </summary>
    public class CreateNoHelper
    {
        /// <summary>
		/// 功能描述：平台代单号生成
		/// </summary>
		/// <returns></returns>
		public static string CreatPayOrderNo()
        {
            return "LXP" + GetRandomNumberKeyNo();
        }
        /// <summary>
        /// 功能描述：生成唯一的编号
        /// </summary>
        /// <returns></returns>
        private static string GetRandomNumberKeyNo()
        {
            return string.Concat(DateTime.Now.ToString("yyMMddHHmm"), GetUniqueCode(7));
        }
        /// <summary>
        /// 功能:获取Guid的唯一HashCode值
        /// </summary>
        /// <param name="intDigit"></param>
        /// <returns></returns>
        public static string GetUniqueCode(int intDigit)
        {

            string strReturn = Math.Abs(Guid.NewGuid().ToString().GetHashCode()).ToString();
            if (strReturn.Length < intDigit)
            {
                return strReturn.PadRight(intDigit, '0');
            }
            else
            {
                return strReturn.Substring(0, intDigit);
            }
        }
    }
}
