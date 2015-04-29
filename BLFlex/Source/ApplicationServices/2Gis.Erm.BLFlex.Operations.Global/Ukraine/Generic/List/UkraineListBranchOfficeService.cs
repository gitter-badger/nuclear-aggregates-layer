using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Ukraine.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.List
{
    public class UkraineListBranchOfficeService : ListEntityDtoServiceBase<BranchOffice, UkraineListBranchOfficeDto>, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public UkraineListBranchOfficeService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<BranchOffice>();

            var dynamicObjectsQuery = _finder.For<BusinessEntityInstance>().Select(x => new
                {
                    Instance = x,
                    x.BusinessEntityPropertyInstances
                });

            return
                query.Join(dynamicObjectsQuery,
                          x => x.Id,
                          y => y.Instance.EntityId,
                          (x, y) =>
                          new UkraineListBranchOfficeDto
                              {
                                  Id = x.Id,
                                  ContributionTypeId = x.ContributionTypeId,
                                  ContributionType = x.ContributionType.Name,
                                  BargainTypeId = x.BargainTypeId,
                                  BargainType = x.BargainType.Name,
                                  Name = x.Name,
                                  LegalAddress = x.LegalAddress,
                                  Ipn = y.BusinessEntityPropertyInstances.FirstOrDefault(z => z.PropertyId == IpnIdentity.Instance.Id).TextValue,
                                  Egrpou = x.Inn,
                                  Annotation = x.Annotation,
                                  IsActive = x.IsActive,
                                  IsDeleted = x.IsDeleted
                              })
                    .QuerySettings(_filterHelper, querySettings);
        }
    }
}
