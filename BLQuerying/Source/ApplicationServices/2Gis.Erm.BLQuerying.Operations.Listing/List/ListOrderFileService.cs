using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderFileService : ListEntityDtoServiceBase<OrderFile, ListOrderFileDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListOrderFileService(
            IFinder finder,
            FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<OrderFile>();

            return query
                .Where(x => !x.IsDeleted)
                .Select(x => new ListOrderFileDto
                {
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    FileId = x.FileId,
                    FileName = x.File.FileName,
                    OrderId = x.OrderId,
                    IsDeleted = x.IsDeleted,
                    FileKind = x.FileKind.ToStringLocalizedExpression(),
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}