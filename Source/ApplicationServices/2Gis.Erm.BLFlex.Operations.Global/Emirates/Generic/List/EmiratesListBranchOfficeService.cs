﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.List
{
    public class EmiratesListBranchOfficeService : ListEntityDtoServiceBase<BranchOffice, EmiratesListBranchOfficeDto>,
                                                             IEmiratesAdapted
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public EmiratesListBranchOfficeService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<EmiratesListBranchOfficeDto> List(
            QuerySettings querySettings,
            out int count)
        {
            var query = _finder.FindAll<BranchOffice>();

            return
                query.Select(x =>
                             new EmiratesListBranchOfficeDto
                                 {
                                     Id = x.Id,
                                     ContributionTypeId = x.ContributionTypeId,
                                     ContributionType = x.ContributionType.Name,
                                     BargainTypeId = x.BargainTypeId,
                                     BargainType = x.BargainType.Name,
                                     Name = x.Name,
                                     LegalAddress = x.LegalAddress,
                                     CommercialLicense = x.Inn,
                                     Annotation = x.Annotation,
                                     IsActive = x.IsActive,
                                     IsDeleted = x.IsDeleted
                                 })
                     .QuerySettings(_filterHelper, querySettings, out count)
                     .ToArray();
        }
    }
}
