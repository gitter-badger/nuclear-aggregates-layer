using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Old
{
    [Obsolete("Use IOwnerValidator")]
    public sealed class ValidateOwnerIsNotReserveRequest<TEntity> : Request
        where TEntity : IEntityKey, ICuratedEntity
    {
        public long Id { get; set; }
    }
}