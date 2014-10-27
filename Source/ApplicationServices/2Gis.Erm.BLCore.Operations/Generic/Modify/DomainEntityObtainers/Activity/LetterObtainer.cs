using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

            // FIXME {s.pomadin, 27.10.2014}: См изменения в AppointmentObtainer
            var letter = dto.IsNew() 
                ? new Letter
                    {
                        CreatedBy = dto.CreatedByRef.GetId(),
                        CreatedOn = dto.CreatedOn,
                        ModifiedBy = dto.ModifiedByRef.GetId(),
                        ModifiedOn = dto.ModifiedOn,
                        Timestamp = dto.Timestamp,
                    } 
                : _finder.FindOne(Specs.Find.ById<Letter>(dto.Id));

            letter.Header = dto.Header;
            letter.Description = dto.Description;
            letter.Priority = dto.Priority;
            letter.ScheduledOn = dto.ScheduledOn;
            letter.Status = dto.Status;
            letter.OwnerCode = dto.OwnerRef.GetId();

            return letter;
        }
    }
}
