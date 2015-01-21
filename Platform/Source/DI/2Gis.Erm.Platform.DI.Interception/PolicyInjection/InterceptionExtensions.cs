using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Operations;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.DI.Interception.PolicyInjection
{
    public static class InterceptionExtensions
    {
        public static Microsoft.Practices.Unity.InterceptionExtension.Interception SetInterceptorForOperations<THandler>(
            this Microsoft.Practices.Unity.InterceptionExtension.Interception interception,
            Dictionary<LambdaExpression, IEnumerable<IOperationServiceInterceptionDescriptor<IOperation>>> operation2DescriptorsMap,
            Func<LifetimeManager> lifetimeManagerCreator,
            Func<ResolvedParameter[]> resolvedParametersCreator)
            where THandler : ICallHandler
        {
            var resolvedParameters = resolvedParametersCreator();

            foreach (var mapItem in operation2DescriptorsMap)
            {
                var methodCallExpression = mapItem.Key.Body as MethodCallExpression;
                if (methodCallExpression == null)
                {
                    throw new NotSupportedException(mapItem.Key + " must be MethodCallExpression");
                }

                var methodInfo = methodCallExpression.Method;
                foreach (var descriptor in mapItem.Value)
                {
                    var serviceType = descriptor.OperationServiceType;
                    if (!methodInfo.DeclaringType.IsAssignableFrom(serviceType))
                    {
                        throw new NotSupportedException("Intercepted operation service must implement specified operation contract");
                    }

                    var serviceMethodInfo = serviceType.GetMethods().Single(x => AreMethodEquals(x, methodInfo));
                    if (!serviceMethodInfo.IsVirtual && !serviceType.IsInterface)
                    {
                        throw new NotSupportedException("Intercepted operation method should be virtual or interface");
                    }

                    var constructorParameters = new List<object>(resolvedParameters);
                    constructorParameters.AddRange(descriptor.HandlerInjectionParameters);
                    var injectionConstructor = new InjectionConstructor(constructorParameters.ToArray());

                    var policyName = serviceType.Name;
                    var policyDefinition = interception.AddPolicy(policyName);

                    policyDefinition
                        .AddMatchingRule(new TypeMatchingRule(serviceType))
                        .AddMatchingRule(new MethodSignatureMatchingRule(methodInfo.Name, methodInfo.GetParameters().Select(x => x.ParameterType.FullName)))
                        .AddCallHandler<THandler>(lifetimeManagerCreator(), injectionConstructor);

                    var interceptor = serviceType.IsInterface
                                          ? new Interceptor<InterfaceInterceptor>() as Interceptor
                                          : new Interceptor<VirtualMethodInterceptor>();

                    policyDefinition.Container.RegisterType(serviceType,
                                                            interceptor,
                                                            new InterceptionBehavior<PolicyInjectionBehavior>());

                    var operationType = serviceType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && typeof(IEntityOperation).IsAssignableFrom(x));
                    if (operationType != null)
                    {
                        var operationDescriptor = new EntitySet(operationType.GetGenericArguments().Select(x => x.AsEntityName()).ToArray());
                        policyDefinition.Container.RegisterType(serviceType,
                                                                operationDescriptor.ToString(),
                                                                interceptor,
                                                                new InterceptionBehavior<PolicyInjectionBehavior>());
                    }
                }
            }

            return interception;
        }

        // Original source at http://ayende.com/blog/2658/method-equality
        private static bool AreMethodEquals(MethodInfo left, MethodInfo right)
        {
            if (left.Equals(right))
            {
                return true;
            }
            
            var leftParams = left.GetParameters();
            var rightParams = right.GetParameters();
            if (leftParams.Length != rightParams.Length)
            {
                return false;
            }

            for (var i = 0; i < leftParams.Length; i++)
            {
                if (leftParams[i].ParameterType != rightParams[i].ParameterType)
                {
                    return false;
                }
            }

            if (left.ReturnType != right.ReturnType)
            {
                return false;
            }

            return true;
        }
    }
}