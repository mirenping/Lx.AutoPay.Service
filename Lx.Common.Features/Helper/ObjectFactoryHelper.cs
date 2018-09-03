using Lx.Common.Helper;
using Lx.Common.Models.Var;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Features.Helper
{
    /// <summary>
    /// 对象工厂帮助类 
    /// </summary>
    public class ObjectFactoryHelper
    {
        #region   变量定义  

        //查询实现程序集
        private static Assembly m_queryImplementAssembly = null;

        //实例对像
        private static Dictionary<int, object> m_dictObject = null;

        //查询实现程序集名称
        private static string m_queryImplementAssemblyName = "Lx.Common.Implement";

        #endregion

        #region 构造函数  

        static ObjectFactoryHelper()
        {
            try
            {
                m_dictObject = new Dictionary<int, object>();
                m_queryImplementAssembly = Assembly.Load(m_queryImplementAssemblyName);
            }
            catch (Exception ex)
            {
                string strMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                SysLogHelper.LogMessage("ObjectFactoryHelper.Static", strMessage, LogLevel.Error);
            }
        }

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:创建对象实例
        /// </summary>
        /// <typeparam name="T">实例的接口类型</typeparam>
        /// <param name="strClassFullName">实例的类全名称(命名空间.类型名,程序集)</param>   
        /// <param name="ignoreCase">类全名是否区分大小写</param>
        /// <returns>对象实例</returns>
        public static T CreateTypeInstance<T>(string strClassFullName, bool ignoreCase = false)
        {
            if (!string.IsNullOrWhiteSpace(strClassFullName))
            {
                object objTemp = null;

                try
                {
                    //加载类型
                    Type typTemp = Type.GetType(strClassFullName, true, ignoreCase);

                    //根据类型创建实例
                    objTemp = Activator.CreateInstance(typTemp, true);
                    //objTemp = typTemp.Assembly.CreateInstance(strClassFullName, ignoreCase);
                    return (T)objTemp;
                }
                catch (Exception ex)
                {
                    string strMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                    SysLogHelper.LogMessage("ObjectFactoryHelper.CreateTypeInstance", strMessage, LogLevel.Error);
                    return default(T);

                }
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 功能:创建对象实例
        /// </summary>
        /// <typeparam name="T">实例的接口类型</typeparam>
        /// <param name="strAssemblyName">类型所在程序集名称</param>
        /// <param name="strClassNameSpace">类型所在命名空间</param>
        /// <param name="strClassName">类型名称</param>
        /// <param name="ignoreCase">类名是否区分大小写</param>
        /// <returns>对象实例</returns>
        public static T CreateTypeInstance<T>(string strAssemblyName, string strClassNameSpace, string strClassName, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(strAssemblyName) || string.IsNullOrWhiteSpace(strClassNameSpace) || string.IsNullOrWhiteSpace(strClassName))
            {
                return default(T);
            }
            else
            {
                object objTemp = null;

                try
                {
                    //命名空间.类型名
                    string fullName = strClassNameSpace + "." + strClassName;

                    //加载程序集，创建程序集里面的 命名空间.类型名 实例
                    objTemp = Assembly.Load(strAssemblyName).CreateInstance(fullName, ignoreCase);

                    //类型转换并返回
                    return (T)objTemp;
                }
                catch (Exception ex)
                {
                    string strMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                    SysLogHelper.LogMessage("ObjectFactoryHelper.CreateTypeInstance", strMessage, LogLevel.Error);
                    return default(T);
                }
            }
        }

        /// <summary>
        /// 功能:创建对象实例
        /// </summary>
        /// <typeparam name="T">实例的接口类型</typeparam>
        /// <param name="strClassName">实例的类名称</param>
        /// <param name="strClassNameSpace">实例的类所在的空间名称</param>
        /// <param name="blnCreateNew">是否每次创建新对象</param>
        /// <returns>对象实例</returns>
        public static T CreateInstance<T>(string strClassName, string strClassNameSpace, bool blnCreateNew = false)
        {
            if (!string.IsNullOrWhiteSpace(strClassName))
            {
                object objTemp = null;
                string strInstanceName = "";
                if (string.IsNullOrWhiteSpace(strClassNameSpace))
                {
                    strInstanceName = m_queryImplementAssemblyName + "." + strClassName;
                }
                else
                {
                    strInstanceName = m_queryImplementAssemblyName + "." + strClassNameSpace + "." + strClassName;
                }

                try
                {
                    if (blnCreateNew)
                    {
                        objTemp = m_queryImplementAssembly.CreateInstance(strInstanceName);
                    }
                    else
                    {
                        int intKey = strInstanceName.GetHashCode();
                        if (!m_dictObject.TryGetValue(intKey, out objTemp))
                        {
                            objTemp = m_queryImplementAssembly.CreateInstance(strInstanceName);
                            m_dictObject[intKey] = objTemp;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string strMessage = SysLogHelper.GetErrorLogInfo(ex, true);
                    SysLogHelper.LogMessage("ObjectFactoryHelper.CreateInstance", strMessage, LogLevel.Error);
                    return default(T);
                }

                return (T)objTemp;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 功能:创建对象实例
        /// </summary>
        /// <typeparam name="T">实例的接口类型</typeparam>
        /// <param name="strClassNameKey">实例的类名称键值</param>
        /// <param name="strClassNameSpaceKey">实例的类所在的空间名称键值</param>
        /// <param name="blnCreateNew">是否每次创建新对象</param>
        /// <returns>对象实例</returns>
        public static T CreateInstanceUseKey<T>(string strClassNameKey, string strClassNameSpaceKey, bool blnCreateNew = false)
        {
            if (!string.IsNullOrWhiteSpace(strClassNameKey))
            {
                //类名称
                string strClassName = ConfigurationManager.AppSettings[strClassNameKey];
                //类命名空间
                string strClassNameSpace = "";
                if (!string.IsNullOrWhiteSpace(strClassNameSpaceKey))
                {
                    strClassNameSpace = ConfigurationManager.AppSettings[strClassNameSpaceKey];
                }

                return CreateInstance<T>(strClassName, strClassNameSpace, blnCreateNew);
            }
            else
            {
                return default(T);
            }
        }

        #endregion
    }
}
