using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Common.Models.Log
{
    /// <summary>
    /// 功能描述:系统日志输出    
    /// </summary>
    public class CustomLayout : PatternLayout
    {
        public CustomLayout()
        {
            this.AddConverter("Property", typeof(CustomPatternLayoutConverter));
        }
    }
}
