using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Implement.CacheStrategy
{
    /// <summary>
    /// 功能： Redis 主机配置节点
    /// </summary>
    public class RedisHostEntry
    {
        #region 属性定义  

        /// <summary>
        ///     主机送，可使用 IP 或者域名
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     端口
        /// </summary>
        public int Port { get; set; }

        #endregion

        #region  构造函数  

        /// <summary>
        ///     功能：构建函数
        /// </summary>
        public RedisHostEntry()
        {
            Port = 6379;
        }

        #endregion

        #region  方法定义  

        /// <summary>
        ///     功能：转换为 EndPoint
        /// </summary>
        /// <returns></returns>
        internal EndPoint ToEntPoint()
        {
            return new DnsEndPoint(Host, Port);
        }

        #endregion
    }
}
