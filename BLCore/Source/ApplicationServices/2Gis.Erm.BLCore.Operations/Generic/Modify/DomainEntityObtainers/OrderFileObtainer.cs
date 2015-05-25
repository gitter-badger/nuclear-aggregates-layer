using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class OrderFileObtainer : IBusinessModelEntityObtainer<OrderFile>, IAggregateReadModel<Order>
    {
        private readonly IFinder _finder;

        public OrderFileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public OrderFile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrderFileDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<OrderFile>(dto.Id)).SingleOrDefault() ??
                             new OrderFile { IsActive = true };

            entity.OrderId = dto.OrderId;
            entity.FileId = dto.FileId;
            entity.FileKind = dto.FileKind;
            entity.Comment = dto.Comment;
            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}