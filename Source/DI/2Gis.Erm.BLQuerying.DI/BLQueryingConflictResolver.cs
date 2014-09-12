using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.Operations.Listing;

namespace DoubleGis.Erm.BLQuerying.DI
{
    // ReSharper disable InconsistentNaming
    public class BLQueryingConflictResolver
    // ReSharper restore InconsistentNaming
    {
        private static readonly EntityName[] QdsEntityNames = { EntityName.Order };

        public static Type ListServices(Type operationType, EntitySet entitySet, IEnumerable<Type> candidates)
        {
            if (!typeof(IListEntityService).IsAssignableFrom(operationType))
            {
                return null;
            }

            var businessModel = ConfigFileSetting.Enum.Required<BusinessModel>("BusinessModel").Value;
            if (businessModel == BusinessModel.Russia)
            {
                var entityName = entitySet.Entities.Single();
                if (QdsEntityNames.Contains(entityName))
                {
                    return candidates.Single(x => x.Assembly == typeof(QdsListOrderService).Assembly);
                }
            }

            return candidates.Single(x => x.Assembly != typeof(QdsListOrderService).Assembly);
        }
    }
}