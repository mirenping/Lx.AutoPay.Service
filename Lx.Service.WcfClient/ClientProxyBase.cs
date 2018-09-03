using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfClient
{
    /// <summary>
    ///     功能：代理基类
    /// </summary>
    /// <typeparam name="TChannel">通道类别</typeparam>
    public class ClientProxyBase<TChannel> where TChannel : ICommunicationObject
    {
        #region 方法定义 

        /// <summary>
        ///     功能：同步方法调用
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">待执行的方法</param>
        /// <param name="endPointName">客户端终节点名称</param>
        /// <returns>调用结果</returns>
        protected static FeedBackResponse<TResult> CallMethod<TResult>(Func<TChannel, FeedBackResponse<TResult>> func, string endPointName = "")
        {
            TChannel clientTemp = default(TChannel);
            FeedBackResponse<TResult> fbrReturn = new FeedBackResponse<TResult>();

            try
            {
                clientTemp = (TChannel)Activator.CreateInstance(typeof(TChannel), endPointName);
                return func(clientTemp);
            }
            catch (Exception ex)
            {
                fbrReturn.SetClientErrorMsg(ex);
            }
            finally
            {
                try
                {
                    if (null != clientTemp && clientTemp.State != CommunicationState.Faulted)
                    {
                        clientTemp.Close();
                    }
                }
                catch
                {
                }
            }

            return fbrReturn;
        }

        /// <summary>
        ///     功能：异步方法调用
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="func">待执行的方法</param>
        /// <param name="endPointName">客户端终节点名称</param>
        /// <returns>调用结果</returns>
        protected static async Task<FeedBackResponse<TResult>> CallMethodAsync<TResult>(Func<TChannel, Task<FeedBackResponse<TResult>>> func, string endPointName = "")
        {
     
            TChannel clientTemp = default(TChannel);
            FeedBackResponse<TResult> fbrReturn = new FeedBackResponse<TResult>();
            try
            {
                clientTemp = (TChannel)Activator.CreateInstance(typeof(TChannel), endPointName);
                return await func(clientTemp);
            }
            catch (AggregateException aex)
            {
                fbrReturn.SetClientErrorMsg(aex.InnerExceptions[0]);
            }
            catch (Exception ex)
            {
                fbrReturn.SetClientErrorMsg(ex);
            }
            finally
            {
                try
                {
                    if (null != clientTemp && clientTemp.State != CommunicationState.Faulted)
                    {
                        clientTemp.Close();
                    }
                }
                catch
                {
                }
            }

            return fbrReturn;
        }

        #endregion
    }
}
