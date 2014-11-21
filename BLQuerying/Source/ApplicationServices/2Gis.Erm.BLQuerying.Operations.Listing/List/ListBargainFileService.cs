﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListBargainFileService : ListEntityDtoServiceBase<BargainFile, ListBargainFileDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListBargainFileService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<BargainFile>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListBargainFileDto
                {
                    Id = x.Id,
                    FileId = x.FileId,
                    FileName = x.File.FileName,
                    CreatedOn = x.CreatedOn,
                    BargainId = x.BargainId,
                    IsDeleted = x.IsDeleted,
                    FileKind = ((BargainFileKind)x.FileKind).ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}