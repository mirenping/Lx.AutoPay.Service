using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Lx.Service.WcfService.Behaviors;
using Lx.Common.Models.DataBase;
using Lx.Service.WcfHelper;

namespace Lx.Service.WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                  Namespace = "Lx.Service.WcfService",
                  Name = "UserService")]
    [ErrorInterceptor]
    public class UserService : IUserService
    {  
        
        
        /// <summary>
       /// 功能描述:测试服务
       /// </summary>
       /// <param name="strParam">监控参数</param>
       /// <returns>返回结果</returns>       
        public FeedBackResponse<string> TestService(string strParam)
        {
            strParam = strParam ?? "";
            return new FeedBackResponse<string>() { ResultData = strParam + "UserService is ok!" };
        }

        /// <summary>
        /// 功能描述：获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public FeedBackResponse<TAccounts> GetUserData(long userId)
        {
            return UserServiceHelper.GetUserData(userId);
        }
    }
}
