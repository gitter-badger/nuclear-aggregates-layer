using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class ContactObtainer : IBusinessModelEntityObtainer<Contact>, IAggregateReadModel<User>
    {
        private readonly IFinder _finder;

        public ContactObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Contact ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ContactDomainEntityDto)domainEntityDto;

            var contact = _finder.FindOne(Specs.Find.ById<Contact>(dto.Id)) 
                ?? new Contact { IsActive = true };

            contact.Id = dto.Id;
            contact.FirstName = dto.FirstName;
            contact.MiddleName = dto.MiddleName;
            contact.LastName = dto.LastName;
            contact.Salutation = dto.Salutation;
            contact.MainPhoneNumber = dto.MainPhoneNumber;
            contact.AdditionalPhoneNumber = dto.AdditionalPhoneNumber;
            contact.MobilePhoneNumber = dto.MobilePhoneNumber;
            contact.HomePhoneNumber = dto.HomePhoneNumber;
            contact.Fax = dto.Fax;
            contact.WorkEmail = dto.WorkEmail;
            contact.HomeEmail = dto.HomeEmail;
            contact.Website = dto.Website;
            contact.ImIdentifier = dto.ImIdentifier;
            contact.JobTitle = dto.JobTitle;
            contact.Department = dto.Department;
            contact.WorkAddress = dto.WorkAddress;
            contact.HomeAddress = dto.HomeAddress;
            contact.Comment = dto.Comment;

            contact.GenderCode = dto.GenderCode;
            contact.FamilyStatusCode = dto.FamilyStatusCode;
            contact.AccountRole = dto.AccountRole;

            contact.ClientId = dto.ClientRef.Id.Value;
            contact.OwnerCode = dto.OwnerRef.Id.Value;

            contact.IsFired = dto.IsFired;
            contact.BirthDate = dto.BirthDate;
            contact.Timestamp = dto.Timestamp;

            return contact;
        }
    }
}