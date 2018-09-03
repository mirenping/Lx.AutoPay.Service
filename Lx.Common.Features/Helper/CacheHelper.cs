using Lx.Common.Helper;
using Lx.Common.Interface;
using Lx.Common.Models;
using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Features.Helper
{
    /// <summary>
    /// 功能描述：缓存帮助类
    /// </summary>
    public class CacheHelper
    {
        #region  常量定义  

        /// <summary>
        ///     缓存过期时间(分钟)
        /// </summary>
        private static readonly TimeSpan? s_cacheExpiration;

        /// <summary>
        ///     缓存策略实例
        /// </summary>
        private static readonly ICacheStrategy s_cacheStrategy;
        #endregion

        #region   构造函数  
        /// <summary>
        ///     功能：静态构造函数，创建默认的缓存实例
        /// </summary>
        static CacheHelper()
        {
            try
            {
                //获取缓存策略类别
                string strCacheStrategyType = ConfigurationManager.AppSettings["CacheStrategyType"];
                if (!string.IsNullOrWhiteSpace(strCacheStrategyType))
                {
                    //获取缓存过期时间
                    string strCacheExpiration = ConfigurationManager.AppSettings["CacheExpiration"];
                    double value;
                    if (!string.IsNullOrWhiteSpace(strCacheExpiration))
                    {
                        double.TryParse(strCacheExpiration, out value);
                        s_cacheExpiration = TimeSpan.FromMinutes(value);
                    }

                    //创建缓存策略类实例           
                    var redisConfigInfo = JsonConfigInfo.LoadFromFile("cache.json");
                    s_cacheStrategy = ComponentLoader.Load<ICacheStrategy>(redisConfigInfo);
                }
            }
            catch (Exception ex)
            {
                string strMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                SysLogHelper.LogMessage("CacheHelper.Static", strMessage, LogLevel.Error);
            }
        }

        #endregion

        #region  方法定义  

        #region  公共方法  

        /// <summary>
        ///     功能：创建缓存实例
        /// </summary>
        /// <param name="csType">缓存策略类别</param>
        /// <returns>缓存实例</returns>
        public static ICacheStrategy CreateCacheStrategy(CacheStrategyType csType)
        {
            return ObjectFactoryHelper.CreateInstance<ICacheStrategy>(csType + "CacheStrategy", "CacheStrategy", true);
        }

        #endregion

        #endregion

        #region   设置缓存  

        /// <summary>
        ///     功能：设置永久缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <returns>是否设置成功</returns>
        public static bool Set(string key, object value)
        {
            bool blnSuccess = false;
            string strKey = GlobalHelper.GetLowerString(key);
            try
            {
                blnSuccess = s_cacheStrategy.Set(strKey, value, s_cacheExpiration);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.Set", ex.Message, LogLevel.Error, WriteLogType.FileLog);
            }
            return blnSuccess;
        }

        /// <summary>
        ///     功能：设置永久缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <returns>是否设置成功</returns>
        public static bool Add(string key, object value)
        {
            bool blnSuccess = false;
            if (null != s_cacheStrategy)
            {
                string strKey = GlobalHelper.GetLowerString(key);

                try
                {
                    blnSuccess = s_cacheStrategy.Add(strKey, value, s_cacheExpiration);
                }
                catch (Exception ex)
                {
                    SysLogHelper.LogMessage("CacheHelper.Add", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                }
            }

            return blnSuccess;
        }

        /// <summary>
        ///     功能：设置永久缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <returns>是否设置成功</returns>
        public static async Task<bool> AddAsync(string key, object value)
        {
            bool blnSuccess = false;
            if (null != s_cacheStrategy)
            {
                string strKey = GlobalHelper.GetLowerString(key);

                try
                {
                    blnSuccess = await s_cacheStrategy.AddAsync(strKey, value, s_cacheExpiration);
                }
                catch (Exception ex)
                {
                    SysLogHelper.LogMessage("CacheHelper.AddAsync", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                }
            }

            return blnSuccess;
        }

        /// <summary>
        ///     功能：设置缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="numOfMinutes">缓存过期时间(单位:分)</param>
        /// <returns>是否设置成功</returns>
        public static bool Set(string key, object value, double numOfMinutes)
        {
            bool blnSuccess = false;
            if (null != s_cacheStrategy)
            {
                string strKey = GlobalHelper.GetLowerString(key);
                try
                {
                    blnSuccess = s_cacheStrategy.Set(strKey, value, TimeSpan.FromMinutes(numOfMinutes));
                }
                catch (Exception ex)
                {

                    SysLogHelper.LogMessage("CacheHelper.Set_numOfMinutes", ex.Message, LogLevel.Error, WriteLogType.FileLog);

                }
            }

            return blnSuccess;
        }

        /// <summary>
        ///     功能：设置缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="numOfMinutes">缓存过期时间(单位:分)</param>
        /// <returns>是否设置成功</returns>
        public static async Task<bool> SetAsync(string key, object value, double numOfMinutes)
        {
            bool blnSuccess = false;
            if (null != s_cacheStrategy)
            {
                string strKey = GlobalHelper.GetLowerString(key);
                try
                {
                    blnSuccess = await s_cacheStrategy.SetAsync(strKey, value, TimeSpan.FromMinutes(numOfMinutes));
                }
                catch (Exception ex)
                {
                    SysLogHelper.LogMessage("CacheHelper.SetAsync", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                }
            }

            return blnSuccess;
        }

        #endregion

        #region   计数器  

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">初始值</param>
        /// <returns>是否设置成功</returns>
        public static bool InitCounter(string key, long value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return s_cacheStrategy.InitCounter(strKey, value);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.InitCounter", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return false;
            }
        }

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">初始值</param>
        /// <returns>是否设置成功</returns>
        public static bool InitCounter(string key, double value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return s_cacheStrategy.InitCounter(strKey, value);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.InitCounter_double", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return false;
            }
        }

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">初始值</param>
        /// <returns>是否设置成功</returns>
        public static async Task<bool> InitCounterAsync(string key, long value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return await s_cacheStrategy.InitCounterAsync(strKey, value);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.InitCounterAsync", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return false;
            }
        }

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">初始值</param>
        /// <returns>是否设置成功</returns>
        public static async Task<bool> InitCounterAsync(string key, double value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return await s_cacheStrategy.InitCounterAsync(strKey, value);
            }
            catch (Exception ex)
            {

                SysLogHelper.LogMessage("CacheHelper.InitCounterAsync_double", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return false;
            }
        }


        /// <summary>
        ///     功能：改变计数器的值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">改变量</param>
        /// <returns>当前值</returns>
        public static long ChangeCounter(string key, long value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            return s_cacheStrategy.ChangeCounter(strKey, value);
        }


        /// <summary>
        ///     功能：改变计数器的值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">改变量</param>
        /// <returns>当前值</returns>
        public static double ChangeCounter(string key, double value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            return s_cacheStrategy.ChangeCounter(strKey, value);
        }

        /// <summary>
        ///     功能：改变计数器的值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">改变量</param>
        /// <returns>当前值</returns>
        public static async Task<long?> ChangeCounterAsync(string key, long value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return await s_cacheStrategy.ChangeCounterAsync(strKey, value);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.ChangeCounterAsync", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return null;

            }
        }

        /// <summary>
        ///     功能：改变计数器的值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <param name="value">改变量</param>
        /// <returns>当前值</returns>
        public static async Task<double?> ChangeCounterAsync(string key, double value)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return await s_cacheStrategy.ChangeCounterAsync(strKey, value);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.ChangeCounterAsync", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return null;
            }
        }

        /// <summary>
        ///     功能：获取计数器的当前值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <returns>当前值</returns>
        public static long? GetCounter(string key)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return s_cacheStrategy.GetCounter(strKey);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.GetCounter", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return null;
            }
        }

        /// <summary>
        ///     功能：获取计数器的当前值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <returns>当前值</returns>
        public static double? GetDoubleCounter(string key)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return s_cacheStrategy.GetDoubleCounter(strKey);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.GetDoubleCounter", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return null;
            }
        }

        /// <summary>
        ///     功能：获取计数器的当前值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <returns>当前值</returns>
        public static async Task<long?> GetCounterAsync(string key)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return await s_cacheStrategy.GetCounterAsync(strKey);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.GetCounterAsync", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return null;
            }
        }


        /// <summary>
        ///     功能：获取计数器的当前值
        /// </summary>
        /// <param name="key">计数器名称</param>
        /// <returns>当前值</returns>
        public static async Task<double?> GetDoubleCounterAsync(string key)
        {
            string strKey = GlobalHelper.GetLowerString(key);

            try
            {
                return await s_cacheStrategy.GetDoubleCounterAsync(strKey);
            }
            catch (Exception ex)
            {
                SysLogHelper.LogMessage("CacheHelper.GetDoubleCounterAsync", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                return null;
            }
        }

        #endregion

        #region  移除缓存  

        /// <summary>
        ///     功能：移除所有缓存项
        /// </summary>
        /// <returns>是否成功</returns>
        public static bool RemoveAll()
        {
            bool blnSuccess = false;
            if (null != s_cacheStrategy)
            {
                try
                {
                    blnSuccess = s_cacheStrategy.RemoveAll();
                }
                catch (Exception ex)
                {
                    SysLogHelper.LogMessage("CacheHelper.RemoveAll", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                }
            }

            return blnSuccess;
        }

        /// <summary>
        ///     功能：移除缓存项(服务器上Key存在就移除)
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <returns>是否移除成功</returns>
        public static bool Remove(string key)
        {
            bool blnSuccess = false;
            if (null != s_cacheStrategy)
            {
                string strKey = GlobalHelper.GetLowerString(key);
                try
                {

                    var o = s_cacheStrategy.Get<object>(strKey);
                    blnSuccess = (o == null ? true : s_cacheStrategy.Remove(strKey));
                }
                catch (Exception ex)
                {
                    SysLogHelper.LogMessage("CacheHelper.Remove", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                }
            }

            return blnSuccess;
        }
        /// <summary>
        ///     功能：移除缓存项(服务器上Key存在就移除)
        /// </summary>
        /// <param name="keys">缓存项的键值</param>
        /// <returns>是否移除成功</returns>
        public static bool Remove(IEnumerable<string> keys)
        {
            bool success = true;
            foreach (var key in keys)
            {
                success = Remove(key) && success;
            }
            return success;
        }

        #endregion

        #region   获取缓存  
        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存项的键值</param>
        /// <returns>获取到的缓存项</returns>
        public static T Get<T>(string key)
        {
            object obj = default(T);

            if (null != s_cacheStrategy)
            {
                string strKey = GlobalHelper.GetLowerString(key);
                try
                {
                    obj = s_cacheStrategy.Get<T>(strKey);
                }
                catch (Exception ex)
                {

                    SysLogHelper.LogMessage("CacheHelper.Get", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                }
            }

            return (T)obj;
        }

        /// <summary>
        ///     功能：从缓存加载数据，如果没有，则从回调加载数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T LoadFromCache<T>(string cacheKey, Func<T> func)
        {
            var data = Get<T>(cacheKey);
            if (data == null)
            {
                data = func();
                if (data != null)
                {
                    Set(cacheKey, data);
                }
            }
            return data;
        }

        /// <summary>
        ///     功能：批量获取缓存
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="keys">缓存键数组</param>
        /// <returns>缓存对像集合</returns>
        public static List<T> Get<T>(IEnumerable<string> keys)
        {
            var lstReturn = new List<T>();

            if (null != s_cacheStrategy)
            {
                List<string> lstKeys = new List<string>();
                if (null != keys)
                {
                    keys.Any(key =>
                    {
                        string strKey = GlobalHelper.GetLowerString(key);
                        lstKeys.Add(strKey);
                        return false;
                    });
                }

                try
                {
                    lstReturn = s_cacheStrategy.Get<T>(lstKeys);
                }
                catch (Exception ex)
                {
                    SysLogHelper.LogMessage("CacheHelper.Get", ex.Message, LogLevel.Error, WriteLogType.FileLog);
                }
            }

            return lstReturn;
        }

        #endregion
    }
}
