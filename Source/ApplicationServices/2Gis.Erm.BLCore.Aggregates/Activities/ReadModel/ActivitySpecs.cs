using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Activities.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel
{
    public static class ActivitySpecs
    {
        public static class Activity
        {
            public static class Find
            {
                public static FindSpecification<ActivityDto> InProgress()
                {
                    return new FindSpecification<ActivityDto>(x => x.Status == ActivityStatus.InProgress);
                }

                public static FindSpecification<ActivityDto> RelatedToClient(long clientId,
                                                                             IEnumerable<long> firmIds,
                                                                             IEnumerable<long> contactIds,
                                                                             IEnumerable<long> dealIds)
                {
                    var relatedToClient = new FindSpecification<ActivityDto>(x => x.ClientId == clientId ||
                                                                                  (x.FirmId.HasValue && firmIds.Contains(x.FirmId.Value)) ||
                                                                                  (x.ContactId.HasValue && contactIds.Contains(x.ContactId.Value)) ||
                                                                                  (x.DealId.HasValue && dealIds.Contains((long)x.DealId)));

                    return InProgress() && relatedToClient;
                }
            }

            public static class Select
            {
                public static ISelectSpecification<ActivityInstance, ActivityDto> ActivityDto()
                {
                    return new SelectSpecification<ActivityInstance, ActivityDto>(x => new ActivityDto
                        {
                            Id = x.Id,
                            ClientId = x.ClientId,
                            ContactId = x.ContactId,
                            FirmId = x.FirmId,
                            DealId = (long?)x.ActivityPropertyInstances
                                             .Where(y => y.PropertyId == DealIdIdentity.Instance.Id)
                                             .Select(y => y.NumericValue)
                                             .FirstOrDefault(),
                            Status = (ActivityStatus)(int)x.ActivityPropertyInstances
                                                      .Where(y => y.PropertyId == StatusIdentity.Instance.Id)
                                                      .Select(y => y.NumericValue)
                                                      .FirstOrDefault(),
                        });
                }

                public static ISelectSpecification<ActivityInstance, ActivityInstanceDto> ActivityInstanceDto()
                {
                    return new SelectSpecification<ActivityInstance, ActivityInstanceDto>(x => new ActivityInstanceDto
                        {
                            ActivityInstance = x,
                            ActivityPropretyInstances = x.ActivityPropertyInstances
                        });
                }
            }
        }
    }
}