using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lx.Common.Models
{
    /// <summary>
    /// Json配置文件信息类
    /// </summary>
    public class JsonConfigInfo
    {
        #region  变量定义  

        //配置信息
        private JObject m_itemes = null;

        #endregion

        #region   构造函数  


        #endregion

        #region   静态函数  

        /// <summary>
        /// 功能:从文件加载配置
        /// </summary>
        /// <param name="configFile">配置文件</param>
        /// <returns></returns>
        public static JsonConfigInfo LoadFromFile(string configFile)
        {
            string strFilePath = GetConfigFilePath(configFile);

            if (!File.Exists(strFilePath))
            {
                throw new Exception("配置文件 " + strFilePath + " 不存在");
            }
            var json = File.ReadAllText(strFilePath);

            return LoadFromString(json);
        }

        /// <summary>
        /// 功能:从字符串加载配置
        /// </summary>
        /// <param name="json">配置字符串</param>
        /// <returns></returns>
        public static JsonConfigInfo LoadFromString(string json)
        {
            JsonConfigInfo configInfo = new JsonConfigInfo();
            configInfo.m_itemes = JObject.Parse(json);
            return configInfo;
        }

        #endregion

        #region   属性定义  

        public JObject Itemes
        {
            get
            {
                return m_itemes;
            }
        }

        #endregion

        #region  方法定义  

        /// <summary>
        /// 功能:获取整形参数值
        /// </summary>
        /// <param name="key">参数键值</param>
        /// <returns>整形参数值</returns>
        public int GetInt(string key)
        {
            return GetValue<int>(key);
        }

        /// <summary>
        /// 功能:获取字符串参数值
        /// </summary>
        /// <param name="key">参数键值</param>
        /// <returns>字符串参数值</returns>
        public string GetString(string key)
        {
            return GetValue<string>(key);
        }

        /// <summary>
        /// 功能:获取泛型参数值
        /// </summary>
        /// <param name="key">参数键值</param>
        /// <returns>泛型参数值</returns>
        public T GetValue<T>(string key)
        {
            if (null == m_itemes)
            {
                return default(T);
            }
            return m_itemes[key].Value<T>();
        }

        /// <summary>
        /// 功能:获取字符串参数数组
        /// </summary>
        /// <param name="key">参数键值</param>
        /// <returns>字符串参数值数组</returns>
        public List<string> GetStringList(string key)
        {
            if (null == m_itemes)
            {
                return null;
            }
            return m_itemes[key].Select(x => x.Value<string>()).ToList();
        }

        /// <summary>
        /// 功能:获取对象数组列表
        /// </summary>
        /// <param name="key">参数键值</param>
        /// <returns>对象数组列表</returns>
        public List<JObject> GetObjectList(string key)
        {
            if (null == m_itemes)
            {
                return null;
            }
            return m_itemes[key].Select(x => x.Value<JObject>()).ToList();
        }

        #endregion

        #region   函数与过程  

        /// <summary>
        /// 功能:获取配置文件路径
        /// </summary>
        /// <param name="strConfigFileName">配置文件名称</param>
        /// <returns>配置文件路径</returns>
        private static string GetConfigFilePath(string strConfigFileName)
        {
            string strConfigFilePath = Thread.GetDomain().BaseDirectory;// System.Environment.CurrentDirectory;
            strConfigFilePath = Path.Combine(strConfigFilePath, "Config");
            strConfigFilePath = Path.Combine(strConfigFilePath, strConfigFileName);
            return strConfigFilePath;
        }

        #endregion
    }
}
