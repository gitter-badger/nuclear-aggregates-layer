using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class LetterObtainer : IBusinessModelEntityObtainer<Letter>, IAggregateReadModel<Letter>
    {
        private readonly IFinder _finder;

        public LetterObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Letter ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LetterDomainEntityDto)domainEntityDto;

            var letter = dto.IsNew() 
                ? new Letter { IsActive = true, Status = dto.Status, OwnerCode = dto.OwnerRef.GetId() }
                : _finder.Find(Specs.Find.ById<Letter>(dto.Id)).One();

            letter.Header = dto.Header;
            letter.Description = dto.Description;
            letter.Priority = dto.Priority;
            letter.ScheduledOn = dto.ScheduledOn;
            letter.Timestamp = dto.Timestamp;

            return letter;
        }
    }
}
