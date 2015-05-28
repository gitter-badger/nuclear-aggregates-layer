using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetContactDtoService : GetDomainEntityDtoServiceBase<Contact>
    {
        private readonly ISecureFinder _finder;

        public GetContactDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<Contact> GetDto(long entityId)
        {
            return _finder.FindObsolete(new FindSpecification<Contact>(x => x.Id == entityId))
                          .Select(x => new ContactDomainEntityDto
                              {
                                  Id = x.Id,
                                  ReplicationCode = x.ReplicationCode,
                                  FullName = x.FullName,
                                  FirstName = x.FirstName,
                                  MiddleName = x.MiddleName,
                                  LastName = x.LastName,
                                  Salutation = x.Salutation,
                                  MainPhoneNumber = x.MainPhoneNumber,
                                  AdditionalPhoneNumber = x.AdditionalPhoneNumber,
                                  MobilePhoneNumber = x.MobilePhoneNumber,
                                  HomePhoneNumber = x.HomePhoneNumber,
                                  Fax = x.Fax,
                                  WorkEmail = x.WorkEmail,
                                  HomeEmail = x.HomeEmail,
                                  Website = x.Website,
                                  ImIdentifier = x.ImIdentifier,
                                  JobTitle = x.JobTitle,
                                  Department = x.Department,
                                  WorkAddress = x.WorkAddress,
                                  HomeAddress = x.HomeAddress,
                                  Comment = x.Comment,
                                  GenderCode = x.GenderCode,
                                  FamilyStatusCode = x.FamilyStatusCode,
                                  AccountRole = x.AccountRole,
                                  ClientRef = new EntityReference { Id = x.ClientId, Name = x.Client.Name },
                                  ClientReplicationCode = x.Client.ReplicationCode,
                                  IsFired = x.IsFired,
                                  BirthDate = x.BirthDate,
                                  OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                  CreatedOn = x.CreatedOn,
                                  IsActive = x.IsActive,
                                  IsDeleted = x.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                  ModifiedOn = x.ModifiedOn,
                                  Timestamp = x.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Contact> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            if (parentEntityName.Equals(EntityType.Instance.Client()))
            {
                return _finder.FindObsolete(new FindSpecification<Client>(x => x.Id == parentEntityId))
                              .Select(x => new ContactDomainEntityDto
                                  {
                                      OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                      ClientRef = new EntityReference { Id = x.Id, Name = x.Name },
                                      MainPhoneNumber = x.MainPhoneNumber,
                                      AdditionalPhoneNumber = x.AdditionalPhoneNumber1 ?? x.AdditionalPhoneNumber2,
                                      Fax = x.Fax,
                                      WorkAddress = x.MainAddress,
                                      IsActive = true
                                  })
                              .Single();
            }

            return new ContactDomainEntityDto();
        }
    }
}