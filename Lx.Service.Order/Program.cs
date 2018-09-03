using log4net.Config;
using Lx.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Lx.Service.Order
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));

                //运行服务
                HostFactory.Run(cf =>
                {
                    cf.SetServiceName("Lx.Service.Order");
                    cf.SetDisplayName("Lx.Service.Order");
                    cf.SetDescription("订单服务");

                    cf.Service<TopshelfWrapper>(sv =>
                    {
                        sv.ConstructUsing(b => new TopshelfWrapper());
                        sv.WhenStarted(o => o.Start());
                        sv.WhenStopped(o => o.Stop());
                    });

                    cf.RunAsLocalSystem();
                    cf.StartAutomatically();
                    cf.EnablePauseAndContinue();
                });
            }
            catch (Exception ex)
            {
                string strMessage = $"服务启动失败,原因:{SysLogHelper.GetErrorLogInfo(ex, true)}";
                SysLogHelper.LogMessage("Lx.Service.Order", strMessage);
            }

            Console.ReadLine();
        }
    }
}
