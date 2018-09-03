using Lx.Common.Models;
using Newtonsoft.Json;
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
    /// 功能描述:配置帮助类  
    /// </summary>
    public class ConfigHelper
    {
        #region   方法定义  

        /// <summary>
        /// 功能:获取程序配置文件路径
        /// </summary>
        /// <returns>程序配置文件路径</returns>
        public static string GetExeConfigFilePath()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        /// <summary>
        /// 功能:加载Json配置文件信息
        /// </summary>
        /// <param name="configFile">Json配置文件信息对象</param>
        /// <returns></returns>
        public static JsonConfigInfo LoadFromFile(string configFile)
        {
            return JsonConfigInfo.LoadFromFile(configFile);
        }

        /// <summary>
        /// 功能: 从字符串加载配置
        /// </summary>
        /// <param name="json">配置字符串</param>
        /// <returns></returns>
        public static JsonConfigInfo LoadFromString(string json)
        {
            return JsonConfigInfo.LoadFromString(json);
        }

        /// <summary>
        /// 功能: 从字符串加载配置
        /// </summary>
        /// <param name="config">配置对象</param>
        /// <returns></returns>
        public static JsonConfigInfo LoadFromObject(object config)
        {
            return JsonConfigInfo.LoadFromString(JsonConvert.SerializeObject(config));
        }

        /// <summary>
        /// 功能: 获取转换类型后的配置值
        /// </summary>       
        /// <typeparam name="T">转换的类型</typeparam>
        /// <param name="strConfigKey">待转换类型的配置键值</param>
        /// <param name="strDefaultValue">默认值</param>
        /// <returns>转换类型后的配置值</returns>
        public static T GetConfigValue<T>(string strConfigKey, string strDefaultValue = "")
        {
            //配置值
            string strConfigValue = ConfigurationManager.AppSettings[strConfigKey];
            return GlobalHelper.ParseStringData<T>(strConfigValue, strDefaultValue);
        }

        /// <summary>
        /// 功能: 获取字符串类型配置值
        /// </summary>       
        /// <param name="strConfigKey">配置键值</param>
        /// <param name="strDefaultValue">默认值</param>
        /// <returns>字符串类型配置值</returns>
        public static string GetStringValue(string strConfigKey, string strDefaultValue = "")
        {
            return GetConfigValue<string>(strConfigKey, strDefaultValue);
        }

        #endregion
    }
}
