using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.Log
{
    /// <summary>
    /// 功能描述:转换器    
    /// </summary>
    public class CustomPatternLayoutConverter : PatternLayoutConverter
    {
        /// <summary>
        /// 功能:通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="loggingEvent"></param>
        /// <returns></returns>
        private object LookupProperty(string property, log4net.Core.LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;

            if (null != loggingEvent.MessageObject)
            {
                PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
                if (null != propertyInfo)
                {
                    propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
                }
            }

            return propertyValue;
        }

        /// <summary>
        /// 功能:转换字段属性
        /// </summary>
        /// <param name="writer">写入器</param>
        /// <param name="loggingEvent">日志事件</param>
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (null != Option)
            {
                // 写入指定键的值
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // 写入所有的键值对
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
        }
    }
}
