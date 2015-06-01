using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

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

            var phoneCall = dto.IsNew() 
                ? new Phonecall { IsActive = true, Status = dto.Status, OwnerCode = dto.OwnerRef.GetId() } 
                : _finder.Find(Specs.Find.ById<Phonecall>(dto.Id)).One();

            phoneCall.Header = dto.Header;
            phoneCall.Description = dto.Description;
            phoneCall.Priority = dto.Priority;
            phoneCall.Purpose = dto.Purpose;
            phoneCall.ScheduledOn = dto.ScheduledOn;
            phoneCall.Timestamp = dto.Timestamp;

            return phoneCall;
        }
    }
}
