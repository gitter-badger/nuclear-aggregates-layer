using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel
{
    public static class ActivitySpecs
    {
        public static class Activity
        {
            public static class Find
            {
	            public static FindSpecification<DictionaryEntityInstance> OnlyActivities()
	            {
		            return new FindSpecification<DictionaryEntityInstance>(
			            entity => entity.DictionaryEntityPropertyInstances.Any(
				            property => property.PropertyId == IdentityBase<EntityTypeNameIdentity>.Instance.Id
				                        && (property.NumericValue == (int)EntityName.Appointment
				                            || property.NumericValue == (int)EntityName.Phonecall
				                            || property.NumericValue == (int)EntityName.Task)));
	            }

	            public static FindSpecification<DictionaryEntityInstance> InProgress()
                {
					return new FindSpecification<DictionaryEntityInstance>(
	                    entity => entity.DictionaryEntityPropertyInstances.Any(
		                    property => property.PropertyId == StatusIdentity.Instance.Id && property.NumericValue == (int)ActivityStatus.InProgress));
                }

				public static FindSpecification<DictionaryEntityInstance> RelatedToClient(long clientId)
                {
					return new FindSpecification<DictionaryEntityInstance>(
	                    entity => entity.DictionaryEntityPropertyInstances.Any(
		                    property => property.PropertyId == ClientIdIdentity.Instance.Id && property.NumericValue == clientId));
                }

				public static FindSpecification<DictionaryEntityInstance> RelatedToClient(long clientId,
                                                                             IEnumerable<long> firmIds,
                                                                             IEnumerable<long> contactIds,
                                                                             IEnumerable<long> dealIds)
				{
					return RelatedToClient(clientId) || new FindSpecification<DictionaryEntityInstance>(
	                    entity => entity.DictionaryEntityPropertyInstances.Any(
		                    property => (property.PropertyId == ContactIdIdentity.Instance.Id && property.NumericValue.HasValue && contactIds.Contains((long)property.NumericValue.Value))
		                                || (property.PropertyId == DealIdIdentity.Instance.Id && property.NumericValue.HasValue && dealIds.Contains((long)property.NumericValue.Value))
		                                || (property.PropertyId == FirmIdIdentity.Instance.Id && property.NumericValue.HasValue && firmIds.Contains((long)property.NumericValue.Value))
		                              ));
                }
            }
        }
    }
}