﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBranchOfficeService : ListEntityDtoServiceBase<BranchOffice, ListBranchOfficeDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListBranchOfficeService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListBranchOfficeDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<BranchOffice>();

            return query
                .Where(x => !x.IsDeleted)
                .DefaultFilter(_filterHelper, querySettings)
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
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}