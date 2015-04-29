using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Aggregates
{
    public static partial class Indicators
    {
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
        public static class Aggregates
        {
            public static readonly Type Repository = typeof(IAggregateRepository);
            public static readonly Type ReadModel = typeof(IAggregateReadModel);
            public static readonly Type ReadModelGeneric = typeof(IAggregateReadModel<>);
            public static readonly Type RepositoryRoot = typeof(IAggregateRootRepository<>);
            public static readonly Type UnknownAggregatePart = typeof(IUnknownAggregatePartRepository);
            public static readonly Type RepositoryPart = typeof(IAggregatePartRepository<>);

            internal static IEnumerable<Type> ExtractArgsForIndicator(Type checkingType, Type checkingIndicator)
            {
                if (!checkingIndicator.GetTypeInfo().IsGenericType || !checkingIndicator.GetTypeInfo().IsGenericTypeDefinition)
                {
                    throw new InvalidOperationException("Only open generic indicators must be specified");
                }

                var targetGenericTypeParameters = checkingIndicator.GenericTypeArguments;
                var resolvedIndicatorUsings = checkingType.GetTypeInfo().ImplementedInterfaces
                                                          .Where(t => t.GetTypeInfo().IsGenericType && checkingIndicator == t.GetGenericTypeDefinition())
                                                          .Select(t => t.GenericTypeArguments)
                                                          .ToArray();

                if (resolvedIndicatorUsings.Length == 0)
                {
                    return new Type[0];
                }

                var firstTypeArgumentsBucket = resolvedIndicatorUsings.First();
                if (firstTypeArgumentsBucket.Length != targetGenericTypeParameters.Length)
                {
                    throw new InvalidOperationException("Type arguments count is not equal to indicator type parameters count");
                }

                if (resolvedIndicatorUsings.Length == 1)
                {
                    return firstTypeArgumentsBucket;
                }

                for (int i = 1; i < resolvedIndicatorUsings.Length; i++)
                {
                    var checkingResolvedIndicatorUsings = resolvedIndicatorUsings[i];
                    if (checkingResolvedIndicatorUsings.Length != targetGenericTypeParameters.Length)
                    {
                        throw new InvalidOperationException("Type arguments count is not equal to indicator type parameters count");
                    }

                    for (int j = 0; j < checkingResolvedIndicatorUsings.Length; j++)
                    {
                        if (checkingResolvedIndicatorUsings[j] != firstTypeArgumentsBucket[j])
                        {
                            throw new InvalidOperationException("Indicator has several usings with different type arguments");
                        }
                    }
                }

                return firstTypeArgumentsBucket;
            }

            /// <summary>
            /// Группы индикаторов
            /// </summary>
            public static class Group
            {
                /// <summary>
                /// Все маркерные интерфейсы для слоя агрегатов
                /// </summary>
                public static readonly Type[] All =
                    {
                        Repository,
                        ReadModel,
                        ReadModelGeneric,
                        RepositoryRoot,
                        UnknownAggregatePart,
                        RepositoryPart,
                    };

                /// <summary>
                /// Маркерные интерфейсы для слоя агрегатов - updatable части
                /// </summary>
                public static readonly Type[] Repositories =
                    {
                        Repository,
                        RepositoryRoot,
                        UnknownAggregatePart,
                        RepositoryPart
                    };

                /// <summary>
                /// Маркерные интерфейсы для слоя агрегатов - readonly части
                /// </summary>
                public static readonly Type[] ReadOnly =
                    {
                        ReadModel,
                        ReadModelGeneric
                    };
            }
        }

        public static bool IsAggregateRepository(this Type checkingType)
        {
            return Aggregates.Repository.GetTypeInfo().IsAssignableFrom(checkingType.GetTypeInfo());
        }

        public static bool IsUnknownAggregatePart(this Type checkingType)
        {
            return Aggregates.UnknownAggregatePart.GetTypeInfo().IsAssignableFrom(checkingType.GetTypeInfo());
        }

        public static bool IsAggregateReadModel(this Type checkingType)
        {
            return Aggregates.ReadModel.GetTypeInfo().IsAssignableFrom(checkingType.GetTypeInfo());
        }

        public static Type ResolveAggregateRoot(this Type checkingType)
        {
            return new[] { AggregateForRepositoryRoot(checkingType), AggregateForRepositoryPart(checkingType) }.Where(t => t != null).Distinct().Single();
        }

        public static Type AggregateForReadModel(this Type checkingType)
        {
            return Aggregates.ExtractArgsForIndicator(checkingType, Aggregates.ReadModelGeneric).SingleOrDefault();
        }

        public static Type AggregateForRepositoryRoot(this Type checkingType)
        {
            return Aggregates.ExtractArgsForIndicator(checkingType, Aggregates.RepositoryRoot).SingleOrDefault();
        }

        public static Type AggregateForRepositoryPart(this Type checkingType)
        {
            return Aggregates.ExtractArgsForIndicator(checkingType, Aggregates.RepositoryPart).SingleOrDefault();
        }

        public static IEnumerable<Type> GetAggregateParts(this Type checkingType, Type aggregateRoot)
        {
            var entityTypeInfo = typeof(IEntity).GetTypeInfo();
            var genericArguments = checkingType.GetTypeInfo().ImplementedInterfaces
                                               .Where(t => t.GetTypeInfo().IsGenericType)
                                               .SelectMany(t => t.GenericTypeArguments)
                                               .Where(x => x.GetTypeInfo().IsAssignableFrom(entityTypeInfo))
                                               .Distinct()
                                               .ToArray();
            return genericArguments.Except(new[] { aggregateRoot }).ToArray();
        }
    }
}
