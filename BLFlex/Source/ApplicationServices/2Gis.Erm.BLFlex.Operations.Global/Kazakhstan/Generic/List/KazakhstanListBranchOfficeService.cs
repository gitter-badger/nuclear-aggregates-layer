using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Kazakhstan.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.List
{
    public class KazakhstanListBranchOfficeService : ListEntityDtoServiceBase<BranchOffice, KazakhstanListBranchOfficeDto>, IKazakhstanAdapted
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public KazakhstanListBranchOfficeService(
            IQuery query,
            FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            return _query.For<BranchOffice>()
                          .Select(x => new KazakhstanListBranchOfficeDto
                                           {
                                               Id = x.Id,
                                               ContributionTypeId = x.ContributionTypeId,
                                               ContributionType = x.ContributionType.Name,
                                               BargainTypeId = x.BargainTypeId,
                                               BargainType = x.BargainType.Name,
                                               Name = x.Name,
                                               LegalAddress = x.LegalAddress,
                                               BinIin = x.Inn,
                                               Annotation = x.Annotation,
                                               IsActive = x.IsActive,
                                               IsDeleted = x.IsDeleted
                                           })
                          .QuerySettings(_filterHelper, querySettings);
        }
    }
}
