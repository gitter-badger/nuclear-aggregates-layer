using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Operations
{
    public sealed class OperationsRegistry : IOperationsRegistry
    {
        private readonly IReadOnlyDictionary<Type, Dictionary<EntitySet, Type>> _entitySpecificOperations;
        private readonly IReadOnlyDictionary<Type, Type> _notCoupledOperations;
        private readonly IReadOnlyDictionary<Type, IOperationIdentity> _operations2IdentitiesMap;

        public OperationsRegistry(
            IReadOnlyDictionary<Type, Dictionary<EntitySet, Type>> entitySpecificOperations,
            IReadOnlyDictionary<Type, Type> notCoupledOperations,
            IReadOnlyDictionary<Type, IOperationIdentity> operations2IdentitiesMap)
        {
            _entitySpecificOperations = entitySpecificOperations;
            _notCoupledOperations = notCoupledOperations;
            _operations2IdentitiesMap = operations2IdentitiesMap;
        }

        public IReadOnlyDictionary<Type, Dictionary<EntitySet, Type>> EntitySpecificOperations
        {
            get { return _entitySpecificOperations; }
        }

        public IReadOnlyDictionary<Type, Type> NotCoupledOperations
        {
            get { return _notCoupledOperations; }
        }

        public IReadOnlyDictionary<Type, IOperationIdentity> Operations2IdentitiesMap
        {
            get { return _operations2IdentitiesMap; }
        }

        public bool TryGetEntitySpecificOperation<TOperation>(EntitySet descriptor, out Type resolvedImplementation, out EntitySet resolvedOperationDescriptor)
            where TOperation : class, IOperation
        {
            resolvedImplementation = null;
            var defaultDescriptor = EntitySet.Create.GenericEntitySpecific;
            resolvedOperationDescriptor = defaultDescriptor;
            var operationType = typeof(TOperation);

            Dictionary<EntitySet, Type> implementationsList;
            if (!EntitySpecificOperations.TryGetValue(operationType, out implementationsList))
            {
                return false;
            }

            Type implementationType;
            if (implementationsList.TryGetValue(descriptor, out implementationType) && implementationType != null)
            {
                resolvedImplementation = implementationType;
                resolvedOperationDescriptor = descriptor;
                return true;
            }

            if (implementationsList.TryGetValue(defaultDescriptor, out implementationType) && implementationType != null)
            {
                resolvedImplementation = implementationType;
                resolvedOperationDescriptor = defaultDescriptor;
                return true;
            }

            return false;
        }

        public bool TryGetNotCoupledOperation<TOperation>(out Type resolvedImplementation)
            where TOperation : class, IOperation
        {
            resolvedImplementation = null;
            var operationType = typeof(TOperation);

            Type implementationType;
            if (NotCoupledOperations.TryGetValue(operationType, out implementationType) && implementationType != null)
            {
                resolvedImplementation = implementationType;
                return true;
            }

            return false;
        }
    }
}