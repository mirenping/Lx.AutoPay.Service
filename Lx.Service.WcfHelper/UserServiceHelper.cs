using DapperExtensions;
using Lx.Common.Features.Helper;
using Lx.Common.Models.DataBase;
using Lx.Common.Models.Var;
using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lx.Service.WcfHelper
{
    /// <summary>
    /// 功能描述:用户相关服务帮助类
    /// </summary>
    public class UserServiceHelper
    {
        /// <summary>
        /// 功能描述：根据用户Id获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static FeedBackResponse<TAccounts> GetUserData(long userId)
        {

            var model = new TAccounts();
            var fbrReturn = new FeedBackResponse<TAccounts>();
            var cacheKey = CacheKeys.MakeUserLoginKey(userId);
            model = CacheHelper.Get<TAccounts>(cacheKey);
            if (null == model)
            {
                var predicate = Predicates.Field<TAccounts>(et => et.FID, Operator.Eq, userId);
                model = DBDataSourceHelper.GetSingleOrmData<TAccounts>(predicate);
                if (null != model)
                {
                    CacheHelper.Set(CacheKeys.MakeUserLoginKey(userId), model);
                }
            }
            fbrReturn.ResultData = model;
            return fbrReturn;

        }
    }
}
