using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementElementDenialReasonService :
        ListEntityDtoServiceBase<AdvertisementElementDenialReason, ListAdvertisementElementDenialReasonsDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAdvertisementElementDenialReasonService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
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
            var query = _finder.FindAll<AdvertisementElementDenialReason>();

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

            var checkedReasons = _finder.FindAll<AdvertisementElementDenialReason>().Where(x => x.AdvertisementElementId == advertisementElementId);
            return _finder.FindAll<DenialReason>()
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