﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    public interface IOperationsRegistry
    {
        IReadOnlyDictionary<Type, Dictionary<EntitySet, Type>> EntitySpecificOperations { get; }
        IReadOnlyDictionary<Type, Type> NotCoupledOperations { get; }
        IReadOnlyDictionary<Type, IOperationIdentity> Operations2IdentitiesMap { get; }

        bool TryGetEntitySpecificOperation<TOperation>(EntitySet descriptor, out Type resolvedImplementation, out EntitySet resolvedOperationDescriptor)
            where TOperation : class, IOperation;

        bool TryGetNotCoupledOperation<TOperation>(out Type resolvedImplementation)
            where TOperation : class, IOperation;
    }
}
