using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Helper
{
    /// <summary>
    /// 功能描述:文件帮助类 
    /// </summary>
    public class FileHelper
    {
        #region 变量定义 

        /// <summary>
        /// 文件同步访问对象
        /// </summary>
        private static readonly object m_synclock = new object();

        #endregion

        #region  构造函数  

        /// <summary>
        /// 防止实例化
        /// </summary>
        private FileHelper() { }

        #endregion

        #region   方法定义  

        #region  写入数据  

        /// <summary>
        /// 功能:获取Hash描述表写入数据到文件
        /// </summary>
        /// <param name="strValue">待写入的数据</param>        
        /// <param name="strFilePath">待写入文件的路径</param>
        public static void WriteFileData(string strValue, string strFilePath)
        {
            WriteFileData(strValue, strFilePath, true);
        }

        /// <summary>
        /// 功能:获取Hash描述表写入数据到文件
        /// </summary>
        /// <param name="lstValue">待写入的数据</param>       
        /// <param name="strFilePath">待写入文件的路径</param>
        public static void WriteFileData(List<string> lstValue, string strFilePath)
        {
            WriteFileData(lstValue, strFilePath, true);
        }

        /// <summary>
        /// 功能:获取Hash描述表写入数据到文件
        /// </summary>
        /// <param name="strValue">待写入的数据</param>
        /// <param name="blnAppend">是否是追加数据</param>
        /// <param name="strFilePath">待写入文件的路径</param>
        public static void WriteFileData(string strValue, string strFilePath, bool blnAppend)
        {
            lock (m_synclock)
            {
                //文件写入器
                StreamWriter swTemp = null;

                try
                {
                    //创建目录
                    string strSavePath = Path.GetDirectoryName(strFilePath);
                    if (!Directory.Exists(strSavePath))
                    {
                        Directory.CreateDirectory(strSavePath);
                    }

                    //初始化文件写入器
                    swTemp = new StreamWriter(strFilePath, blnAppend, Encoding.UTF8);

                    //自动刷新
                    swTemp.AutoFlush = true;

                    //写入数据
                    swTemp.WriteLine(strValue);
                }
                catch (Exception ex)
                {
                    string strTemp = ex.Message;
                    throw ex;
                }
                finally
                {
                    if (null != swTemp)
                    {
                        swTemp.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 功能:获取Hash描述表写入数据到文件
        /// </summary>
        /// <param name="lstValue">待写入的数据</param>
        /// <param name="blnAppend">是否是追加数据</param>
        /// <param name="strFilePath">待写入文件的路径</param>
        public static void WriteFileData(List<string> lstValue, string strFilePath, bool blnAppend)
        {
            if (null != lstValue && lstValue.Count > 0)
            {
                lock (m_synclock)
                {
                    //文件写入器
                    StreamWriter swTemp = null;

                    try
                    {
                        //创建目录
                        string strSavePath = Path.GetDirectoryName(strFilePath);
                        if (!Directory.Exists(strSavePath))
                        {
                            Directory.CreateDirectory(strSavePath);
                        }

                        //初始化文件写入器
                        swTemp = new StreamWriter(strFilePath, blnAppend, Encoding.UTF8);

                        //自动刷新
                        swTemp.AutoFlush = true;

                        //写入关键字    
                        foreach (string strValue in lstValue)
                        {
                            swTemp.WriteLine(strValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        string strTemp = ex.Message;
                        throw ex;
                    }
                    finally
                    {
                        if (null != swTemp)
                        {
                            swTemp.Close();
                        }
                    }
                }
            }
        }

        #endregion

        #region 读取数据  

        /// <summary>
        /// 功能:获取Hash描述表读取文件内容
        /// </summary>
        /// <param name="strFilePath">文件路径</param>        
        /// <returns>文件内容</returns> 
        public static string ReadFileData(string strFilePath)
        {
            return ReadFileData(strFilePath, Encoding.UTF8);
        }

        /// <summary>
        /// 功能:获取Hash描述表读取文件内容
        /// </summary>
        /// <param name="strFilePath">文件路径</param>        
        /// <returns>文件内容</returns> 
        public static byte[] ReadFileByteData(string strFilePath)
        {
            byte[] bytReturn = null;

            if (File.Exists(strFilePath))
            {
                bytReturn = File.ReadAllBytes(strFilePath);
            }

            return bytReturn;
        }

        /// <summary>
        /// 功能:获取Hash描述表读取文件内容
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        /// <param name="fileEncoding">文件编码</param>
        /// <returns>文件内容</returns> 
        public static string ReadFileData(string strFilePath, Encoding fileEncoding)
        {
            string strReturnValue = "";

            if (File.Exists(strFilePath))
            {
                strReturnValue = File.ReadAllText(strFilePath, fileEncoding);
            }

            return strReturnValue;
        }

        #endregion

        #region  删除文件  

        /// <summary>
        /// 功能:获取Hash描述表删除文件
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        public static void DeleteFile(string strFilePath)
        {
            try
            {
                if (File.Exists(strFilePath))
                {
                    File.Delete(strFilePath);
                }
            }
            catch (Exception ex)
            {
                string strTemp = ex.Message;
                throw ex;
            }
        }

        #endregion

        #region  判断文件是否存在  

        /// <summary>
        /// 功能:获取Hash描述表判断文件是否存在
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        /// <returns>文件是否存在</returns>
        public static bool IsFileExist(string strFilePath)
        {
            return File.Exists(strFilePath);
        }

        #endregion

        #endregion
    }
}
