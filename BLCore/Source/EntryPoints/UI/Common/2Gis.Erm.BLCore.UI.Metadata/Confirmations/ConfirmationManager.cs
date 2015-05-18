using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Confirmations
{
    public static class ConfirmationManager
    {
        private static readonly IReadOnlyDictionary<IOperationIdentity, Func<string>> DefaultOperationToConfirmationMap = new Dictionary<IOperationIdentity, Func<string>>
            {
                { DeleteIdentity.Instance, () => BLResources.DeleteConfirmation },
                { ActivateIdentity.Instance, () => BLResources.ActivateConfirmation },
                { DeactivateIdentity.Instance, () => BLResources.DeactivateConfirmation }
            };

        private static readonly IReadOnlyDictionary<StrictOperationIdentity, Func<string>> OperationToConfirmationMap = new Dictionary<StrictOperationIdentity, Func<string>>
            {
                { new StrictOperationIdentity(DeleteIdentity.Instance, new EntitySet(EntityType.Instance.CategoryGroup())), () => BLResources.DeleteCategoryGroupConfirmation }
            };


        public static string GetConfirmation(StrictOperationIdentity operationIdentity)
        {
            Func<string> accessor;
            if (OperationToConfirmationMap.TryGetValue(operationIdentity, out accessor))
            {
                return accessor();
            }

            return DefaultOperationToConfirmationMap.TryGetValue(operationIdentity.OperationIdentity, out accessor) ? accessor() : null;
        }
    }
}