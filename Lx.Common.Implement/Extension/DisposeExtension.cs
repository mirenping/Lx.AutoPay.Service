using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Implement.Extension
{
    /// <summary>
    /// 功能描述：释放资源帮助类
    /// </summary>
    public static class DisposeExtension
    { 
        /// <summary>
        ///  功能：释放 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public static void Dispose<T>(this IEnumerable<T> items) where T : IDisposable
        {
            if (items == null) throw new ArgumentNullException("items");
            foreach (var item in items)
            {
                try
                {
                    if (item != null)
                    {
                        item.Dispose();
                    }
                }
                catch (Exception)
                {
                }
            }
        }     
    }
}
