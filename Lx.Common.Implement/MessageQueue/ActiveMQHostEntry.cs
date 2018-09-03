using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Implement.MessageQueue
{
    /// <summary>
    /// 功能： MQ 主机配置节点
    /// </summary>
    public class ActiveMQHostEntry
    {
        #region   属性定义  

        /// <summary>
        ///     连接URL
        /// </summary>
        /// <example>
        ///     tcp://activemqhost:61616
        /// </example>
        public string Uri { get; set; }

        /// <summary>
        ///     brocker 用户名, 不填用户名，则不使用用户名密码进行验证
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     brocker 密码
        /// </summary>
        public string Password { get; set; }

        #endregion
    }
}
