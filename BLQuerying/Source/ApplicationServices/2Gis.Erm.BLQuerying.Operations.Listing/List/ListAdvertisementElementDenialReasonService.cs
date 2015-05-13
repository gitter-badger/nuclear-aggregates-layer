using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementElementDenialReasonService :
        ListEntityDtoServiceBase<AdvertisementElementDenialReason, ListAdvertisementElementDenialReasonsDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementElementDenialReasonService(
            IQuery query,
            FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            if (querySettings.ParentEntityName.Equals(EntityType.Instance.AdvertisementElement()))
            {
                return ListCheckedReasons(querySettings);
            }

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.AdvertisementElementStatus()))
            {
                return ListPossibleReasons(querySettings);
            }

            return new RemoteCollection<object>(new object[0], 0);
        }

        private IRemoteCollection ListCheckedReasons(QuerySettings querySettings)
        {
            var query = _query.For<AdvertisementElementDenialReason>();

            var result = query
                .Select(x => new ListAdvertisementElementDenialReasonsDto
                    {
                        Id = x.Id,
                        AdvertisementElementId = x.AdvertisementElementId,
                        DenialReasonId = x.DenialReasonId,
                        DenialReasonName = x.DenialReason.Name,
                        Comment = x.Comment,
                        Checked = true,
                        IsActive = x.DenialReason.IsActive,
                        DenialReasonType = x.DenialReason.Type.ToStringLocalizedExpression(),
                    })
                .QuerySettings(_filterHelper, querySettings);

            return result;
        }

        private IRemoteCollection ListPossibleReasons(QuerySettings querySettings)
        {
            if (!querySettings.ParentEntityId.HasValue)
            {
                throw new ArgumentException("ParentEntityId must be specified", "querySettings");
            }

            var advertisementElementId = querySettings.ParentEntityId.Value;

            var checkedReasons = _query.For<AdvertisementElementDenialReason>().Where(x => x.AdvertisementElementId == advertisementElementId);
            return _query.For<DenialReason>()
                          .GroupJoin(checkedReasons, dr => dr.Id, aedr => aedr.DenialReasonId, (dr, aedr) => new { dr, aedr = aedr.FirstOrDefault() })
                          .Select(x => new ListAdvertisementElementDenialReasonsDto
                              {
                                  Id = x.aedr != null ? x.aedr.Id : 0,
                                  AdvertisementElementId = advertisementElementId,
                                  DenialReasonId = x.dr.Id,
                                  DenialReasonName = x.dr.Name,
                                  Comment = x.aedr != null ? x.aedr.Comment : string.Empty,
                                  Checked = x.aedr != null,
                                  IsActive = x.dr.IsActive,
                                  DenialReasonType = (x.dr.Type).ToStringLocalizedExpression(),
                              })
                          .QuerySettings(_filterHelper, querySettings);
        }
    }
}