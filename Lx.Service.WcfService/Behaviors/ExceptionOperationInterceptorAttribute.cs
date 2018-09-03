using Lx.Common.Features.Helper;
using Lx.Service.Common;
using Lx.Service.WcfContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.WcfService.Behaviors
{
    public class ExceptionOperationInterceptorAttribute : Attribute, IOperationBehavior
    {
        #region  变量定义  

        private readonly OperationDescription m_operationDescription;

        #endregion

        #region  构造函数  

        public ExceptionOperationInterceptorAttribute(OperationDescription operationDescription)
        {
            m_operationDescription = operationDescription;
        }

        #endregion

        #region  方法定义  

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
            IOperationInvoker invoke = dispatchOperation.Invoker;
            dispatchOperation.Invoker = CreateInvoker(invoke);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        private OperationInvoker CreateInvoker(IOperationInvoker oldInvoker)
        {
            return new OperationInvoker(oldInvoker, this.m_operationDescription);
        }

        #endregion

        #region   内部类定义  

        private class OperationInvoker : IOperationInvoker
        {
            #region  变量定义  

            //方法操作
            private IOperationInvoker m_oldInvoker;

            //方法描述
            private OperationDescription m_operationDescription;

            #endregion
        
            #region   构造函数  

            /// <summary>
            /// 功能:方法调用
            /// </summary>
            /// <param name="oldInvoker">原调用</param>
            /// <param name="operationDescription">方法描述</param>
            public OperationInvoker(IOperationInvoker oldInvoker, OperationDescription operationDescription)
            {
                m_oldInvoker = oldInvoker;
                m_operationDescription = operationDescription;
            }

            #endregion
        
            #region  属性定义  

            /// <summary>
            /// 功能:获取一个值，该值指定调度程序是调用 <see cref="M:System.ServiceModel.Dispatcher.IOperationInvoker.Invoke(System.Object,System.Object[],System.Object[]@)"/> 方法还是调用 <see cref="M:System.ServiceModel.Dispatcher.IOperationInvoker.InvokeBegin(System.Object,System.Object[],System.AsyncCallback,System.Object)"/> 方法。
            /// </summary>
            /// <returns>
            /// 如果调度程序调用同步操作，则为 true；否则为 false。
            /// </returns>          
            public bool IsSynchronous
            {
                get
                {
                    return this.m_oldInvoker.IsSynchronous;
                }
            }

            #endregion
        
            #region  方法定义  

            /// <summary>
            /// 功能:返回参数对象的 <see cref="T:System.Array"/>。
            /// </summary>
            /// <returns>
            /// 要用作操作的实参的形参。
            /// </returns>
            public object[] AllocateInputs()
            {
                return m_oldInvoker.AllocateInputs();
            }

            /// <summary>
            /// 功能:从一个实例和输入对象的集合返回一个对象和输出对象的集合。
            /// </summary>
            /// <returns>
            /// 返回值。
            /// </returns>
            /// <param name="instance">要调用的对象。</param><param name="inputs">方法的输入。</param><param name="outputs">方法的输出。</param>
            public object Invoke(object instance, object[] inputs, out object[] outputs)
            {
                outputs = null;
                object returnValue = null;

                try
                {
                    return m_oldInvoker.Invoke(instance, inputs, out outputs);
                }
                catch (DataMessageException dmex)
                {
                    returnValue = GetDataErrorResponse(dmex);
                }
                catch (Exception ex)
                {
                    returnValue = GetErrorResponse(instance, ex);
                }

                return returnValue;
            }

            /// <summary>
            /// 功能:异步实现  <see cref="M:System.ServiceModel.Dispatcher.IOperationInvoker.Invoke(System.Object,System.Object[],System.Object[]@)"/> method.
            /// </summary>
            /// <returns>
            /// 用来完成异步调用的 <see cref="T:System.IAsyncResult"/>。
            /// </returns>
            /// <param name="instance">要调用的对象。</param><param name="inputs">方法的输入。</param><param name="callback">异步回调对象。</param><param name="state">关联的状态数据。</param>
            public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
            {
                return m_oldInvoker.InvokeBegin(instance, inputs, callback, state);
            }

            /// <summary>
            /// 功能:异步结束方法。
            /// </summary>
            /// <returns>
            /// 返回值。
            /// </returns>
            /// <param name="instance">调用的对象。</param><param name="outputs">方法的输出。</param><param name="result"><see cref="T:System.IAsyncResult"/> 对象。</param>
            public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
            {
                outputs = null;
                object returnValue = null;

                try
                {
                    returnValue = m_oldInvoker.InvokeEnd(instance, out outputs, result);
                }
                catch (DataMessageException dmex)
                {
                    returnValue = GetDataErrorResponse(dmex);
                }
                catch (Exception ex)
                {
                    returnValue = GetErrorResponse(instance, ex);
                }

                return returnValue;
            }

            #endregion

            #region  函数与过程  

            /// <summary>
            /// 功能:创建错误返回对象
            /// </summary>
            /// <param name="instance">服务实例对象</param>
            /// <param name="ex">错语</param>
            /// <returns>错误返回对象</returns>
            private FeedBackResponseBase GetErrorResponse(object instance, Exception ex)
            {
                StringBuilder sbSource = new StringBuilder();
                FeedBackResponseBase fbrbReturn = new FeedBackResponseBase();

                if (null != instance)
                {
                    sbSource.Append(instance.ToString());
                }
                sbSource.Append(".");
                sbSource.Append(m_operationDescription.Name);

                fbrbReturn.SetServiceErrorMsg(ex);
              //日志

                return fbrbReturn;
            }

            /// <summary>
            /// 功能:创建数据错误返回对象
            /// </summary>
            /// <param name="instance">服务实例对象</param>
            /// <param name="dmex">错误</param>
            /// <returns>错误返回对象</returns>
            private FeedBackResponseBase GetDataErrorResponse(DataMessageException dmex)
            {
                FeedBackResponseBase fbrbReturn = new FeedBackResponseBase();

                string strMegInfo = GlobalHelper.GetBdMessageValue(dmex.MsgCode, dmex.FormatArgs);

                fbrbReturn.SetMsgCode(dmex.MsgCode, strMegInfo);

                return fbrbReturn;
            }

            #endregion
        }

        #endregion
    }
}
