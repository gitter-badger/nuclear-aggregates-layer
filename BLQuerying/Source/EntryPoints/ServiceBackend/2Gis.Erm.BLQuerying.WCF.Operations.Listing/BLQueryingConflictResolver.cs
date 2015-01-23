﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Qds.Operations.Listing;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLQuerying.WCF.Operations.Listing
{
    // ReSharper disable InconsistentNaming
    public static class BLQueryingConflictResolver
    // ReSharper restore InconsistentNaming
    {
        private static readonly IEntityType[] QdsEntityNames =
            {
                EntityType.Instance.Order(),
                EntityType.Instance.Client(),
                EntityType.Instance.Firm()
            };

        public static Type ListServices(Type operationType, EntitySet entitySet, IEnumerable<Type> candidates)
        {
            if (!typeof(IListEntityService).IsAssignableFrom(operationType))
            {
                return null;
            }

            // настройка для экстренного выключения Elasticsearch
            var enableElasticsearch = ConfigFileSetting.Bool.Required("EnableElasticsearch").Value;
            if (!enableElasticsearch)
            {
                return candidates.Single(x => x.Assembly != typeof(QdsListOrderService).Assembly);
            }

            var entityName = entitySet.Entities.Single();
            if (QdsEntityNames.Contains(entityName))
            {
                return candidates.Single(x => x.Assembly == typeof(QdsListOrderService).Assembly);
            }

            return candidates.Single(x => x.Assembly != typeof(QdsListOrderService).Assembly);
        }
    }
}