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
    public sealed class PhonecallObtainer : IBusinessModelEntityObtainer<Phonecall>, IAggregateReadModel<Phonecall>
    {
        private readonly IFinder _finder;

        public PhonecallObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Phonecall ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PhonecallDomainEntityDto)domainEntityDto;

            // FIXME {s.pomadin, 27.10.2014}: См изменения в AppointmentObtainer
            var phoneCall = dto.IsNew() 
                ? new Phonecall
                    {
                        CreatedBy = dto.CreatedByRef.GetId(),
                        CreatedOn = dto.CreatedOn,
                        ModifiedBy = dto.ModifiedByRef.GetId(),
                        ModifiedOn = dto.ModifiedOn,
                        Timestamp = dto.Timestamp,
                    } 
                : _finder.FindOne(Specs.Find.ById<Phonecall>(dto.Id));

            phoneCall.Header = dto.Header;
            phoneCall.Description = dto.Description;
            phoneCall.Priority = dto.Priority;
            phoneCall.Purpose = dto.Purpose;
            phoneCall.ScheduledOn = dto.ScheduledOn;
            phoneCall.Status = dto.Status;
            phoneCall.OwnerCode = dto.OwnerRef.GetId();

            return phoneCall;
        }
    }
}
