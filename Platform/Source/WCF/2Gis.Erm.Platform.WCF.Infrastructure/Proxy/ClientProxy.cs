using System;
using System.Linq.Expressions;
using System.ServiceModel;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy
{
    public sealed class ClientProxy<TChannel> : IClientProxy<TChannel>
    {
        private readonly TChannel _channel;
        private readonly IClientChannel _clientChannel;

        public ClientProxy(TChannel channel)
        {
            _channel = channel;
            _clientChannel = (IClientChannel)channel;
        }

        public void Execute(Action<TChannel> action)
        {
            Func<TChannel, bool> eatResultFunc = x =>
            {
                action(x);
                return true;
            };

            Execute(eatResultFunc);
        }

        public TResult Execute<TResult>(Func<TChannel, TResult> func)
        {
            // Do not use the 'using' statement when working with a WCF client: http://msdn.microsoft.com/en-us/library/aa355056.aspx
            try
            {
                var result = func(_channel);
                _clientChannel.Close();

                return result;
            }
            catch (FaultException ex)
            {
                _clientChannel.Close();
                throw new ApplicationException(ex.Message, ex);
            }
            catch (CommunicationException ex)
            {
                _clientChannel.Abort();
                throw new ApplicationException(ex.Message, ex);
            }
            catch (TimeoutException ex)
            {
                _clientChannel.Abort();
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception)
            {
                _clientChannel.Abort();
                throw;
            }
        }

        public bool TryExecuteWithFaultContract<TResult>(Func<TChannel, TResult> func, out TResult result, out object faultContract)
        {
            // Do not use the 'using' statement when working with a WCF client: http://msdn.microsoft.com/en-us/library/aa355056.aspx
            try
            {
                result = func(_channel);
                faultContract = null;
                _clientChannel.Close();

                return true;
            }
            catch (FaultException ex)
            {
                _clientChannel.Close();

                var exceptionType = ex.GetType();
                if (!exceptionType.IsGenericType)
                {
                    throw new ApplicationException(ex.Message, ex);
                }

                var argumentType = exceptionType.GetGenericArguments()[0];
                if (argumentType == typeof(ExceptionDetail))
                {
                    throw new ApplicationException(ex.Message, ex);
                }

                // harvest fault contract
                var parameterExpression = Expression.Parameter(exceptionType, "x");
                var detailProperty = exceptionType.GetProperty("Detail");
                var parameterDetailProperty = Expression.Property(parameterExpression, detailProperty);
                var lambdaExpression = Expression.Lambda(parameterDetailProperty, parameterExpression);
                var @delegate = lambdaExpression.Compile();

                faultContract = @delegate.DynamicInvoke(ex);
                result = default(TResult);
                return false;
            }
            catch (CommunicationException ex)
            {
                _clientChannel.Abort();
                throw new ApplicationException(ex.Message, ex);
            }
            catch (TimeoutException ex)
            {
                _clientChannel.Abort();
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception)
            {
                _clientChannel.Abort();
                throw;
            }
        }
    }
}