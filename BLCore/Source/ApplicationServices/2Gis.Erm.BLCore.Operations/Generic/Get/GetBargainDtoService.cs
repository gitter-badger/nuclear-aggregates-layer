using System;
using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public sealed class GetBargainDtoService : GetDomainEntityDtoServiceBase<Bargain>
    {
        private readonly ISecureFinder _finder;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public GetBargainDtoService(IUserContext userContext,
                                    ISecureFinder finder,
                                    ISecurityServiceFunctionalAccess functionalAccessService,
                                    ILegalPersonReadModel legalPersonReadModel,
                                    IBranchOfficeReadModel branchOfficeReadModel)
            : base(userContext)
        {
            _finder = finder;
            _functionalAccessService = functionalAccessService;
            _legalPersonReadModel = legalPersonReadModel;
            _branchOfficeReadModel = branchOfficeReadModel;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Bargain> GetDto(long entityId)
        {
            return _finder.FindObsolete(Specs.Find.ById<Bargain>(entityId))
                          .Select(entity => new BargainDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Number = entity.Number,
                                  BargainTypeRef = new EntityReference { Id = entity.BargainTypeId, Name = entity.BargainType.Name },
                                  CustomerLegalPersonRef = new EntityReference { Id = entity.CustomerLegalPersonId, Name = entity.LegalPerson.LegalName },
                                  ExecutorBranchOfficeRef =
                                      new EntityReference { Id = entity.ExecutorBranchOfficeId, Name = entity.BranchOfficeOrganizationUnit.ShortLegalName },
                                  Comment = entity.Comment,
                                  SignedOn = entity.SignedOn,
                                  ClosedOn = entity.ClosedOn,
                                  BargainEndDate = entity.BargainEndDate,
                                  BargainKind = entity.BargainKind,
                                  HasDocumentsDebt = entity.HasDocumentsDebt,
                                  DocumentsComment = entity.DocumentsComment,
                                  OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn,
                                  Timestamp = entity.Timestamp
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Bargain> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var dto = new BargainDomainEntityDto { BargainKind = BargainKind.Client, SignedOn = DateTime.UtcNow.Date };
            if (parentEntityId.HasValue)
            {
                if (parentEntityName.Equals(EntityType.Instance.LegalPerson()))
                {
                    dto.CustomerLegalPersonRef = new EntityReference
                        {
                            Id = parentEntityId,
                            Name = _legalPersonReadModel.GetLegalPersonName(parentEntityId.Value)
                        };
                }

                if (parentEntityName.Equals(EntityType.Instance.Client()))
                {
                    dto.ClientId = parentEntityId.Value;
                }
            }

            long legalPersonId;
            if (!string.IsNullOrEmpty(extendedInfo) &&
                long.TryParse(Regex.Match(extendedInfo, @"legalPersonId=(\d+)").Groups[1].Value, out legalPersonId))
            {
                dto.CustomerLegalPersonRef = new EntityReference
                    {
                        Id = legalPersonId,
                        Name = _legalPersonReadModel.GetLegalPersonName(legalPersonId)
                    };
            }

            long branchOfficeOrganizationUnitId;
            if (!string.IsNullOrEmpty(extendedInfo) &&
                long.TryParse(Regex.Match(extendedInfo, @"branchOfficeOrganizationUnitId=(\d+)").Groups[1].Value, out branchOfficeOrganizationUnitId))
            {
                dto.ExecutorBranchOfficeRef = new EntityReference
                    {
                        Id = branchOfficeOrganizationUnitId,
                        Name = _branchOfficeReadModel.GetBranchOfficeOrganizationName(branchOfficeOrganizationUnitId)
                    };
            }

            dto.IsLegalPersonChoosingDenied = dto.CustomerLegalPersonRef != null && dto.CustomerLegalPersonRef.Id.HasValue;
            dto.IsBranchOfficeOrganizationUnitChoosingDenied = dto.ExecutorBranchOfficeRef != null && dto.ExecutorBranchOfficeRef.Id.HasValue;

            return dto;
        }

        protected override void SetDtoProperties(IDomainEntityDto<Bargain> domainEntityDto, long entityId, bool readOnly, long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            ((BargainDomainEntityDto)domainEntityDto).UserCanWorkWithAdvertisingAgencies =
                _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement,
                                                                       _userContext.Identity.Code);
        }
    }
}