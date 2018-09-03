using Lx.Common.Helper;
using Lx.Service.Order.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.Order
{
    public class TopshelfWrapper
    {
        #region  方法定义  

        /// <summary>
        /// 功能:启动服务         
        /// </summary>        
        public void Start()
        {
            OrderMQConsumerHelper.Initalize();
        }

        /// <summary>
        /// 功能:停止服务         
        /// </summary>        
        public void Stop()
        {
            try
            {
                //关闭服务  
                OrderMQConsumerHelper.Release();
                //调用垃圾回收
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                string strMessage = $"服务停止失败,原因:{SysLogHelper.GetErrorLogInfo(ex, true)}";
                SysLogHelper.LogMessage("Lx.Service.Order", strMessage);
            }
        }

        #endregion      
    }
}
