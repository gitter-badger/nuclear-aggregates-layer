﻿using System.Linq;

using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
            return _finder.Find<Contact>(x => x.Id == entityId)
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
                                  GenderCode = (Gender)x.GenderCode,
                                  FamilyStatusCode = (FamilyStatus)x.FamilyStatusCode,
                                  AccountRole = (AccountRole)x.AccountRole,
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

        protected override IDomainEntityDto<Contact> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            if (parentEntityName == EntityName.Client)
            {
                return _finder.Find<Client>(x => x.Id == parentEntityId)
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