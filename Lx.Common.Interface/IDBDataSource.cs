using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Interface
{
    /// <summary>
    /// 数据库数据源接口
    /// </summary>
    public abstract class IDBDataSource
    {
        #region   变量定义 

        //命令执行超时时间
        private int m_excuteTimeout = 30;

        //读数据库连接串
        private string m_readConnectionString;

        //写数据库的连接字符串
        private string m_writeConnectionString;

        //公共数据库的连接字符串
        private string m_commonConnectionString;

        //读数据库提供程序
        private readonly DbProviderFactory m_readProviderFactory;

        //写数据库提供程序工厂
        private readonly DbProviderFactory m_writeProviderFactory;

        //公共数据库提供程序工厂
        private readonly DbProviderFactory m_commonProviderFactory;

        #endregion

        #region   属性定义  

        /// <summary>
        ///     是否系统默认数据源
        /// </summary>
        public bool IsDefault
        {
            get;
            set;
        }

        /// <summary>
        ///     命令执行超时时间
        /// </summary>
        public int ExcuteTimeout
        {
            get { return m_excuteTimeout; }

            set { m_excuteTimeout = value; }
        }

        /// <summary>
        ///     写入数据库的连接字符串
        /// </summary>
        public string WriteConnectionString
        {
            get { return m_writeConnectionString; }

            set { m_writeConnectionString = value; }
        }

        /// <summary>
        ///     读取数据库的连接字符串
        /// </summary>
        public string ReadConnectionString
        {
            get { return m_readConnectionString; }

            set { m_readConnectionString = value; }
        }

        /// <summary>
        ///     公共数据库的连接字符串
        /// </summary>
        public string CommonConnectionString
        {
            get { return m_commonConnectionString; }

            set { m_commonConnectionString = value; }
        }

        #endregion

        #region  构造函数  

        public IDBDataSource()
        {
            var writeConnectionSetting = ConfigurationManager.ConnectionStrings["WriteConnection"];
            var readConnectionSetting = ConfigurationManager.ConnectionStrings["ReadConnection"];
            var commonConnectionSetting = ConfigurationManager.ConnectionStrings["CommonConnection"];

            if (null != writeConnectionSetting)
            {
                m_writeConnectionString = writeConnectionSetting.ConnectionString;
                m_writeProviderFactory = DbProviderFactories.GetFactory(writeConnectionSetting.ProviderName);
            }

            if (null != readConnectionSetting)
            {
                m_readConnectionString = readConnectionSetting.ConnectionString;
                m_readProviderFactory = DbProviderFactories.GetFactory(readConnectionSetting.ProviderName);
            }

            if (null != commonConnectionSetting)
            {
                m_commonConnectionString = commonConnectionSetting.ConnectionString;
                m_commonProviderFactory = DbProviderFactories.GetFactory(commonConnectionSetting.ProviderName);
            }
        }

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:获取数据库当前时间函数名称
        /// </summary>
        /// <returns>数据库当前时间函数名称</returns>
        public abstract string GetDBCurrentTime();


        /// <summary>
        /// 功能描述:开启一个整体事物 
        /// </summary>
        /// <param name="tranLevel">事物级别</param>
        /// <returns>事物对象</returns>
        public IDbTransaction BeginTransaction(IsolationLevel tranLevel = IsolationLevel.RepeatableRead)
        {
            IDbConnection conn = CreateConnection();
            return conn.BeginTransaction();
        }

        #endregion

        #region Ado方法  

        /// <summary>
        /// 功能:测试连接
        /// </summary>
        /// <returns>连接是否成功</returns>
        public abstract bool TestConnection();

        /// <summary>
        /// 功能:获取字段左引用符号
        /// </summary>
        /// <returns>字段左引用符号</returns>
        public abstract string GetLeftColumnSymbol();

        /// <summary>
        /// 功能:获取字段右引用符号
        /// </summary>
        /// <returns>字段右引用符号</returns>
        public abstract string GetRightColumnSymbol();

        /// <summary>
        /// 功能:获取参数符号
        /// </summary>
        public abstract string GetParameterSymbol();

        /// <summary>
        /// 功能:统一数据库脚本
        /// </summary>
        /// <param name="strCmdText">sql脚本</param>
        /// <returns>统一后的脚本</returns>
        public string UniformCommandText(string strCmdText)
        {
            if (!IsDefault)
            {
                strCmdText = strCmdText.Replace("?", GetParameterSymbol());
                strCmdText = strCmdText.Replace("`", GetLeftColumnSymbol());
                strCmdText = strCmdText.Replace("`", GetRightColumnSymbol());
                strCmdText = strCmdText.Replace("now()", GetDBCurrentTime());
            }

            return strCmdText;
        }


        /// <summary>
        /// 功能:获取分页Sql语句
        /// </summary>
        /// <param name="strSql">读取数据sql</param>
        /// <param name="intPageNo">页码</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strSortBy">排序信息</param>        
        /// <returns>分页Sql</returns>
        public abstract string GetPageSql(string strSql, int intPageNo,
                                          int intPageSize, string strSortBy);

        /// <summary>
        /// 功能:从数据库中查询数据
        /// </summary>       
        /// <param name="cmdText">查询命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>返回值</returns>
        public abstract DataSet QueryData(string cmdText,
                                          Dictionary<string, object> commandParameters = null,
                                          CommandType cmdType = CommandType.Text);

        /// <summary>
        /// 功能:从数据库中查询数据
        /// </summary>       
        /// <param name="cmdText">查询命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="outPutValue">输出参数值</param>
        /// <returns>返回值</returns>
        public abstract DataSet QueryData(string cmdText, out string outPutValue,
                                          Dictionary<string, object> commandParameters = null,
                                          CommandType cmdType = CommandType.Text);

        /// <summary>
        /// 功能:保存数据到数据库(包括新增、修改、删除)
        /// </summary>       
        /// <param name="cmdText">保存命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="lstParameters">命令参数列表</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>受影响行数</returns>
        public abstract int SaveData(string cmdText,
                                      List<Dictionary<string, object>> lstParameters,
                                      CommandType cmdType = CommandType.Text, bool useTran = false);

        /// <summary>
        /// 功能:获取数据库单值
        /// </summary>       
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>返回值</returns>
        public abstract object ExecuteScalar(string cmdText,
                                             Dictionary<string, object> commandParameters = null,
                                             CommandType cmdType = CommandType.Text, bool useWriteConn = false);

        /// <summary>
        /// 功能:执行命令语句
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>受影响的行数</returns>
        public abstract int ExecuteNonQuery(string cmdText,
                                            Dictionary<string, object> commandParameters = null,
                                            CommandType cmdType = CommandType.Text, bool useTran = false);

        /// <summary>
        /// 功能:执行命令语句
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="useTran">是否使用事物</param>
        /// <param name="outPutValue">输出参数值</param>
        /// <returns>受影响的行数</returns>
        public abstract int ExecuteNonQuery(string cmdText, out string outPutValue,
                                           Dictionary<string, object> commandParameters = null,
                                           CommandType cmdType = CommandType.Text, bool useTran = false);

        #endregion

        #region  Orm方法

        /// <summary>
        /// 功能:返回一条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数,执行条件查询时要参数化</param>
        /// <returns></returns>
        public T ExecuteReaderReturnT<T>(string sql, object param, bool useWriteConn = false)
        {
            using (IDbConnection conn = CreateConnection(useWriteConn))
            {
                return conn.Query<T>(sql, param, commandTimeout: m_excuteTimeout).SingleOrDefault();
            }
        }

        /// <summary>
        /// 功能:写入操作，并返回一条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数</param>
        /// <returns></returns>
        public T ExecuteWriteReturnT<T>(string sql, object param, bool isTran = false)
        {
            using (IDbConnection conn = CreateConnection(true))
            {
                if (isTran)
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var result = conn.Query<T>(sql, param, commandTimeout: m_excuteTimeout, transaction: tran).SingleOrDefault();
                        tran.Commit();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
                return conn.Query<T>(sql, param, commandTimeout: m_excuteTimeout).SingleOrDefault();
            }
        }

        /// <summary>
        /// 功能:返回一条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">sql参数,执行条件查询时要参数化</param>
        /// <returns></returns>
        public async Task<T> ExecuteReaderReturnTAsync<T>(string sql, object param, bool useWriteConn = false)
        {
            using (IDbConnection conn = CreateConnection(useWriteConn))
            {
                return (await conn.QueryAsync<T>(sql, param, commandTimeout: m_excuteTimeout).ConfigureAwait(false)).SingleOrDefault();
            }
        }

        /// <summary>
        /// 功能:返回多条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数,执行条件查询时要参数化</param>
        /// <returns></returns>
        public List<T> ExecuteReaderReturnListT<T>(string sql, object param)
        {
            using (IDbConnection conn = CreateConnection(false))
            {
                return conn.Query<T>(sql, param, commandTimeout: m_excuteTimeout).ToList();
            }
        }

        /// <summary>
        /// 功能:返回多条记录
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数,执行条件查询时要参数化</param>
        /// <returns></returns>
        public async Task<List<T>> ExecuteReaderReturnListTAsync<T>(string sql, object param)
        {
            using (IDbConnection conn = CreateConnection(false))
            {
                return (await conn.QueryAsync<T>(sql, param, commandTimeout: m_excuteTimeout).ConfigureAwait(false)).ToList();
            }
        }

        /// <summary>
        /// 功能:执行sql，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="dbTrans"></param>
        /// <returns></returns>
        public int ExecuteUpdate(string sql, object param, IDbTransaction dbTrans = null)
        {
            //返回值            
            int returnData = 0;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = conn.Execute(sql, param, dbTrans, m_excuteTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;

        }

        /// <summary>
        /// 功能:执行sql，返回影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> ExecuteUpdateAsync(string sql, object param)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return await conn.ExecuteAsync(sql, param, commandTimeout: m_excuteTimeout).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 功能:添加Orm数据
        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>添加结果</returns>
        public object AddOrmData<T>(T objData, bool useTran = false) where T : class
        {
            //返回值            
            object returnData = 0;

            //数据库连接
            IDbConnection conn = null;

            //数据库事物
            IDbTransaction tran = null;

            try
            {
                //获取数据库连接
                conn = CreateConnection();

                //创建事物
                if (useTran)
                {
                    //数据库事物
                    tran = conn.BeginTransaction();
                    returnData = conn.Insert(objData, tran, m_excuteTimeout);
                    tran.Commit();
                }
                else
                {
                    returnData = conn.Insert(objData, null, m_excuteTimeout);
                }
            }
            catch (Exception)
            {
                if (useTran)
                {
                    tran.Rollback();
                }
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }


        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>添加结果</returns>
        public object AddOrmData<T>(T objData, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            object returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = conn.Insert(objData, dbTrans, m_excuteTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:批量添加Orm数据
        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>添加结果</returns>
        public object AddOrmData<T>(List<T> objData, bool useTran = false) where T : class
        {
            //返回值            
            object returnData = 0;

            //数据库连接
            IDbConnection conn = null;

            //数据库事物
            IDbTransaction tran = null;

            try
            {
                //获取数据库连接
                conn = CreateConnection();

                //创建事物
                if (useTran)
                {
                    //数据库事物
                    tran = conn.BeginTransaction();
                    conn.Insert<T>(objData, tran, m_excuteTimeout);
                    tran.Commit();
                }
                else
                {
                    conn.Insert<T>(objData, null, m_excuteTimeout);
                }
                returnData = objData.Count;
            }
            catch (Exception)
            {
                if (useTran)
                {
                    tran.Rollback();
                }
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:批量添加Orm数据
        /// </summary>
        /// <typeparam name="T">待添加的类型</typeparam>
        /// <param name="objData">待添加数据</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>添加结果</returns>
        public object AddOrmData<T>(List<T> objData, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            object returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                conn.Insert<T>(objData, dbTrans, m_excuteTimeout);
                returnData = objData.Count;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:更新Orm数据
        /// </summary>
        /// <typeparam name="T">待更新的类型</typeparam>
        /// <param name="objData">待更新数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>更新结果</returns>
        public bool UpdateOrmData<T>(T objData, bool useTran = false) where T : class
        {
            //返回值            
            bool returnData = false;

            //数据库连接
            IDbConnection conn = null;

            //数据库事物
            IDbTransaction tran = null;

            try
            {
                //获取数据库连接
                conn = CreateConnection();

                //创建事物
                if (useTran)
                {
                    //数据库事物
                    tran = conn.BeginTransaction();
                    returnData = conn.Update(objData, tran, m_excuteTimeout);
                    tran.Commit();
                }
                else
                {
                    returnData = conn.Update(objData, null, m_excuteTimeout);
                }
            }
            catch (Exception)
            {
                if (useTran)
                {
                    tran.Rollback();
                }
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:删除Orm数据
        /// </summary>
        /// <typeparam name="T">待删除的类型</typeparam>
        /// <param name="objData">待删除数据</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>删除结果</returns>
        public bool UpdateOrmData<T>(T objData, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            bool returnData = false;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = conn.Update(objData, dbTrans, m_excuteTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:删除Orm数据
        /// </summary>
        /// <typeparam name="T">待删除的类型</typeparam>
        /// <param name="objData">待删除数据</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>删除结果</returns>
        public bool DeleteOrmData<T>(T objData, bool useTran = false) where T : class
        {
            //返回值            
            bool returnData = false;

            //数据库连接
            IDbConnection conn = null;

            //数据库事物
            IDbTransaction tran = null;

            try
            {
                //获取数据库连接
                conn = CreateConnection();

                //创建事物
                if (useTran)
                {
                    //数据库事物
                    tran = conn.BeginTransaction();
                    returnData = conn.Delete(objData, tran, m_excuteTimeout);
                    tran.Commit();
                }
                else
                {
                    returnData = conn.Delete(objData, null, m_excuteTimeout);
                }
            }
            catch (Exception)
            {
                if (useTran)
                {
                    tran.Rollback();
                }
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:删除Orm数据
        /// </summary>
        /// <typeparam name="T">待删除的类型</typeparam>
        /// <param name="objData">待删除数据</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>删除结果</returns>
        public bool DeleteOrmData<T>(T objData, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            bool returnData = false;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = conn.Delete(objData, dbTrans, m_excuteTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取主键数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="id">主键值</param>  
        /// <param name="dbTrans">事物对象</param>
        /// <returns>主键数据</returns>
        public T GetOrmDataByID<T>(object id, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            //返回值            
            T returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(useWriteConn);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取分页数据
                returnData = conn.Get<T>(id, dbTrans, m_excuteTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        ///     功能：获取数据库单值,同 ExecuteScalar, 只是参数不一样
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="parames">命令参数</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>返回值</returns>
        public object ExecuteOrmScalar(string cmdText, object parames, CommandType cmdType, IDbTransaction dbTrans = null, bool useWriteConn = false)
        {
            //返回值            
            object returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(useWriteConn);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = conn.ExecuteScalar(cmdText, parames, dbTrans, m_excuteTimeout, cmdType);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        ///     功能：获取数据库单值,同 ExecuteScalar, 只是参数不一样
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="parames">命令参数</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>返回值</returns>
        public async Task<object> ExecuteOrmScalarAsync(string cmdText, object parames, CommandType cmdType, IDbTransaction dbTrans = null, bool useWriteConn = false)
        {
            //返回值            
            object returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(useWriteConn);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = await conn.ExecuteScalarAsync(cmdText, parames, dbTrans, m_excuteTimeout, cmdType).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:执行sql，返回影响行数
        /// </summary>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>             
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">命令执行事物</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>返回影响行数</returns>
        public int ExecuteOrmData(string cmdText, object parames, CommandType cmdType, IDbTransaction dbTrans = null)
        {
            //返回值            
            int returnData = 0;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = conn.Execute(cmdText, parames, dbTrans, m_excuteTimeout, cmdType);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public async Task<int> ExecuteOrmDataAsync(string cmdText, object parames, CommandType cmdType, IDbTransaction dbTrans = null)
        {
            //返回值            
            int returnData = 0;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                returnData = await conn.ExecuteAsync(cmdText, parames, dbTrans, m_excuteTimeout, cmdType).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>   
        /// <param name="dbTrans">事物对象</param>
        /// <returns>主键数据</returns>
        public T GetSingleOrmData<T>(object predicate, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            //返回值            
            T returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(useWriteConn);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取数据
                returnData = conn.GetList<T>(predicate, null, dbTrans, this.m_excuteTimeout).FirstOrDefault<T>();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>             
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public T GetSingleOrmData<T>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            //返回值            
            T returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(useWriteConn);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取分页数据
                returnData = conn.Query<T>(cmdText, parames, dbTrans, false, this.m_excuteTimeout, cmdType).FirstOrDefault<T>();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>             
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public async Task<T> GetSingleOrmDataAsync<T>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null, bool useWriteConn = false) where T : class
        {
            //返回值            
            T returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取分页数据
                returnData = (await conn.QueryAsync<T>(cmdText, parames, dbTrans, commandTimeout: m_excuteTimeout, commandType: cmdType).ConfigureAwait(false)).FirstOrDefault<T>();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>
        /// <param name="sortInfo">排序信息(多字段排序)</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public List<T> GetOrmData<T>(object predicate, string sortField, bool isAscending, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            //排序
            IList<ISort> sort = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //转换排序
                sort = ConvertSortInfo(sortField, isAscending);

                //获取分页数据
                returnData = conn.GetList<T>(predicate, sort, dbTrans, m_excuteTimeout).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>
        /// <param name="sortInfo">排序信息(多字段排序)</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public List<T> GetOrmData<T>(object predicate, Dictionary<string, bool> sortInfo, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            //排序
            IList<ISort> sort = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //转换排序
                sort = ConvertSortInfo(sortInfo);

                //获取分页数据
                returnData = conn.GetList<T>(predicate, sort, dbTrans, m_excuteTimeout).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public List<T> GetOrmData<T>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取分页数据
                returnData = conn.Query<T>(cmdText, parames, dbTrans, false, m_excuteTimeout, cmdType).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public async Task<List<T>> GetOrmDataAsync<T>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取分页数据
                returnData = (await conn.QueryAsync<T>(cmdText, parames, dbTrans, commandType: cmdType, commandTimeout: m_excuteTimeout).ConfigureAwait(false)).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public Tuple<List<T1>, long> GetOrmPageData<T1>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null)
        {
            //返回值            
            Tuple<List<T1>, long> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取数据
                var multiData = conn.QueryMultiple(cmdText, parames, dbTrans, commandTimeout: m_excuteTimeout, commandType: cmdType);

                returnData = new Tuple<List<T1>, long>(multiData.Read<T1>().ToList(), multiData.Read<long>().FirstOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public async Task<Tuple<List<T1>, long>> GetOrmPageDataAsync<T1>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null)
        {
            //返回值            
            Tuple<List<T1>, long> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取数据
                var multiData = await conn.QueryMultipleAsync(cmdText, parames, dbTrans, commandTimeout: m_excuteTimeout, commandType: cmdType);
                returnData = new Tuple<List<T1>, long>((await multiData.ReadAsync<T1>().ConfigureAwait(false)).ToList(),
                                                       (await multiData.ReadAsync<long>().ConfigureAwait(false)).FirstOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public Tuple<List<T1>, List<T2>> GetOrmMultipleData<T1, T2>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null)
        {
            //返回值            
            Tuple<List<T1>, List<T2>> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取数据
                var multiData = conn.QueryMultiple(cmdText, parames, dbTrans, commandTimeout: m_excuteTimeout, commandType: cmdType);
                returnData = new Tuple<List<T1>, List<T2>>(multiData.Read<T1>().ToList(), multiData.Read<T2>().ToList());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public async Task<Tuple<List<T1>, List<T2>>> GetOrmMultipleDataAsync<T1, T2>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null)
        {
            //返回值            
            Tuple<List<T1>, List<T2>> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取数据
                var multiData = await conn.QueryMultipleAsync(cmdText, parames, dbTrans, commandTimeout: m_excuteTimeout, commandType: cmdType);
                returnData = new Tuple<List<T1>, List<T2>>((await multiData.ReadAsync<T1>().ConfigureAwait(false)).ToList(),
                                                           (await multiData.ReadAsync<T2>().ConfigureAwait(false)).ToList());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public Tuple<List<T1>, List<T2>, List<T3>> GetOrmMultipleData<T1, T2, T3>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null)
        {
            //返回值            
            Tuple<List<T1>, List<T2>, List<T3>> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取数据
                var multiData = conn.QueryMultiple(cmdText, parames, dbTrans, commandTimeout: m_excuteTimeout, commandType: cmdType);
                returnData = new Tuple<List<T1>, List<T2>, List<T3>>(multiData.Read<T1>().ToList(), multiData.Read<T2>().ToList(), multiData.Read<T3>().ToList());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取多项数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>
        /// <typeparam name="T2">数据类型2</typeparam>
        /// <typeparam name="T3">数据类型3</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>
        /// <param name="dbTrans">事物对象</param>
        /// <returns>列表数据</returns>
        public async Task<Tuple<List<T1>, List<T2>, List<T3>>> GetOrmMultipleDataAsync<T1, T2, T3>(string cmdText, CommandType cmdType, object parames, IDbTransaction dbTrans = null)
        {
            //返回值            
            Tuple<List<T1>, List<T2>, List<T3>> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //获取数据
                var multiData = await conn.QueryMultipleAsync(cmdText, parames, dbTrans, commandTimeout: m_excuteTimeout, commandType: cmdType);
                returnData = new Tuple<List<T1>, List<T2>, List<T3>>((await multiData.ReadAsync<T1>().ConfigureAwait(false)).ToList(),
                                                                     (await multiData.ReadAsync<T2>().ConfigureAwait(false)).ToList(),
                                                                     (await multiData.ReadAsync<T3>().ConfigureAwait(false)).ToList());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, Dictionary<string, bool> sortInfo, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            //排序
            IList<ISort> sort = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //转换排序
                sort = ConvertSortInfo(sortInfo);

                //获取分页数据
                returnData = conn.GetPage<T>(predicate, sort, pageIndex, pageSize, dbTrans, m_excuteTimeout).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        public List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, string sortField, bool isAscending, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            //排序
            IList<ISort> sort = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //转换排序
                sort = ConvertSortInfo(sortField, isAscending);

                //获取分页数据
                returnData = conn.GetPage<T>(predicate, sort, pageIndex, pageSize, dbTrans, m_excuteTimeout).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        /// <param name="totalCount">数据总数</param>
        /// <returns>分页数据</returns>
        public List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, Dictionary<string, bool> sortInfo, out long totalCount, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            //排序
            IList<ISort> sort = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //转换排序
                sort = ConvertSortInfo(sortInfo);

                //获取分页数据
                returnData = conn.GetPage<T>(predicate, sort, pageIndex, pageSize, dbTrans, m_excuteTimeout).ToList();

                //获取分页总数
                totalCount = conn.Count<T>(predicate, dbTrans, m_excuteTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
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
        /// <param name="totalCount">数据总数</param>
        /// <returns>分页数据</returns>
        public List<T> GetOrmPageData<T>(object predicate, int pageIndex, int pageSize, string sortField, bool isAscending, out long totalCount, IDbTransaction dbTrans = null) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            //排序
            IList<ISort> sort = null;

            try
            {
                //获取数据库连接
                if (null == dbTrans)
                {
                    conn = CreateConnection(false);
                }
                else
                {
                    conn = dbTrans.Connection;
                }

                //转换排序
                sort = ConvertSortInfo(sortField, isAscending);

                //获取分页数据
                returnData = conn.GetPage<T>(predicate, sort, pageIndex, pageSize, dbTrans, m_excuteTimeout).ToList();

                //获取分页总数
                totalCount = conn.Count<T>(predicate, dbTrans, m_excuteTimeout);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null == dbTrans && null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取公共单一数据（主要用于主键获取)
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="predicate">查询条件谓语对象</param>       
        /// <returns>主键数据</returns>
        public T GetSingleOrmCommonData<T>(object predicate) where T : class
        {
            //返回值            
            T returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                conn = CreateCommonConnection();

                //获取数据
                returnData = conn.GetList<T>(predicate, null, null, this.m_excuteTimeout).FirstOrDefault<T>();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取公共库列表数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="cmdText">查询命令</param>
        /// <param name="parames">命令参数</param>        
        /// <param name="cmdType">命令类型</param>       
        /// <returns>列表数据</returns>
        public List<T> GetOrmCommonData<T>(string cmdText, CommandType cmdType, object parames) where T : class
        {
            //返回值            
            List<T> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                conn = CreateCommonConnection();

                //获取分页数据
                returnData = conn.Query<T>(cmdText, parames, null, false, m_excuteTimeout, cmdType).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:获取公共库分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(包括分页语句和求总数语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>       
        /// <returns>列表数据</returns>
        public Tuple<List<T1>, long> GetOrmPageCommonData<T1>(string cmdText, CommandType cmdType, object parames)
        {
            //返回值            
            Tuple<List<T1>, long> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                conn = CreateCommonConnection();

                //获取数据
                var multiData = conn.QueryMultiple(cmdText, parames, null, commandTimeout: m_excuteTimeout, commandType: cmdType);

                returnData = new Tuple<List<T1>, long>(multiData.Read<T1>().ToList(), multiData.Read<long>().FirstOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        /// <summary>
        /// 功能:异步获取公共数据库分页数据
        /// </summary>
        /// <typeparam name="T1">数据类型1</typeparam>       
        /// <param name="cmdText">查询命令(包括分页语句和求总数语句)</param>
        /// <param name="parames">命令参数</param>       
        /// <param name="cmdType">命令类型</param>        
        /// <returns>列表数据</returns>
        public async Task<Tuple<List<T1>, long>> GetOrmPageCommonDataAsync<T1>(string cmdText, CommandType cmdType, object parames)
        {
            //返回值            
            Tuple<List<T1>, long> returnData = null;

            //数据库连接
            IDbConnection conn = null;

            try
            {
                //获取数据库连接
                conn = CreateCommonConnection();

                //获取数据
                var multiData = await conn.QueryMultipleAsync(cmdText, parames, null, commandTimeout: m_excuteTimeout, commandType: cmdType);
                returnData = new Tuple<List<T1>, long>((await multiData.ReadAsync<T1>().ConfigureAwait(false)).ToList(),
                                                       (await multiData.ReadAsync<long>().ConfigureAwait(false)).FirstOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return returnData;
        }

        #endregion

        #region 函数与过程

        /// <summary>
        /// 功能:创建公共数据库连接对象
        /// </summary>       
        /// <returns>数据库连接对象</returns>
        private IDbConnection CreateCommonConnection()
        {
            IDbConnection conReturn = null;
            conReturn = m_commonProviderFactory.CreateConnection();
            conReturn.ConnectionString = m_commonConnectionString;

            conReturn.Open();

            return conReturn;
        }

        /// <summary>
        /// 功能:创建数据库连接对象
        /// </summary>
        /// <param name="blnWrite">是否写数据库</param>
        /// <returns>数据库连接对象</returns>
        private IDbConnection CreateConnection(bool blnWrite = true)
        {
            IDbConnection conReturn = null;
            if (blnWrite)
            {
                conReturn = m_writeProviderFactory.CreateConnection();
                conReturn.ConnectionString = m_writeConnectionString;
            }
            else
            {
                conReturn = m_readProviderFactory.CreateConnection();
                conReturn.ConnectionString = m_readConnectionString;
            }

            conReturn.Open();

            return conReturn;
        }

        /// <summary>
        /// 功能:转换排序信息
        /// </summary>
        /// <param name="sortField">排序字段</param>
        /// <param name="isAscending">是否升序</param>
        /// <returns>转换后的排序信息</returns>
        private IList<ISort> ConvertSortInfo(string sortField, bool isAscending)
        {
            List<ISort> lstReturn = new List<ISort>();

            lstReturn.Add(new Sort { Ascending = isAscending, PropertyName = sortField });

            return lstReturn;
        }

        /// <summary>
        /// 功能:转换排序信息
        /// </summary>
        /// <param name="sortInfo">排序信息</param>
        /// <returns>转换后的排序信息</returns>
        private IList<ISort> ConvertSortInfo(Dictionary<string, bool> sortInfo)
        {
            List<ISort> lstReturn = new List<ISort>();

            if (null != sortInfo && sortInfo.Count > 0)
            {
                sortInfo.Any(si =>
                {
                    lstReturn.Add(new Sort { Ascending = si.Value, PropertyName = si.Key });
                    return false;
                });
            }

            return lstReturn;
        }

        #endregion
    }
}
