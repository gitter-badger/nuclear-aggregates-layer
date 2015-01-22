using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class BargainFileObtainer : IBusinessModelEntityObtainer<BargainFile>, IAggregateReadModel<Bargain>
    {
        private readonly IFinder _finder;

        public BargainFileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public BargainFile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (BargainFileDomainEntityDto)domainEntityDto;

            var entity = _finder.FindOne(Specs.Find.ById<BargainFile>(dto.Id)) 
                ?? new BargainFile { IsActive = true };
            
            entity.FileKind = dto.FileKind;
            entity.FileId = dto.FileId;
            entity.BargainId = dto.BargainRef.Id.Value;
            entity.Comment = dto.Comment;
            entity.Timestamp = dto.Timestamp;
            
            return entity;
        }
    }
}