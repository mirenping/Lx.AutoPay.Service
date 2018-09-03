using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lx.Service.Common
{
    /// <summary>
    /// 功能描述:服务帮助类
    /// </summary>
    public class ServiceHelper
    {
        #region   构造函数  

        /// <summary>
        /// 防止实例化
        /// </summary>
        private ServiceHelper() { }

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:获取配置的服务名称列表
        /// </summary>
        /// <returns>服务名称列表</returns>
        public static List<string> GetConfigServices()
        {
            List<string> lstReturn = new List<string>();
            // 读取配置文件
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            ServiceModelSectionGroup serviceModelSectionGroup = (ServiceModelSectionGroup)configuration.GetSectionGroup("system.serviceModel");
            if (null != serviceModelSectionGroup && null != serviceModelSectionGroup.Services)
            {
                //获取每个服务
                foreach (ServiceElement serviceElement in serviceModelSectionGroup.Services.Services)
                {
                    lstReturn.Add(serviceElement.Name);
                }
            }
            return lstReturn;
        }

        #endregion

        #region  函数与过程  

        #region   配置相关  

        /// <summary>
        /// 功能:获取配置文件路径
        /// </summary>
        /// <param name="strConfigFileName">配置文件名称</param>
        /// <returns></returns>
        private static string GetConfigFilePath(string strConfigFileName)
        {
            return Path.Combine(Thread.GetDomain().BaseDirectory, strConfigFileName);
        }

        /// <summary>
        /// 功能:获取指定配置文件中某键的值
        /// </summary>
        /// <param name="strKeyName">键名称</param>
        /// <param name="strConfigSectionName">配置节名称</param>
        /// <param name="strConfigFileName">配置文件名称</param>        
        /// <returns>某键的值</returns>
        public static string GetServiceConfigValue(string strKeyName, string strConfigFileName)
        {
            return GetServiceConfigValue(strKeyName, "appSettings", strConfigFileName);
        }

        /// <summary>
        /// 功能:获取前缀相关配置列表
        /// </summary>
        /// <param name="strPrefix">相关配置前缀</param>
        /// <param name="strConfigSectionName">配置节名称</param>
        /// <param name="strConfigFileName">配置文件名称</param> 
        /// <returns>相关配置列表</returns>
        public static List<string> GetServicePrefixConfigValues(string strPrefix, string strConfigFileName)
        {
            return GetServicePrefixConfigValues(strPrefix, "appSettings", strConfigFileName);
        }

        /// <summary>
        /// 功能:获取指定配置文件中某键的值
        /// </summary>
        /// <param name="strKeyName">键名称</param>
        /// <param name="strConfigSectionName">配置节名称</param>
        /// <param name="strConfigFileName">配置文件名称</param>        
        /// <returns>某键的值</returns>
        public static string GetServiceConfigValue(string strKeyName, string strConfigSectionName, string strConfigFileName)
        {
            //加载配置文件
            XElement xeTemp = XElement.Load(GetConfigFilePath(strConfigFileName));

            //获取指定节的配置值
            var lstConfigValues = (from p in xeTemp.Element(strConfigSectionName).Descendants()
                                   let kValue = p.Attribute("key").Value
                                   let vValue = p.Attribute("value").Value
                                   where kValue.Equals(strKeyName, StringComparison.InvariantCultureIgnoreCase)
                                   select vValue).ToList();

            return lstConfigValues[0];
        }

        /// <summary>
        /// 功能:获取前缀相关配置列表
        /// </summary>
        /// <param name="strPrefix">相关配置前缀</param>
        /// <param name="strConfigSectionName">配置节名称</param>
        /// <param name="strConfigFileName">配置文件名称</param> 
        /// <returns>相关配置列表</returns>
        public static List<string> GetServicePrefixConfigValues(string strPrefix, string strConfigSectionName, string strConfigFileName)
        {
            //加载配置文件
            XElement xeTemp = XElement.Load(GetConfigFilePath(strConfigFileName));

            //获取前缀相关配置列表
            var lstConfigValues = (from p in xeTemp.Element(strConfigSectionName).Descendants()
                                   let kValue = p.Attribute("key").Value
                                   let vValue = p.Attribute("value").Value
                                   where kValue.StartsWith(strPrefix, StringComparison.InvariantCultureIgnoreCase) &&
                                         string.IsNullOrEmpty(vValue) == false
                                   select vValue).ToList();

            return lstConfigValues;
        }

        #endregion

      

        #endregion
    }
}
