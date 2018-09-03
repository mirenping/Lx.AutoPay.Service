using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Interface
{
    /// <summary>
    /// 功能描述：缓存接口类
    /// </summary>
    public interface ICacheStrategy : IDisposable
    {
        #region  添加缓存  

        /// <summary>
        ///    功能：添加缓存项(服务器上如果Key存在则不添加,否则添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间；如果为空，表示无过期时间</param>
        /// <returns>是否添加成功</returns>
        bool Add(string key, object value, TimeSpan? expiry);

        /// <summary>
        ///     功能：添加缓存项(服务器上如果Key存在则不添加,否则添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间；如果为空，表示无过期时间</param>
        /// <returns>是否添加成功</returns>
        Task<bool> AddAsync(string key, object value, TimeSpan? expiry);

        #endregion

        #region  设置缓存

        /// <summary>
        ///     功能：设置缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间；如果为空表示不设过期时间</param>
        /// <returns>是否设置成功</returns>
        bool Set(string key, object value, TimeSpan? expiry);

        /// <summary>
        ///     功能：设置缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间；如果为空表示不设过期时间</param>
        /// <returns>是否设置成功</returns>
        Task<bool> SetAsync(string key, object value, TimeSpan? expiry);

        #endregion

        #region  移除缓存  

        /// <summary>
        ///     功能：移除所有缓存项
        /// </summary>
        /// <returns>是否成功</returns>
        bool RemoveAll();

        /// <summary>
        ///     功能：移除所有缓存项
        /// </summary>
        /// <returns>是否成功</returns>
        Task<bool> RemoveAllAsync();

        /// <summary>
        ///     功能：移除缓存项(服务器上Key存在就移除)
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <returns>是否移除成功</returns>
        bool Remove(string key);

        /// <summary>
        ///     功能：移除缓存项(服务器上Key存在就移除)
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <returns>是否移除成功</returns>
        Task<bool> RemoveAsync(string key);

        #endregion

        #region   获取缓存  

        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存项的键值</param>
        /// <returns>获取到的缓存项</returns>
        T Get<T>(string key);

        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存项的键值</param>
        /// <returns>获取到的缓存项</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        ///     功能：批量获取缓存
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="keys">缓存键数组</param>
        /// <returns>缓存对像集合</returns>
        List<T> Get<T>(IEnumerable<string> keys);

        /// <summary>
        ///     功能：批量获取缓存
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="keys">缓存键数组</param>
        /// <returns>缓存对像集合</returns>
        Task<List<T>> GetAsync<T>(IEnumerable<string> keys);

        #endregion

        #region   记数计算  

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        Task<bool> InitCounterAsync(string key, long value);

        /// <summary>
        ///     功能：调整计数器
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <param name="value">要调整的值</param>
        /// <returns>获取到的缓存项</returns>
        Task<long> ChangeCounterAsync(string key, long value);

        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        Task<long?> GetCounterAsync(string key);

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        bool InitCounter(string key, long value);

        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">要增加的值</param>
        /// <returns>当前值</returns>
        long ChangeCounter(string key, long value);

        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        long? GetCounter(string key);

        #endregion

        #region double 类型计数

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        Task<bool> InitCounterAsync(string key, double value);

        /// <summary>
        ///     功能：调整计数器
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <param name="value">要调整的值</param>
        /// <returns>获取到的缓存项</returns>
        Task<double> ChangeCounterAsync(string key, double value);

        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        Task<double?> GetDoubleCounterAsync(string key);

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        bool InitCounter(string key, double value);

        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">要增加的值</param>
        /// <returns>当前值</returns>
        double ChangeCounter(string key, double value);



        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        double? GetDoubleCounter(string key);

        #endregion

        #region  判断缓存项是否存在  

        /// <summary>
        ///     功能：判断缓存项是否存在
        /// </summary>
        /// <param name="key">缓存项键值</param>
        /// <returns>缓存项是否存在</returns>
        bool Contains(string key);

        /// <summary>
        ///     功能：判断缓存项是否存在
        /// </summary>
        /// <param name="key">缓存项键值</param>
        /// <returns>缓存项是否存在</returns>
        Task<bool> ContainsAsync(string key);

        #endregion
    }
}
