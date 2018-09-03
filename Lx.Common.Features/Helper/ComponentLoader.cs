using Lx.Common.Helper;
using Lx.Common.Interface;
using Lx.Common.Models;
using Lx.Common.Models.Var;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Features.Helper
{
    /// <summary>
    /// 组件加载类
    /// </summary>
    public class ComponentLoader
    {
        #region  方法定义  

        /// <summary>
        ///     功能：加载组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configInfo"></param>
        /// <returns></returns>
        public static T Load<T>(JsonConfigInfo configInfo)
        {
            var componentTypeName = configInfo.GetString("_type");
            if (componentTypeName == null)
            {
                var message = $"无法加载组件:{typeof(T).FullName}配置文件中缺少 _type 属性";
                SysLogHelper.LogMessage("CacheHelper.Load", message, LogLevel.Error, WriteLogType.FileLog);
                throw new ArgumentOutOfRangeException(message);
               
            }
            var classNameArray = componentTypeName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            if (classNameArray.Length == 1)
            {
                var message = $"无法加载组件:{typeof(T).FullName}配置文件中缺少 _type 属性,缺少命名空间:{componentTypeName}";
                SysLogHelper.LogMessage("CacheHelper.Load", message, LogLevel.Error, WriteLogType.FileLog);
                throw new ArgumentOutOfRangeException(message);
              
            }
            var @namespace = string.Join(".", classNameArray.Take(classNameArray.Length - 1));
            var className = classNameArray.Last();
            var target = ObjectFactoryHelper.CreateInstance<T>(className, @namespace, true);
            if (target == null)
            {
                var componentType = Type.GetType(componentTypeName);
                target = (T)Activator.CreateInstance(componentType);
            }
            JsonConvert.PopulateObject(configInfo.Itemes.ToString(), target);
            var component = target as IComponent;

            if (component != null)
            {
                component.Init();
            }
            return target;
        }

        #endregion
    }
}
