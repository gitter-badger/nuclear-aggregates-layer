using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeClient
{
    public class ChangeGenericEntityClientService<TEntity> : IChangeGenericEntityClientService<TEntity> where TEntity : class, IEntityKey
    {
        private const string GenericImplementationInvalid = "Change client operation is supported by Firm, LegalPerson and Deal only";
        public ChangeEntityClientResult Execute(long entityId, long clientId, bool bypassValidation)
        {
            throw new NotSupportedException(GenericImplementationInvalid);
        }

        public ChangeEntityClientValidationResult Validate(long entityId, long clientId)
        {
            throw new NotSupportedException(GenericImplementationInvalid);
        }
    }
}