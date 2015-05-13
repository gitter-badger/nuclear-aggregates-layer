using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.List
{
    public sealed class MultiCultureListBranchOfficeService : ListEntityDtoServiceBase<BranchOffice, ListBranchOfficeDto>, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IChileAdapted
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public MultiCultureListBranchOfficeService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<BranchOffice>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListBranchOfficeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Annotation = x.Annotation,
                    Rut = x.Inn,
                    Ic = x.Ic,
                    Inn = x.Inn,
                    BargainTypeId = x.BargainTypeId,
                    BargainType = x.BargainType.Name,
                    ContributionTypeId = x.ContributionTypeId,
                    ContributionType = x.ContributionType.Name,
                    LegalAddress = x.LegalAddress,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}