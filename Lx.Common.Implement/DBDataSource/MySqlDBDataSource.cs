using DapperExtensions.Sql;
using Lx.Common.Interface;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lx.Common.Implement.DBDataSource
{
    /// <summary>
    /// 功能:My Sql数据库操作实现 
    /// </summary>
    public class MySqlDBDataSource : IDBDataSource
    {
        #region 构造函数  

        public MySqlDBDataSource()
        {
            this.IsDefault = true;
            DapperExtensions.DapperExtensions.SqlDialect = new MySqlDialect();

        }

        #endregion

       #region  方法定义  

        /// <summary>
        /// 功能:测试连接
        /// </summary>
        /// <returns>连接是否成功</returns>
        public override bool TestConnection()
        {
            //返回值
            bool blnReturn = false;

            //数据库连接
            MySqlConnection writeConn = null;
            MySqlConnection readConn = null;

            try
            {
                //获取数据库连接
                writeConn = new MySqlConnection(this.WriteConnectionString);
                readConn = new MySqlConnection(this.ReadConnectionString);

                //打开链接
                writeConn.Open();
                readConn.Open();

                blnReturn = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != writeConn)
                {
                    writeConn.Close();
                }

                if (null != readConn)
                {
                    readConn.Close();
                }
            }

            return blnReturn;
        }

        /// <summary>
        /// 功能:获取数据库当前时间函数名称
        /// </summary>
        /// <returns>数据库当前时间函数名称</returns>
        public override string GetDBCurrentTime()
        {
            return "now()";
        }

        /// <summary>
        /// 功能:获取字段左引用符号
        /// </summary>
        /// <returns>字段左引用符号</returns>
        public override string GetLeftColumnSymbol()
        {
            return "`";
        }

        /// <summary>
        /// 功能:获取字段右引用符号
        /// </summary>
        /// <returns>字段右引用符号</returns>
        public override string GetRightColumnSymbol()
        {
            return "`";
        }

        /// <summary>
        /// 功能:参数符号
        /// </summary>
        public override string GetParameterSymbol()
        {
            return "?";
        }

        /// <summary>
        /// 功能:获取分页Sql语句
        /// </summary>
        /// <param name="strSql">读取数据sql</param>
        /// <param name="intPageNo">页码, 从0开始</param>
        /// <param name="intPageSize">页大小</param>
        /// <param name="strSortBy">排序信息</param>        
        /// <returns>分页Sql</returns>
        public override string GetPageSql(string strSql, int intPageNo,
                                          int intPageSize, string strSortBy)
        {
            StringBuilder sbReturn = new StringBuilder();

            //分页计算
            int intLowRowNum = intPageSize * intPageNo;

            //分页语句          
            sbReturn.Append(strSql);

            //添加排序
            if (!string.IsNullOrWhiteSpace(strSortBy))
            {
                sbReturn.Append(" order by ");
                strSortBy = s_orderByRegex.Replace(strSortBy, "");
                sbReturn.Append(strSortBy);
                sbReturn.Append(" ");
            }

            sbReturn.Append(" limit ");
            sbReturn.Append(intLowRowNum);
            sbReturn.Append(",");
            sbReturn.Append(intPageSize);

            return sbReturn.ToString();
        }
        /// <summary>
        /// 排序过滤表达式
        /// </summary>
        static Regex s_orderByRegex = new Regex("[^a-z_, ]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 功能:从数据库中查询数据
        /// </summary>       
        /// <param name="commandText">查询命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>返回值</returns>
        public override DataSet QueryData(string commandText,
                                          Dictionary<string, object> commandParameters = null,
                                          CommandType commandType = CommandType.Text)
        {
            //返回值
            DataSet dsReturn = null;

            //数据库连接
            MySqlConnection conn = null;

            try
            {
                //统一数据库脚本
                commandText = this.UniformCommandText(commandText);

                //获取数据库连接
                conn = new MySqlConnection(this.ReadConnectionString);

                //查询命令
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandTimeout = this.ExcuteTimeout;
                cmd.CommandType = commandType;

                //添加查询参数
                SetCommandParameter(cmd, commandParameters);

                //填充数据
                MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                dsReturn = new DataSet();
                sda.Fill(dsReturn);
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

            return dsReturn;
        }

        /// <summary>
        /// 功能:从数据库中查询数据
        /// </summary>       
        /// <param name="commandText">查询命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="outPutValue">输出参数值</param>
        /// <returns>返回值</returns>
        public override DataSet QueryData(string commandText, out string outPutValue,
                                          Dictionary<string, object> commandParameters = null,
                                          CommandType commandType = CommandType.Text)
        {
            //返回值
            DataSet dsReturn = null;

            //数据库连接
            MySqlConnection conn = null;

            try
            {
                outPutValue = "";

                //统一数据库脚本
                commandText = this.UniformCommandText(commandText);

                //获取数据库连接
                conn = new MySqlConnection(this.ReadConnectionString);

                //查询命令
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandTimeout = this.ExcuteTimeout;
                cmd.CommandType = commandType;

                //添加查询参数
                SetCommandParameter(cmd, commandParameters, true);

                //填充数据
                MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                dsReturn = new DataSet();
                sda.Fill(dsReturn);

                //获取输出参数值
                object objValue = cmd.Parameters["?outResult"].Value;
                if (null != objValue)
                {
                    outPutValue = objValue.ToString();
                }
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

            return dsReturn;
        }

        /// <summary>
        /// 功能:保存数据到数据库(包括新增、修改、删除)
        /// </summary>       
        /// <param name="commandText">保存命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="lstParameters">命令参数列表</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>受影响行数</returns>
        public override int SaveData(string commandText,
                                     List<Dictionary<string, object>> lstParameters,
                                     CommandType commandType = CommandType.Text, bool useTran = false)
        {
            //返回值            
            int intAffectedRows = 0;

            //数据库连接
            MySqlConnection conn = null;

            //数据库事物
            MySqlTransaction tran = null;

            try
            {
                //统一数据库脚本
                commandText = this.UniformCommandText(commandText);

                //获取数据库连接
                conn = new MySqlConnection(this.WriteConnectionString);

                //打开连接
                conn.Open();

                //删除命令
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandTimeout = this.ExcuteTimeout;
                cmd.CommandType = commandType;

                //创建事物
                if (useTran)
                {
                    tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                }

                //执行命令
                int intSaveCount = 0;
                lstParameters.ForEach(parm =>
                {
                    //添加查询参数
                    SetCommandParameter(cmd, parm);

                    //执行
                    intSaveCount = cmd.ExecuteNonQuery();
                    intAffectedRows = intAffectedRows + intSaveCount;
                });

                if (useTran)
                {
                    tran.Commit();
                }
            }
            catch (Exception)
            {
                if (useTran)
                {
                    tran.Rollback();
                }
                intAffectedRows = 0;

                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return intAffectedRows;
        }

        /// <summary>
        /// 功能:获取数据库单值
        /// </summary>       
        /// <param name="commandText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <returns>返回值</returns>
        public override object ExecuteScalar(string commandText,
                                             Dictionary<string, object> commandParameters = null,
                                             CommandType commandType = CommandType.Text, bool useWriteConn = false)
        {
            //返回值
            object objReturn = null;

            //数据库连接
            MySqlConnection conn = null;

            try
            {
                //统一数据库脚本
                commandText = this.UniformCommandText(commandText);

                //获取数据库连接
                if (useWriteConn)
                {
                    conn = new MySqlConnection(this.WriteConnectionString);
                }
                else
                {
                    conn = new MySqlConnection(this.ReadConnectionString);
                }

                //打开连接
                conn.Open();

                //查询命令
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandTimeout = this.ExcuteTimeout;
                cmd.CommandType = commandType;

                //添加查询参数
                SetCommandParameter(cmd, commandParameters);

                //获取数据
                objReturn = cmd.ExecuteScalar();
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

            return objReturn;
        }

        /// <summary>
        /// 功能:执行命令语句
        /// </summary>
        /// <param name="commandText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="useTran">是否使用事物</param>
        /// <returns>受影响的行数</returns>
        public override int ExecuteNonQuery(string commandText,
                                            Dictionary<string, object> commandParameters = null,
                                            CommandType commandType = CommandType.Text, bool useTran = false)
        {
            //返回值            
            int intAffectedRows = 0;

            //数据库连接
            MySqlConnection conn = null;

            //数据库事物
            MySqlTransaction tran = null;

            try
            {
                //统一数据库脚本
                commandText = this.UniformCommandText(commandText);

                //获取数据库连接
                conn = new MySqlConnection(this.WriteConnectionString);

                //打开连接
                conn.Open();

                //删除命令
                MySqlCommand cmd = conn.CreateCommand();
                cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandTimeout = this.ExcuteTimeout;
                cmd.CommandType = commandType;

                //创建事物
                if (useTran)
                {
                    tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                }

                //添加查询参数
                SetCommandParameter(cmd, commandParameters);

                //执行命令
                intAffectedRows = cmd.ExecuteNonQuery();

                if (useTran)
                {
                    tran.Commit();
                }
            }
            catch (Exception)
            {
                if (useTran)
                {
                    tran.Rollback();
                }
                intAffectedRows = 0;

                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return intAffectedRows;
        }

        /// <summary>
        /// 功能:执行命令语句
        /// </summary>
        /// <param name="commandText">命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandParameters">命令参数</param>
        /// <param name="useTran">是否使用事物</param>
        /// <param name="outPutValue">输出参数值</param>
        /// <returns>受影响的行数</returns>
        public override int ExecuteNonQuery(string commandText, out string outPutValue,
                                            Dictionary<string, object> commandParameters = null,
                                            CommandType commandType = CommandType.Text, bool useTran = false)
        {
            //返回值            
            int intAffectedRows = 0;

            //数据库连接
            MySqlConnection conn = null;

            //数据库事物
            MySqlTransaction tran = null;

            try
            {
                outPutValue = "";

                //统一数据库脚本
                commandText = this.UniformCommandText(commandText);

                //获取数据库连接
                conn = new MySqlConnection(this.WriteConnectionString);

                //打开连接
                conn.Open();

                //删除命令
                MySqlCommand cmd = conn.CreateCommand();
                cmd = conn.CreateCommand();
                cmd.CommandText = commandText;
                cmd.CommandTimeout = this.ExcuteTimeout;
                cmd.CommandType = commandType;

                //创建事物
                if (useTran)
                {
                    tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                }

                //添加查询参数
                SetCommandParameter(cmd, commandParameters, true);

                //执行命令
                intAffectedRows = cmd.ExecuteNonQuery();

                //获取输出参数值
                object objValue = cmd.Parameters["?outResult"].Value;
                if (null != objValue)
                {
                    outPutValue = objValue.ToString();
                }

                if (useTran)
                {
                    tran.Commit();
                }
            }
            catch (Exception)
            {
                if (useTran)
                {
                    tran.Rollback();
                }
                intAffectedRows = 0;

                throw;
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }

            return intAffectedRows;
        }

        #endregion

        #region  函数与过程  

        /// <summary>
        /// 功能:设置命令参数
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="dictParam">参数值</param>
        /// <param name="needOutParam">是否返回输出参数(参数名:outResult)</param>
        private void SetCommandParameter(MySqlCommand cmd, Dictionary<string, object> dictParam, bool needOutParam = false)
        {
            cmd.Parameters.Clear();
            if (null != dictParam)
            {
                dictParam.Any(p =>
                {
                    cmd.Parameters.Add(new MySqlParameter
                    {
                        Value = p.Value ?? DBNull.Value,
                        ParameterName = p.Key,
                    });

                    return false;
                });
            }

            if (needOutParam)
            {
                MySqlParameter outParam = new MySqlParameter("?outResult", MySqlDbType.String);
                outParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outParam);
            }
        }

        #endregion
    }
}
