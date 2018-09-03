using log4net.Config;
using Lx.Common.Features.Helper;
using Lx.Common.Helper;
using Lx.Common.Models;
using Lx.Service.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Lx.Service.WinServiceHost
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));
                //获取配置的服务名称列表
                List<string> lstConfigServices = ServiceHelper.GetConfigServices();
                //启动服务
                if (null != lstConfigServices && lstConfigServices.Count > 0)
                {
                    //获取Windows服务信息
                    JsonConfigInfo winServiceInfo = ConfigHelper.LoadFromFile("WinServiceInfo.json");

                    //获取待启动的Windows服务的名称    
                    string strServiceInfo = lstConfigServices[0];
                    int intPosition = strServiceInfo.LastIndexOf(".");
                    string strServiceName = strServiceInfo.Substring(intPosition + 1);

                    //获取服务配置
                    JObject joServiceInfo = winServiceInfo.GetValue<JObject>(strServiceName);

                    //运行服务
                    HostFactory.Run(cf =>
                    {
                        cf.SetServiceName(joServiceInfo["ServiceName"].Value<string>());
                        cf.SetDisplayName(joServiceInfo["DisplayName"].Value<string>());
                        cf.SetDescription(joServiceInfo["Description"].Value<string>());

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
                else
                {
                    Console.WriteLine("没有配置服务,无服务可启动!");
                }
            }
            catch (Exception ex)
            {
                string strMessage =$"服务启动失败,原因:{SysLogHelper.GetErrorLogInfo(ex, true)}";
                SysLogHelper.LogMessage("Lx.Service.WinServiceHost.Main", strMessage);
            }

            Console.ReadLine();
        }
    }
}
