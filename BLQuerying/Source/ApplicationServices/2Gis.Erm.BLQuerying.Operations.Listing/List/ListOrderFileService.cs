using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderFileService : ListEntityDtoServiceBase<OrderFile, ListOrderFileDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListOrderFileService(
            IQuery query,
            FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<OrderFile>();

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