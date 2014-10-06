using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Metadata.Crm;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions
{
    using CrmActivityPartyMetadata = Metadata.Crm.ActivityParty;
    using ErmEntityName = Metadata.Erm.EntityName;

    internal static class CrmSdkExtensions
    {
		public static IEnumerable<DynamicEntity> LoadEntities(this ICrmService service, QueryExpression query, int? pageSize = null)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			if (query == null)
				throw new ArgumentNullException("query");

			var doPaging = (query.PageInfo == null || query.PageInfo.Count <= 0) && pageSize.HasValue;

			if (doPaging)
			{
				query.PageInfo = new PagingInfo { PageNumber = 1, Count = pageSize.Value };
			}

			BusinessEntityCollection response = null;
			do
			{
				if (response != null && query.PageInfo != null)
				{
					query.PageInfo.PageNumber++;
					query.PageInfo.PagingCookie = response.PagingCookie;
				}

				response = service.RetrieveMultiple(query);
				if (response == null || response.BusinessEntities == null)
					yield break;

				foreach (var entity in response.BusinessEntities.Cast<DynamicEntity>())
					yield return entity;
			}
			while (response.MoreRecords && doPaging);
		}

        public static IEnumerable<CrmReference> EnumerateActivityReferences(this IEnumerable<DynamicEntity> entities)
        {
            return (entities ?? Enumerable.Empty<DynamicEntity>())
                .Select(x => x.Value(ActivityParty.PartyId))
                .OfType<CrmReference>()
                .Where(x => !x.IsNull);
        }

        public static IEnumerable<CrmReference> FilterByEntityName(this IEnumerable<CrmReference> references, params ErmEntityName[] entityNames)
        {
            var entitiesToAccept = new HashSet<ErmEntityName>(entityNames);

            return (references ?? Enumerable.Empty<CrmReference>())
                .Where(x => x != null && !x.IsNull)
                .Select(x => new { EntityName = x.type.Map(EntityNameExtensions.ToEntityName), Reference = x })
                .Where(x => entitiesToAccept.Contains(x.EntityName))
                .Select(x => x.Reference);
        }

        public static ActivityReference ToReferenceWithin(this CrmReference crmReference, IActivityMigrationContextExtended context)
        {
            return new ActivityReference(crmReference.type.Map(EntityNameExtensions.ToEntityName), context.Parse<long>(crmReference));
        }

	    public static object Value(this DynamicEntity entity, string propertyName)
	    {
		    return entity.Properties.Contains(propertyName) ? entity[propertyName] : null;
	    }
    }
}