using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.Var
{

    #region 接口实现类型

    /// <summary>
    /// 功能:数据源类型
    /// </summary>
    public enum DBDataSourceType
    {
        /// <summary>
        /// Mysql数据源
        /// </summary>
        MySql,

        /// <summary>
        /// Mssql数据源
        /// </summary>
        MsSql
    }



    /// <summary>
    /// 功能:缓存策略类型
    /// </summary>
    public enum CacheStrategyType
    {
        /// <summary>
        /// 本地缓存
        /// </summary>
        Local,

        /// <summary>
        /// Redis缓存
        /// </summary>
        Redis
    }

    #endregion


    /// <summary>
    /// 功能:用于日志的消息级别类型
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     所有
        /// </summary>
        All = 1,

        /// <summary>
        ///     错误消息日志
        /// </summary>
        Error = 2,

        /// <summary>
        ///     常规信息日志
        /// </summary>
        Information = 3
    }

    /// <summary>
    /// 功能:用于日志的写日志类型
    /// </summary>
    public enum WriteLogType
    {
        /// <summary>
        ///     写日志到文件日志，数据库
        /// </summary>
        AllLog = 1,

        /// <summary>
        ///     写日志到文件日志
        /// </summary>
        FileLog = 2,

        /// <summary>
        ///     写日志到数据库日志
        /// </summary>
        DataBaseLog = 3,
        /// <summary>
        ///     写日志到文件和数据库日志
        /// </summary>
        FileDataBaseLog = 4
    }



    #region 消息队列
    /// <summary>
    /// 队列名称
    /// </summary>
    public enum QueueName
    {
        /// <summary>
        /// 日志队列
        /// </summary>
        LxLogQueue,
        /// <summary>
        /// 代付队列
        /// </summary>
        AutoPayQueue,
        /// <summary>
        /// 缓存相关队列
        /// </summary>
        CacheQueue


    }
    #endregion

}
