using Lx.Common.Interface;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Implement.CacheStrategy
{
    /// <summary>
    ///     功能：Redis 缓存实现
    /// </summary>
    public class RedisCacheStrategy : ICacheStrategy, IDisposable, IComponent
    {
        #region 变量定义

        /// <summary>
        ///     Redis 连接选择器实例
        /// </summary>
        private IConnectionMultiplexer m_multiplexer;

        #endregion

        #region  属性定义

        /// <summary>
        ///     是否启用管理模式。如果启用管理模式，就可以使用 RemoveAll 这样的高级参数
        /// </summary>
        public bool AllowAdmin { get; set; }

        /// <summary>
        ///     Redis 服务器列表
        /// </summary>
        public RedisHostEntry[] Hosts { get; set; }

        #endregion

        #region 方法定义  

        /// <summary>
        ///     功能：添加缓存项(服务器上如果Key存在则不添加,否则添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间；如果为空，表示无过期时间</param>
        /// <returns>是否添加成功</returns>
        public bool Add(string key, object value, TimeSpan? expiry)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            try
            {
                var database = m_multiplexer.GetDatabase();

                var valueAsString = JsonConvert.SerializeObject(value);

                return database.StringSet(key, valueAsString, expiry, When.NotExists);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     功能：添加缓存项(服务器上如果Key存在则不添加,否则添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间；如果为空，表示无过期时间</param>
        /// <returns>是否添加成功</returns>
        public async Task<bool> AddAsync(string key, object value, TimeSpan? expiry)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            try
            {
                var database = m_multiplexer.GetDatabase();


                var valueAsString = JsonConvert.SerializeObject(value);

                return await database.StringSetAsync(key, valueAsString, expiry, When.NotExists);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     功能：设置缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间(单位:分钟)；如果为空表示不设过期时间</param>
        /// <returns>是否设置成功</returns>
        public bool Set(string key, object value, TimeSpan? expiry)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");
            var database = m_multiplexer.GetDatabase();
            var valueAsString = JsonConvert.SerializeObject(value);
            return database.StringSet(key, valueAsString, expiry, When.Always);
        }

        /// <summary>
        ///     功能：设置缓存项(服务器上key存在就替换,不存在就添加)
        /// </summary>
        /// <param name="key">缓存键值</param>
        /// <param name="value">缓存值</param>
        /// <param name="expiry">缓存过期时间(单位:分钟)；如果为空表示不设过期时间</param>
        /// <returns>是否设置成功</returns>
        public Task<bool> SetAsync(string key, object value, TimeSpan? expiry)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");
            var database = m_multiplexer.GetDatabase();
            var valueAsString = JsonConvert.SerializeObject(value);
            return database.StringSetAsync(key, valueAsString, expiry, When.Always);
        }

        /// <summary>
        ///     功能：移除所有缓存项
        /// </summary>
        /// <returns>是否成功</returns>
        public bool RemoveAll()
        {
            foreach (var endPoint in m_multiplexer.GetEndPoints())
            {
                var server = m_multiplexer.GetServer(endPoint);
                if (!server.IsSlave)
                {
                    server.FlushAllDatabases();
                }
            }
            return true;
        }

        /// <summary>
        ///     功能：移除所有缓存项
        /// </summary>
        /// <returns>是否成功</returns>
        public async Task<bool> RemoveAllAsync()
        {
            foreach (var endPoint in m_multiplexer.GetEndPoints())
            {
                var server = m_multiplexer.GetServer(endPoint);
                if (!server.IsSlave)
                {
                    await server.FlushAllDatabasesAsync();
                }
            }
            return true;
        }

        /// <summary>
        ///     功能：移除缓存项(服务器上Key存在就移除)
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <returns>是否移除成功</returns>
        public bool Remove(string key)
        {
            var database = m_multiplexer.GetDatabase();

            return database.KeyDelete(key);
        }

        /// <summary>
        ///     功能：移除缓存项(服务器上Key存在就移除)
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <returns>是否移除成功</returns>
        public async Task<bool> RemoveAsync(string key)
        {
            var database = m_multiplexer.GetDatabase();

            return await database.KeyDeleteAsync(key);
        }

        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存项的键值</param>
        /// <returns>获取到的缓存项</returns>
        public T Get<T>(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            var value = database.StringGet(key);
            if (value.IsNull)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <typeparam name="T">缓存项的类型</typeparam>
        /// <param name="key">缓存项的键值</param>
        /// <returns>获取到的缓存项</returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var database = m_multiplexer.GetDatabase();
            var value = await database.StringGetAsync(key);
            if (value.IsNull)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        ///     功能：批量获取缓存
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="keys">缓存键数组</param>
        /// <returns>缓存对像集合</returns>
        public List<T> Get<T>(IEnumerable<string> keys)
        {
            if (keys == null) throw new ArgumentNullException("keys");

            var database = m_multiplexer.GetDatabase();

            // 我们可以使用批量操作 mget, 但是在负载情况下，这种操作是不被支持的
            List<T> items = new List<T>();
            foreach (var key in keys)
            {
                var value = database.StringGet(key);
                if (!value.IsNull)
                {
                    items.Add(JsonConvert.DeserializeObject<T>(value));
                }
            }
            return items;
        }

        /// <summary>
        ///     功能：批量获取缓存
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="keys">缓存键数组</param>
        /// <returns>缓存对像集合</returns>
        public async Task<List<T>> GetAsync<T>(IEnumerable<string> keys)
        {
            if (keys == null) throw new ArgumentNullException("keys");

            var database = m_multiplexer.GetDatabase();

            // 我们可以使用批量操作 mget, 但是在负载情况下，这种操作是不被支持的
            List<T> items = new List<T>();
            foreach (var key in keys)
            {
                var k = key;
                var value = await database.StringGetAsync(k);
                if (!value.IsNull)
                {
                    items.Add(JsonConvert.DeserializeObject<T>(value));
                }
            }
            return items;
        }

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        public bool InitCounter(string key, long value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return database.StringSet(key, value, null, When.NotExists);
        }

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        public bool InitCounter(string key, double value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return database.StringSet(key, value, null);
        }


        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        public async Task<bool> InitCounterAsync(string key, long value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return await database.StringSetAsync(key, value, null, When.NotExists);
        }

        /// <summary>
        ///     功能：初始化计数器
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">初始值</param>
        /// <returns>获取到的缓存项</returns>
        public async Task<bool> InitCounterAsync(string key, double value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return await database.StringSetAsync(key, value, null, When.NotExists);
        }

        /// <summary>
        ///     功能：调整计数器
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <param name="value">要调整的值</param>
        /// <returns>获取到的缓存项</returns>
        public long ChangeCounter(string key, long value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return database.StringIncrement(key, value);
        }

        /// <summary>
        ///     功能：获取缓存项的值
        /// </summary>
        /// <param name="key">缓存项的键值</param>
        /// <param name="value">要增加的值</param>
        /// <returns>当前值</returns>
        public double ChangeCounter(string key, double value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return database.StringIncrement(key, value);
        }

        /// <summary>
        ///     功能：调整计数器
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <param name="value">要调整的值</param>
        /// <returns>获取到的缓存项</returns>
        public async Task<long> ChangeCounterAsync(string key, long value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return await database.StringIncrementAsync(key, value);
        }

        /// <summary>
        ///     功能：调整计数器
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <param name="value">要调整的值</param>
        /// <returns>获取到的缓存项</returns>
        public async Task<double> ChangeCounterAsync(string key, double value)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return await database.StringIncrementAsync(key, value);
        }

        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        public long? GetCounter(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            var redisValue = database.StringGet(key);
            return redisValue.IsNull ? (long?)null : (long)redisValue;
        }

        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        public double? GetDoubleCounter(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            var redisValue = database.StringGet(key);
            return redisValue.IsNull ? (double?)null : (double)redisValue;
        }

        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        public async Task<long?> GetCounterAsync(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            var redisValue = await database.StringGetAsync(key);
            return redisValue.IsNull ? (long?)null : (long)redisValue;
        }

        /// <summary>
        ///     功能：获取计数器值
        /// </summary>
        /// <param name="key">计数器的键值</param>
        /// <returns>计数器当前值</returns>
        public async Task<double?> GetDoubleCounterAsync(string key)
        {

            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            var redisValue = await database.StringGetAsync(key);
            return redisValue.IsNull ? (double?)null : (double)redisValue;
        }

        /// <summary>
        ///     功能：判断缓存项是否存在
        /// </summary>
        /// <param name="key">缓存项键值</param>
        /// <returns>缓存项是否存在</returns>
        public bool Contains(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return database.KeyExists(key);
        }

        /// <summary>
        ///     功能：判断缓存项是否存在
        /// </summary>
        /// <param name="key">缓存项键值</param>
        /// <returns>缓存项是否存在</returns>
        public async Task<bool> ContainsAsync(string key)
        {
            if (key == null) throw new ArgumentNullException("key");

            var database = m_multiplexer.GetDatabase();
            return await database.KeyExistsAsync(key);
        }

        /// <summary>
        ///     功能：执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            var disposingObject = m_multiplexer;
            m_multiplexer = null;
            using (disposingObject as IDisposable)
            {
            }
        }
        /// <summary>
        ///     功能：初始化组件
        /// </summary>
        void IComponent.Init()
        {
            var hosts = Hosts ?? new RedisHostEntry[0];
            var configurationOptions = new ConfigurationOptions
            {
                AllowAdmin = AllowAdmin,
                Password= "mirenping"
            };

            foreach (var host in hosts)
            {
                configurationOptions.EndPoints.Add(host.ToEntPoint());
            }
            m_multiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        }

        #endregion

    }
}
