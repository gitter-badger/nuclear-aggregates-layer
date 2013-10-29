﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public interface IOperationScopeContextsStorage
    {
        OperationScopeNode RootScope { get; }

        void Added<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities) where TEntity : class, IEntity;
        void Deleted<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities) where TEntity : class, IEntity;
        void Updated<TEntity>(IOperationScope targetScope, IEnumerable<long> changedEntities) where TEntity : class, IEntity;
    }
}