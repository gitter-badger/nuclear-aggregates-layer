using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.UI.Web.Mvc.DI;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.DI
{
    public static class InterceptionExtensions
    {
        /// <summary>
        /// Добавляет обработчик типа <typeparamref name="THandler"/> вызова метода класса <typeparamref name="TTargetType"/>, 
        /// указанного в параметре <paramref name="targetMethodExpression"/>
        /// </summary>
        /// <typeparam name="TTargetType"> Тип, для метода которого генерируется обработчик вызова </typeparam>
        /// <typeparam name="THandler"> Тип обработчика вызова </typeparam>
        /// <param name="interception"></param>
        /// <param name="targetMethodExpression"></param>
        /// <param name="policyName"></param>
        /// <param name="scope"></param>
        /// <param name="lifetimeManager"></param>
        /// <param name="injectionMembersForHandler"> Параметры конструктора обработчика вызова </param>
        /// <returns></returns>
        public static Interception SetVirtualInterceptorForMethod<TTargetType, THandler>(this Interception interception,
                                                                                         Expression<Action<TTargetType>> targetMethodExpression,
                                                                                         string policyName,
                                                                                         string scope,
                                                                                         LifetimeManager lifetimeManager,
                                                                                         params InjectionMember[] injectionMembersForHandler)
            where THandler : ICallHandler
        {
            var memberExpression = targetMethodExpression.Body as MethodCallExpression;
            if (memberExpression == null)
            {
                throw new NotSupportedException("Body of targetMethodExpression should be of type MethodCallExpression");
            }

            var methodInfo = memberExpression.Method;
            if (!methodInfo.IsVirtual && !typeof(TTargetType).IsInterface)
            {
                throw new NotSupportedException("Interception method shold be virtual or interface.");
            }

            var interceptor = typeof(TTargetType).IsInterface
                                  ? new Interceptor<InterfaceInterceptor>() as Interceptor
                                  : new Interceptor<VirtualMethodInterceptor>();

            // RegisterType с from = null (либо с одним дженериковым аргументом) добавляет ко всем имеющимся регистрациям переданный в качестве параметра массив InjectionMembers[].
            // В данном случае InjectionMembers[] = { new InterceptionBehavior<PolicyInjectionBehavior>(policyName) }.
            // Альтернативный способ сделать то же самое:
            // interception.Container.Configure<InjectedMembers>().ConfigureInjectionFor<TTargetType>(new InterceptionBehavior<PolicyInjectionBehavior>(policyName));
            interception.Container.RegisterType<TTargetType>(interceptor, new InterceptionBehavior<PolicyInjectionBehavior>(policyName));

            return interception
                .AddPolicy(policyName)
                .AddMatchingRule(new TypeMatchingRule(typeof(TTargetType)))
                .AddMatchingRule(new MethodSignatureMatchingRule(methodInfo.Name, methodInfo.GetParameters().Select(x => x.ParameterType.FullName)))
                .AddCallHandler<THandler>(scope, lifetimeManager, injectionMembersForHandler)
                .Interception;
        }

        /// <summary>
        /// Добавляет обработчик типа <typeparamref name="THandler"/> вызова метода класса <typeparamref name="TTargetType"/>, 
        /// указанного в параметре <paramref name="targetMethodExpression"/>
        /// </summary>
        /// <typeparam name="TTargetType"> Тип, для метода которого генерируется обработчик вызова </typeparam>
        /// <typeparam name="THandler"> Тип обработчика вызова </typeparam>
        /// <param name="interception"> </param>
        /// <param name="targetMethodExpression"> Выражение, содержащее вызов перехватываемого метода </param>
        /// <param name="injectionMembersForHandler"> Параметры конструктора обработчика вызова </param>
        /// <returns></returns>
        public static Interception SetVirtualInterceptorForMethod<TTargetType, THandler>(this Interception interception,
                                                                                         Expression<Action<TTargetType>> targetMethodExpression,
                                                                                         params InjectionMember[] injectionMembersForHandler)
            where THandler : ICallHandler
        {
            return SetVirtualInterceptorForMethod<TTargetType, THandler>(interception,
                                                                         targetMethodExpression,
                                                                         Policy.CommonInterceprion,
                                                                         Mapping.Erm,
                                                                         CustomLifetime.PerRequest,
                                                                         injectionMembersForHandler);
        }
    }
}