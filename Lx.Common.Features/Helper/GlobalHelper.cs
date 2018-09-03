using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Features.Helper
{
    /// <summary>
    /// 功能描述:系统公共帮助类
    /// </summary>
    public class GlobalHelper
    {
        /// <summary>
        /// 功能:获取一个转换类型后的值
        /// </summary>
        /// <typeparam name="T">转换的类型</typeparam>
        /// <param name="value">待转换类型的值</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>value的泛型值,如果不value不是T类型的,就返回默认值</returns>
        public static T GetValue<T>(object value, T defaultValue)
        {
            if (value is T)
            {
                return (T)value;
            }
            return defaultValue;
        }
        #region  ConvertType  

        /// <summary>
        ///     功能：将字符串转换为特定类型，现在只支持原生类型以及数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertType<T>(string value)
        {
            return (T)ConvertType(value, typeof(T));
        }

        private static object ConvertType(string value, Type targetType)
        {
            if (targetType.IsArray)
            {
                var items = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var array = Array.CreateInstance(targetType.GetElementType(), items.Length);
                for (int i = 0; i < items.Length; i++)
                {
                    var item = items[i];
                    array.SetValue(ConvertType(item, targetType.GetElementType()), i);
                }
                return array;
            }
            return Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
        }

        #endregion


        /// <summary>
        /// 功能: 获取小写字符串
        /// </summary>
        /// <param name="strData">原始字符数据</param>
        /// <returns>小写字符串</returns>
        public static string GetLowerString(string strData)
        {
            if (string.IsNullOrWhiteSpace(strData))
            {
                return "";
            }
            return strData.Trim().ToLower();
        }

        /// <summary>
        /// 功能:将字符串转换成等效的类型
        /// </summary>
        /// <param name="strValue">待转换的文本</param>
        /// <param name="strDefaultValue">默认值</param>
        /// <returns>转换后类型</returns>
        /// <returns></returns>
        public static T ParseStringData<T>(string strValue, string strDefaultValue = "")
        {
            if (string.IsNullOrWhiteSpace(strValue))
            {
                if (string.IsNullOrWhiteSpace(strDefaultValue))
                {
                    return default(T);
                }
                else
                {
                    strValue = strDefaultValue;
                }
            }

            object objValue = ConvertType(strValue, typeof(T));
            return (T)objValue;
        }

        /// <summary>
        ///   功能：通过消息代码获取消息正文
        /// </summary>
        /// <param name="msgCode">消息代码</param>
        /// <param name="formatArgs">格式化参数</param>
        /// <returns></returns>
        public static string GetBdMessageValue(string msgCode, params object[] formatArgs)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(msgCode))
                {
                    return "数据错误,请重试!(空msgCode)";
                }
                else
                {
                    string cacheKey = CacheKeys.MakeBdMessageKey(msgCode);
                    string strMegInfo = CacheHelper.Get<string>(cacheKey);
                    if (string.IsNullOrWhiteSpace(strMegInfo))
                    {
                        strMegInfo = DBDataSourceHelper.ExecuteReaderReturnT<string>("select MsgInfo from TMessage where MsgCode = @MsgCode", new { MsgCode = msgCode });
                        if (!string.IsNullOrWhiteSpace(strMegInfo))
                        {
                            CacheHelper.Set(cacheKey, strMegInfo);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(strMegInfo))
                    {
                        strMegInfo = "数据错误,请重试!(未配置)";
                    }
                    else
                    {
                        if (null != formatArgs && formatArgs.Length > 0)
                        {
                            strMegInfo = string.Format(strMegInfo, formatArgs);
                        }
                    }
                    return strMegInfo;
                }
            }
            catch (Exception ex)
            {
              
                return "数据错误,请重试!";
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 功能:获取新的Guid
        /// </summary>
        /// <param name="blnUseFormat">是否需要格式化值</param>
        /// <returns>新的Guid</returns>
        public static string GetNewGuid(bool blnUseFormat = true)
        {
            if (blnUseFormat)
            {
                return Guid.NewGuid().ToString("N");
            }
            return Guid.NewGuid().ToString();
        }



        /// <summary>
        /// 功能:获取机器的名称和IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetMachineName()
        {
            //返回值
            string strReturn = "";
            try
            {
                //获取计算机名称
                strReturn = Dns.GetHostName();
            }
            catch (Exception)
            {
                strReturn = "Error";
            }

            return strReturn;
        }
    }
}
