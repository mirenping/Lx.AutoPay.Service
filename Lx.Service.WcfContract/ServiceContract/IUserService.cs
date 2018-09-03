
using Lx.Common.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfContract
{
    /// <summary>
    /// 功能描述:用户服务
    /// </summary>
    [ServiceContract(Namespace = "Lx.Service.WcfContract",
                     Name = "IUserService")]
    public interface IUserService
    {
        #region  测试  

        /// <summary>
        /// 功能描述:测试服务
        /// </summary>
        /// <param name="strParam">监控参数</param>
        /// <returns>返回结果</returns>
        [OperationContract()]
        FeedBackResponse<string> TestService(string strParam);

        #endregion

        #region 方法定义
        /// <summary>
        /// 功能描述：获取用户数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [OperationContract]
        FeedBackResponse<TAccounts> GetUserData(long userId);
        #endregion
    }
}
