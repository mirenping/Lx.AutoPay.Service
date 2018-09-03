using Lx.Common.Helper;
using Lx.Service.Common;
using Lx.Service.WcfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WinServiceHost
{
    /// <summary>
    /// 功能描述:Topshelf包装器 
    /// </summary>
    public class TopshelfWrapper
    {
        #region   变量定义  

        //日志服务主机
        private ServiceHost<LogService> m_logServiceHost = null;

        //用户服务主机
        private ServiceHost<UserService> m_userServiceHost = null;

        //订单服务主机
        private ServiceHost<OrderService> m_OrderServiceHost = null;
        #endregion


        #region  方法定义  
        /// <summary>
        /// 功能:启动服务         
        /// </summary>        
        public void Start()
        {
            try
            {
                //获取配置的服务名称列表
                List<string> lstConfigServices = ServiceHelper.GetConfigServices();

                //启动服务主机
                lstConfigServices.ForEach(cs =>
                {
                    //获取待启动的Windows服务的名称                    
                    int intPosition = cs.LastIndexOf(".");
                    string strServiceName = cs.Substring(intPosition + 1);
                    //日志服务
                    if (cs.IndexOf("LogService", StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                        m_logServiceHost = new ServiceHost<LogService>();
                        m_logServiceHost.EnableMetadataExchange = true;
                        m_logServiceHost.Open();
                        Console.WriteLine("日志服务已经启动,如果要退出,请输入exit,不要直接关闭");
                    }

                    //用户服务
                    if (cs.IndexOf("UserService", StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                        m_userServiceHost = new ServiceHost<UserService>();
                        m_userServiceHost.EnableMetadataExchange = true;
                        m_userServiceHost.Open();
                        Console.WriteLine("用户服务已经启动,如果要退出,请输入exit,不要直接关闭");
                    }
                    //订单服务
                    if (cs.IndexOf("OrderService", StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                        m_OrderServiceHost = new ServiceHost<OrderService>();
                        m_OrderServiceHost.EnableMetadataExchange = true;
                        m_OrderServiceHost.Open();
                        Console.WriteLine("订单服务已经启动,如果要退出,请输入exit,不要直接关闭");
                    }
                });
            }
            catch (Exception ex)
            {
                string strMessage = $"服务启动失败,原因:{SysLogHelper.GetErrorLogInfo(ex, true)}";
                SysLogHelper.LogMessage("Lx.Service.WinServiceHost.Start", strMessage);
            }
        }

        /// <summary>
        /// 功能:停止服务         
        /// </summary>        
        public void Stop()
        {
            try
            {
                //关闭服务
                if (null != m_logServiceHost)
                {
                    if (m_logServiceHost.State != CommunicationState.Closed)
                    {
                        m_logServiceHost.Close();
                    }

                    m_logServiceHost = null;
                }

                if (null != m_userServiceHost)
                {
                    if (m_userServiceHost.State != CommunicationState.Closed)
                    {
                        m_userServiceHost.Close();
                    }

                    m_userServiceHost = null;
                }

                if (null != m_OrderServiceHost)
                {
                    if (m_OrderServiceHost.State != CommunicationState.Closed)
                    {
                        m_OrderServiceHost.Close();
                    }
                    m_OrderServiceHost = null;
                }
                //调用垃圾回收
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                string strMessage = $"服务启动失败,原因:{SysLogHelper.GetErrorLogInfo(ex, true)}";
                SysLogHelper.LogMessage("Lx.Service.WinServiceHost.Stop", strMessage);
            }
        }

        #endregion
    }
}
