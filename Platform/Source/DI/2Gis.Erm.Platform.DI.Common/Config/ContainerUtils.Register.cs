using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.DI.Common.Extensions;

using Microsoft.Practices.Unity;

using NuClear.DI.Unity.Config.RegistrationResolvers;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public delegate bool ParameterResolver(IUnityContainer container, Type type, string targetNamedMapping, ParameterInfo constructorParameter, out object resolvedParameter);

    public static partial class ContainerUtils
    {
        public static IUnityContainer RegisterOne2ManyTypesPerCallUniqueness(this IUnityContainer container, Type tFrom, Type tTo, LifetimeManager lifetimeManager)
        {
            if (!tFrom.CanMapTo(tTo))
            {
                throw new InvalidOperationException(string.Format("Type {0} is not assignable from {1}", tFrom, tTo));
            }

            return container.RegisterTypeWithDependencies(tFrom, tTo, GetPerCallUniqueMarker(), lifetimeManager, (string)null);
        }

        public static IUnityContainer RegisterOne2ManyTypesPerCallUniqueness<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager) where TTo : TFrom
        {
            return container.RegisterOne2ManyTypesPerCallUniqueness(typeof(TFrom), typeof(TTo), lifetimeManager);
        }
        
        public static IUnityContainer RegisterOne2ManyTypesPerTypeUniqueness(this IUnityContainer container, Type tFrom, Type tTo, LifetimeManager lifetimeManager)
        {
            if (!tFrom.CanMapTo(tTo))
            {
                throw new InvalidOperationException(string.Format("Type {0} is not assignable from {1}", tFrom, tTo));
            }

            return container.RegisterTypeWithDependencies(tFrom, tTo, tTo.GetPerTypeUniqueMarker(), lifetimeManager, (string)null);
        }

        public static IUnityContainer RegisterOne2ManyTypesPerTypeUniqueness<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager) where TTo : TFrom
        {
            return container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(TFrom), typeof(TTo), lifetimeManager);
        }

        public static IUnityContainer RegisterTypeWithDependencies<TFrom, TTo>(this IUnityContainer container, string name, LifetimeManager lifetimeManager, string dependenciesScope) where TTo : TFrom
        {
            var registeringTargetType = typeof(TTo);
            var resolveDependencies = GetResolveSettingsForTypeDependencies(container, registeringTargetType, dependenciesScope);
            return container.RegisterType(typeof(TFrom), typeof(TTo), name, lifetimeManager, new InjectionConstructorEx(resolveDependencies));
        }

        public static IUnityContainer RegisterTypeWithDependencies<TFrom, TTo>(this IUnityContainer container, LifetimeManager lifetimeManager, string dependenciesScope) where TTo : TFrom
        {
            return container.RegisterTypeWithDependencies<TFrom, TTo>(null, lifetimeManager, dependenciesScope);
        }

        public static IUnityContainer RegisterTypeWithDependencies<TFrom, TTo>(this IUnityContainer container, string name, LifetimeManager lifetimeManager) where TTo : TFrom
        {
            return container.RegisterTypeWithDependencies<TFrom, TTo>(name, lifetimeManager, name);
        }

        public static IUnityContainer RegisterTypeWithDependencies<TFrom, TTo>(this IUnityContainer container, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
            where TTo : TFrom
        {
            return container.RegisterTypeWithDependencies(typeof(TFrom), typeof(TTo), name, lifetimeManager, name, injectionMembers);
        }

        public static IUnityContainer RegisterTypeWithDependencies(this IUnityContainer container, Type t, string name, LifetimeManager lifetimeManager)
        {
            return container.RegisterTypeWithDependencies(null, t, name, lifetimeManager, name);
        }

        public static IUnityContainer RegisterTypeWithDependencies(this IUnityContainer container, Type t, LifetimeManager lifetimeManager, string dependenciesScope)
        {
            return container.RegisterTypeWithDependencies(null, t, null, lifetimeManager, dependenciesScope);
        }

        public static IUnityContainer RegisterTypeWithDependencies(this IUnityContainer container, Type t, string name, LifetimeManager lifetimeManager, string dependenciesScope)
        {
            return container.RegisterTypeWithDependencies(null, t, name, lifetimeManager, dependenciesScope);
        }

        public static IUnityContainer RegisterTypeWithDependencies(this IUnityContainer container, Type from, Type to, LifetimeManager lifetimeManager, string dependenciesScope, params InjectionMember[] injectionMembers)
        {
            return container.RegisterTypeWithDependencies(from, to, null, lifetimeManager, dependenciesScope, injectionMembers);
        }
        
        public static IUnityContainer RegisterTypeWithDependencies(this IUnityContainer container, Type from, Type to, string name, LifetimeManager lifetimeManager, params InjectionMember[] injectionMembers)
        {
            return container.RegisterTypeWithDependencies(from, to, name, lifetimeManager, name, injectionMembers);
        }

        public static IUnityContainer RegisterTypeWithDependencies(this IUnityContainer container, Type from, Type to, string name, LifetimeManager lifetimeManager, string dependenciesScope, params InjectionMember[] injectionMembers)
        {
            var resolveDependencies = GetResolveSettingsForTypeDependencies(container, to, dependenciesScope);

            var members = injectionMembers.Concat(new[] { new InjectionConstructorEx(resolveDependencies) }).ToArray();
            return container.RegisterType(from, to, name, lifetimeManager, members);
        }

        /// <summary>
        /// Получает для указанного типа целевой конструктор - единственный (экземплярный и публичный)
        /// В случае отклонений бросает exception
        /// </summary>
        public static ConstructorInfo GetTargetConstructor(this Type targetType)
        {
            var constructors = targetType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException("Type: " + targetType + ". Hasn't default constructor or explicitly declared public constructor");
            }

            if (constructors.Length > 1)
            {
                throw new InvalidOperationException("Type: " + targetType + ". Has more than one public constructor");
            }

            return constructors.Single();
        }

        /// <summary>
        /// Создает дочерний контейнер и регистрирует в нем как singleton указанные зависимости, зарезолвленные из головного контейнера
        /// </summary>
        public static IUnityContainer CreateChildContainerWithParentDependencies(this IUnityContainer container, params Type[] dependencies)
        {
            var childContainer = container.CreateChildContainer();
            foreach (var dependency in dependencies)
            {
                childContainer.RegisterInstance(dependency, container.Resolve(dependency));
            }

            return childContainer;
        }

        private static IEnumerable<object> GetResolveSettingsForTypeDependencies(
            IUnityContainer container,
            Type type,
            string targetNamedMapping)
        {
            var paramsList = new List<object>();
            var diConstructor = type.GetTargetConstructor();
            var constructorParameters = diConstructor.GetParameters();
            foreach (var constructorParameter in constructorParameters)
            {   // resolver используем в порядке приоритета, от последних добавленных, к базовым - своего рода chain of responsibility
                object resolvedParamameter = null;

                var parameterResolvers = ParameterResolvers.Defaults.ToList();
                for (int i = parameterResolvers.Count - 1; i >= 0; i--)
                {
                    if (parameterResolvers[i](container, type, targetNamedMapping, constructorParameter, out resolvedParamameter))
                    {
                        paramsList.Add(resolvedParamameter);
                        break;
                    }
                }

                if (resolvedParamameter == null)
                {
                    throw new InvalidOperationException("Can't resolve constructor parameter scope. Processing type: " + type.FullName + ". Constructor parameter: " + constructorParameter.Name);
                }
            }

            return paramsList.ToArray();
        }
    }
}
