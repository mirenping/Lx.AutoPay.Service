using DapperExtensions;
using Lx.Common.Helper;
using Lx.Common.Interface;
using Lx.Common.Models.Para;
using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Features.Helper
{
    /// <summary>
    /// DB数据源帮助类  
    /// </summary>
    public class DBDataSourceHelper
    {
        #region  常量定义  

        //数据源
        private static readonly IDBDataSource s_dbDataSource;

        #endregion

        #region   属性定义  

        /// <summary>
        ///     数据库的连接字符串
        /// </summary>
        public static string WriteConnectionString
        {
            get { return s_dbDataSource.WriteConnectionString; }
        }

        /// <summary>
        ///     数据库的连接字符串
        /// </summary>
        public static string ReadConnectionString
        {
            get { return s_dbDataSource.ReadConnectionString; }
        }

        /// <summary>
        ///     公共数据库的连接字符串
        /// </summary>
        public string CommonConnectionString
        {
            get { return s_dbDataSource.CommonConnectionString; }
        }

        #endregion

        #region  构造函数 

        static DBDataSourceHelper()
        {
            try
            {
                //获取数据源类型
                string strDBType = ConfigurationManager.AppSettings["DBType"];
                if (string.IsNullOrWhiteSpace(strDBType))
                {
                    strDBType = "MySql";
                }
           
                s_dbDataSource = ObjectFactoryHelper.CreateInstance<IDBDataSource>(strDBType + "DBDataSource", "DBDataSource", true);
            }
            catch (Exception ex)
            {
                string strMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                SysLogHelper.LogMessage("DataSourceHelper.Static", strMessage, LogLevel.Error);
            }
        }

        #endregion

        #region 公共方法  

        /// <summary>
        /// 功能描述:执行整体事物性事物
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="funcTask">任务委托</param>
        /// <returns>返回值</returns>
        public static TResult ExecuteTranscationTask<TResult>(Func<IDbTransaction, TResult> funcTask)
        {
            //返回值            
            TResult returnData = default(TResult);

            //数据库事物
            IDbTransaction tran = null;

            try
            {
                tran = s_dbDataSource.BeginTransaction();
                returnData = funcTask(tran);
                tran.Commit();
            }
            catch (Exception)
            {
                if (null != tran)
                {
                    tran.Rollback();
                }
                throw;
            }
            finally
            {
                if (null != tran)
                {
                    tran.Connection.Close();
                }
            }

            return returnData;
        }

        #endregion

        #region  Ado方法  

        /// <summary>
        /// 功能:获取总数Sql语句
        /// </summary>
        /// <param name="strSql">读取数据sql</param>       
        /// <returns>总数Sql</returns>
        public static string GetCountSql(string strSql)
        {
            var sbReturn = new StringBuilder();
            //添加总数语句
            sbReturn.Append("select count(1) from (");
            //取数据语句          
            sbReturn.Append(strSql);
            sbReturn.Append(") as tempData");

            return sbReturn.ToString();
        }

        /// <summary>
        /// 功能:创建数据源实例
        /// </summary>
        /// <param name="dbType">数据源类型</param>
        /// <returns>数据源实例</returns>
        public static IDBDataSource CreateDataSource(DBDataSourceType dbType)
        {
            return ObjectFactoryHelper.CreateInstance<IDBDataSource>(dbType + "DBDataSource", "DBDataSource", true);
        }

        /// <summary>
        /// 功能: 从数据库中查询数据
        /// </summary>       
        /// <param name="cmdText">查询命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdParameters">命令参数</param>
        /// <returns>返回值</returns>
        public static DataSet QueryData(string cmdText,
                                        Dictionary<string, object> cmdParameters = null,
                                        CommandType cmdType = CommandType.Text)
        {
            return s_dbDataSource.QueryData(cmdText, cmdParameters, cmdType);
        }

        /// <summary>
        /// 功能:从数据库中查询数据
        /// </summary>       
        /// <param name="cmdText">查询命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdParameters">命令参数</param>
        /// <param name="outPutValue">输出参数值</param>
        /// <returns>返回值</returns>
        public static DataSet QueryData(string cmdText, out string outPutValue,
                                        Dictionary<string, object> cmdParameters = null,
                                        CommandType cmdType = CommandType.Text)
        {
            return s_dbDataSource.QueryData(cmdText, out outPutValue, cmdParameters, cmdType);
        }

        /// <summary>
        /// 功能:获取分页Sql语句
        /// </summary>
        /// <param name="strSql">读取数据sql</param>
        /// <param name="intPageNo">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="isAscending">是否升序</param>   
        /// <returns>分页Sql</returns>
        public static string GetPageSql(string strSql, int intPageNo,
                                        int intPageSize, string sortField, bool isAscending)
        {
            string strOrderBy = sortField + " " + (isAscending ? "asc " : "desc ");
            return s_dbDataSource.GetPageSql(strSql, intPageNo, intPageSize, strOrderBy);
        }
        /// <summary>
        /// 功能:获取分页Sql语句
        /// </summary>
        /// <param name="strSql">读取数据sql</param>
        /// <param name="intPageNo">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="sortField">排序字段，自行在排序字段中指定是升序还是降序</param>
        /// <returns>分页Sql</returns>
        public static string GetPageSql(string strSql, int intPageNo,
                                        int intPageSize, string sortField)
        {
            return s_dbDataSource.GetPageSql(strSql, intPageNo, intPageSize, sortField);
        }

        /// <summary>
        /// 功能:获取分页Sql语句
        /// </summary>
        /// <param name="strSql">读取数据sql</param>
        /// <param name="pageInfoParam">分页对象</param>
        /// <returns>分页Sql</returns>
        public static string GetPageSql(string strSql, PageInfoParam pageInfoParam)
        {
            string strOrderBy = "";
            if (null == pageInfoParam.SortInfo || pageInfoParam.SortInfo.Count == 0)
            {
                strOrderBy = pageInfoParam.SortField + " " + (pageInfoParam.SortDirection ? "asc " : "desc ");
            }
            else
            {
                int intCount = 0;
                StringBuilder sbTemp = new StringBuilder();
                pageInfoParam.SortInfo.Any(si =>
                {
                    if (intCount > 0)
                    {
                        sbTemp.Append(",");
                    }

                    sbTemp.Append(si.Key);
                    sbTemp.Append(" ");
                    sbTemp.Append(si.Value ? "asc " : "desc ");

                    intCount++;
                    return false;
                });

                strOrderBy = sbTemp.ToString();
            }

            return s_dbDataSource.GetPageSql(strSql, pageInfoParam.PageIndex, pageInfoParam.PageSize, strOrderBy);
        }

        /// <summary>
        /// 功能:获取分页Sql语句
        /// </summary>
        /// <param name="strSql">读取数据sql</param>
        /// <param name="pageInfoParam">分页对象</param>
        /// <returns>分页Sql</returns>
        public static string GetPageSql<T>(string strSql, PageInfoParam<T> pageInfoParam)
        {
            return s_dbDataSource.GetPageSql(strSql, pageInfoParam.PageIndex, pageInfoParam.PageSize, pageInfoParam.SortField + " " + (pageInfoParam.SortDirection ? "asc " : "desc"));
        }

        /// <summary>
        /// 功能:获取数据库单值
        /// </summary>       
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdParameters">命令参数</param>  
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns>返回值</returns>
        public static T ExecuteScalar<T>(string cmdText,
                                              Dictionary<string, object> cmdParameters = null,
                                              CommandType cmdType = CommandType.Text, bool useWriteConn = false)
        {
            object objTemp = s_dbDataSource.ExecuteScalar(cmdText, cmdParameters, cmdType, useWriteConn);
            return GlobalHelper.GetValue(objTemp, default(T));
        }

        /// <summary>
        /// 功能:执行命令语句
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdParameters">命令参数</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText,
                                          Dictionary<string, object> cmdParameters = null,
                                          CommandType cmdType = CommandType.Text, bool useTran = false)
        {
            return s_dbDataSource.ExecuteNonQuery(cmdText, cmdParameters, cmdType, useTran);
        }

        /// <summary>
        /// 功能: 执行命令语句
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="outPutValue">输出参数值</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdParameters">命令参数</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>受影响的行数</returns>
        public static int ExecuteNonQuery(string cmdText, out string outPutValue,
                                          Dictionary<string, object> cmdParameters = null,
                                          CommandType cmdType = CommandType.Text, bool useTran = false)
        {
            return s_dbDataSource.ExecuteNonQuery(cmdText, out outPutValue, cmdParameters, cmdType, useTran);
        }

        #endregion

        #region Orm方法 

        /// <summary>
        /// 功能:返回一条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数,执行条件查询时要参数化</param>
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns></returns>
        public static T ExecuteReaderReturnT<T>(string sql, dynamic param = null, bool useWriteConn = false)
        {
            return s_dbDataSource.ExecuteReaderReturnT<T>(sql, param as object, useWriteConn);
        }

        /// <summary>
        /// 功能:写入操作，并返回一条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数</param>
        /// <returns></returns>
        public static T ExecuteWriteReturnT<T>(string sql, dynamic param = null, bool isTran = false)
        {
            return s_dbDataSource.ExecuteWriteReturnT<T>(sql, param as object, isTran);
        }

        /// <summary>
        /// 功能:返回一条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数,执行条件查询时要参数化</param>
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns></returns>
        public static Task<T> ExecuteReaderReturnTAsync<T>(string sql, dynamic param = null, bool useWriteConn = false)
        {
            return s_dbDataSource.ExecuteReaderReturnTAsync<T>(sql, param as object, useWriteConn);
        }

        /// <summary>
        /// 功能:返回多条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数,执行条件查询时要参数化</param>
        /// <returns></returns>
        public static List<T> ExecuteReaderReturnListT<T>(string sql, dynamic param = null)
        {
            return s_dbDataSource.ExecuteReaderReturnListT<T>(sql, param as object);
        }

        /// <summary>
        /// 功能:返回多条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数,执行条件查询时要参数化</param>
        /// <returns></returns>
        public static Task<List<T>> ExecuteReaderReturnListTAsync<T>(string sql, dynamic param = null)
        {
            return s_dbDataSource.ExecuteReaderReturnListTAsync<T>(sql, param as object);
        }

        /// <summary>
        /// 功能:执行sql，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="dbTrans">事务处理</param>
        /// <returns></returns>
        public static int ExecuteUpdate(string sql, dynamic param = null, IDbTransaction dbTrans = null)
        {
            return s_dbDataSource.ExecuteUpdate(sql, param as object, dbTrans);
        }

        /// <summary>
        /// 功能:执行sql，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Task<int> ExecuteUpdateAsync(string sql, dynamic param = null)
        {
            return s_dbDataSource.ExecuteUpdateAsync(sql, param as object);
        }

        /// <summary>
        /// 功能:添加Orm数据
        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>添加结果</returns>
        public static object AddOrmData<T>(T objData, bool useTran = false) where T : class
        {
            return s_dbDataSource.AddOrmData(objData, useTran);
        }
        /// </summary>
        /// 功能:添加Orm数据
        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>添加结果</returns>
        public static object AddOrmTranData<T>(T objData, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.AddOrmData(objData, dbTrans);
        }

        /// <summary>
        /// 功能:批量添加Orm数据
        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>添加结果</returns>
        public static object AddOrmData<T>(List<T> objData, bool useTran = false) where T : class
        {
            return s_dbDataSource.AddOrmData(objData, useTran);
        }

        /// <summary>
        /// 功能:批量添加Orm数据
        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>添加结果</returns>
        public static object AddOrmTranData<T>(List<T> objData, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.AddOrmData(objData, dbTrans);
        }

        /// <summary>
        /// 功能:更新Orm数据
        /// </summary>
        /// <typeparam name="T">待更新的类型</typeparam>
        /// <param name="objData">待更新数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>更新结果</returns>
        public static bool UpdateOrmData<T>(T objData, bool useTran = false) where T : class
        {
            return s_dbDataSource.UpdateOrmData(objData, useTran);
        }

        /// <summary>
        /// 功能:删除Orm数据
        /// </summary>
        /// <typeparam name="T">待删除的类型</typeparam>
        /// <param name="objData">待删除数据</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>删除结果</returns>
        public static bool UpdateOrmTranData<T>(T objData, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.UpdateOrmData(objData, dbTrans);
        }

        /// <summary>
        /// 功能:删除Orm数据
        /// </summary>
        /// <typeparam name="T">待删除的类型</typeparam>
        /// <param name="objData">待删除数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>删除结果</returns>
        public static bool DeleteOrmData<T>(T objData, bool useTran = false) where T : class
        {
            return s_dbDataSource.DeleteOrmData(objData, useTran);
        }

        /// <summary>
        /// 功能:删除Orm数据
        /// </summary>
        /// <typeparam name="T">待删除的类型</typeparam>
        /// <param name="objData">待删除数据</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>删除结果</returns>
        public static bool DeleteOrmTranData<T>(T objData, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.DeleteOrmData(objData, dbTrans);
        }

        /// <summary>
        ///     功能：获取数据库单值,
        ///     功能：获取数据库单值,同 ExecuteScalar, 只是参数不一样
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="parames">命令参数</param>
        /// <param name="dbTrans">事物对象</param>
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns>返回值</returns>
        public static T ExecuteOrmScalar<T>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null, bool useWriteConn = false)
        {
            object objTemp = s_dbDataSource.ExecuteOrmScalar(cmdText, parames, cmdType, dbTrans, useWriteConn);
            if (objTemp == null || Convert.IsDBNull(objTemp))
            {
                return default(T);
            }
            return GlobalHelper.ConvertType<T>(objTemp.ToString());
        }

        /// <summary>
        ///     功能：异步获取数据库单值,同 ExecuteScalar, 只是参数不一样
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="parames">命令参数</param>
        /// <param name="dbTrans">事物对象</param>
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns>返回值</returns>
        public static async Task<object> ExecuteOrmScalarAsync(string cmdText, object parames = null, CommandType cmdType = CommandType.Text, IDbTransaction dbTrans = null, bool useWriteConn = false)
        {
            return await s_dbDataSource.ExecuteOrmScalarAsync(cmdText, parames, cmdType, dbTrans, useWriteConn);
        }

        /// <summary>
        /// 功能:执行sql，返回影响行数
        /// </summary>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>             
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">命令执行事物</param>
        /// <returns>返回影响行数</returns>
        public static int ExecuteOrmData(string cmdText, object parames = null, CommandType cmdType = CommandType.Text, IDbTransaction dbTrans = null)
        {
            return s_dbDataSource.ExecuteOrmData(cmdText, parames, cmdType, dbTrans);
        }

        /// <summary>
        /// 功能:异步执行sql，返回影响行数
        /// </summary>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>             
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">命令执行事物</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>返回影响行数</returns>
        public static async Task<int> ExecuteOrmDataAsync(string cmdText, object parames = null, CommandType cmdType = CommandType.Text, IDbTransaction dbTrans = null)
        {
            return await s_dbDataSource.ExecuteOrmDataAsync(cmdText, parames, cmdType, dbTrans);
        }

        /// <summary>
        /// 功能:获取单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>    
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns>主键数据</returns>
        public static T GetSingleOrmData<T>(object predicate, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            return s_dbDataSource.GetSingleOrmData<T>(predicate, dbTrans, useWriteConn);
        }

        /// <summary>
        /// 功能:获取单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="expression">主键字段表达式</param>
        /// <param name="id">主键值</param>
        /// <param name="dbTrans">事物对象</param>
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns></returns>
        public static T GetSingleOrmData<T>(Expression<Func<T, object>> expression, object id, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            var predicate = Predicates.Field<T>(expression, Operator.Eq, id);
            return s_dbDataSource.GetSingleOrmData<T>(predicate, dbTrans, useWriteConn);
        }

        /// <summary>
        /// 功能:获取单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns>列表数据</returns>
        public static T GetSingleOrmData<T>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            return s_dbDataSource.GetSingleOrmData<T>(cmdText, cmdType, parames, dbTrans, useWriteConn);
        }

        /// <summary>
        /// 功能:获取单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>             
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <param name="useWriteConn">是否使用写数据库连接</param>
        /// <returns>列表数据</returns>
        public static async Task<T> GetSingleOrmDataAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            return await s_dbDataSource.GetSingleOrmDataAsync<T>(cmdText, cmdType, parames, dbTrans, useWriteConn);
        }

        /// <summary>
        /// 功能:获取列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static List<T> GetOrmData<T>(object predicate, string sortField, bool isAscending, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmData<T>(predicate, sortField, isAscending, dbTrans);
        }

        /// <summary>
        /// 功能:获取列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>
        /// <param name="sortInfo">排序信息(多字段排序)</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static List<T> GetOrmData<T>(object predicate, Dictionary<string, bool> sortInfo, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmData<T>(predicate, sortInfo, dbTrans);
        }

        /// <summary>
        /// 功能:获取列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static List<T> GetOrmData<T>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmData<T>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static Task<List<T>> GetOrmDataAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmDataAsync<T>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(包括分页语句和求总数语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static Tuple<List<T1>, long> GetOrmPageData<T1>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            return s_dbDataSource.GetOrmPageData<T1>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:异步获取分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(包括分页语句和求总数语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static async Task<Tuple<List<T1>, long>> GetOrmPageDataAsync<T1>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            return await s_dbDataSource.GetOrmPageDataAsync<T1>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(原始语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static Tuple<List<T1>, long> GetOrmPageData<T1>(string cmdText, PageInfoParam pageInfo, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(GetPageSql(cmdText, pageInfo).ToString());
            sbSql.Append(";");
            sbSql.Append(GetCountSql(cmdText).ToString());
            return s_dbDataSource.GetOrmPageData<T1>(sbSql.ToString(), cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:异步获取分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(原始语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static async Task<Tuple<List<T1>, long>> GetOrmPageDataAsync<T1>(string cmdText, PageInfoParam pageInfo, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(GetPageSql(cmdText, pageInfo).ToString());
            sbSql.Append(";");
            sbSql.Append(GetCountSql(cmdText).ToString());
            return await s_dbDataSource.GetOrmPageDataAsync<T1>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取多项数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>
        /// <typeparam name="T2">数据类型2</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static Tuple<List<T1>, List<T2>> GetOrmMultipleData<T1, T2>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            return s_dbDataSource.GetOrmMultipleData<T1, T2>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取多项数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>
        /// <typeparam name="T2">数据类型2</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static async Task<Tuple<List<T1>, List<T2>>> GetOrmMultipleDataAsync<T1, T2>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            return await s_dbDataSource.GetOrmMultipleDataAsync<T1, T2>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取多项数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>
        /// <typeparam name="T2">数据类型2</typeparam>
        /// <typeparam name="T3">数据类型2</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static Tuple<List<T1>, List<T2>, List<T3>> GetOrmMultipleData<T1, T2, T3>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            return s_dbDataSource.GetOrmMultipleData<T1, T2, T3>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取多项数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>
        /// <typeparam name="T2">数据类型2</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public static async Task<Tuple<List<T1>, List<T2>, List<T3>>> GetOrmMultipleDataAsync<T1, T2, T3>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null, IDbTransaction dbTrans = null)
        {
            return await s_dbDataSource.GetOrmMultipleDataAsync<T1, T2, T3>(cmdText, cmdType, parames, dbTrans);
        }

        /// <summary>
        /// 功能:获取分页数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>
        /// <param name="sortInfo">排序信息(多字段排序)</param>        
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>   
        /// <param name="dbTrans">事物对象</param>
        /// <returns>分页数据</returns>
        public static List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, Dictionary<string, bool> sortInfo, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmPageData<T>(predicate, pageIndex, pageSize, sortInfo, dbTrans);
        }

        /// <summary>
        /// 功能:获取分页数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>       
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>  
        /// <param name="sortField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>分页数据</returns>
        public static List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, string sortField, bool isAscending, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmPageData<T>(predicate, pageIndex, pageSize, sortField, isAscending, dbTrans);
        }

        /// <summary>
        /// 功能:获取分页数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>
        /// <param name="sortInfo">排序信息(多字段排序)</param>        
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>   
        /// <param name="totalCount">数据总数</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>分页数据</returns>
        public static List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, Dictionary<string, bool> sortInfo, out long totalCount, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmPageData<T>(predicate, pageIndex, pageSize, sortInfo, out totalCount, dbTrans);
        }

        /// <summary>
        /// 功能:获取分页数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>       
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>  
        /// <param name="sortField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="totalCount">数据总数</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>分页数据</returns>
        public static List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, string sortField, bool isAscending, out long totalCount, IDbTransaction dbTrans = null) where T : class
        {
            return s_dbDataSource.GetOrmPageData<T>(predicate, pageIndex, pageSize, sortField, isAscending, out totalCount, dbTrans);
        }

        /// <summary>
        /// 功能:获取公共库单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="expression">主键字段表达式</param>
        /// <param name="id">主键值</param>      
        /// <returns></returns>
        public static T GetSingleOrmCommonData<T>(Expression<Func<T, object>> expression, object id) where T : class
        {
            var predicate = Predicates.Field<T>(expression, Operator.Eq, id);
            return s_dbDataSource.GetSingleOrmCommonData<T>(predicate);
        }

        /// <summary>
        /// 功能:获取公共库列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>       
        /// <returns>列表数据</returns>
        public static List<T> GetOrmCommonData<T>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null) where T : class
        {
            return s_dbDataSource.GetOrmCommonData<T>(cmdText, cmdType, parames);
        }

        /// <summary>
        /// 功能:获取公共库分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(包括分页语句和求总数语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>       
        /// <returns>列表数据</returns>
        public static Tuple<List<T1>, long> GetOrmPageCommonData<T1>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null)
        {
            return s_dbDataSource.GetOrmPageCommonData<T1>(cmdText, cmdType, parames);
        }

        /// <summary>
        /// 功能:异步获取公共数据库分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(包括分页语句和求总数语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>        
        /// <returns>列表数据</returns>
        public static async Task<Tuple<List<T1>, long>> GetOrmPageCommonDataAsync<T1>(string cmdText, CommandType cmdType = CommandType.Text, object parames = null)
        {
            return await s_dbDataSource.GetOrmPageCommonDataAsync<T1>(cmdText, cmdType, parames);
        }

        #endregion
    }
}
