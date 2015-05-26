using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOffices
        {
            public static class Russia
            {
                public static class Project
                {
                    public static ProjectSpecification<BranchOffice, RussiaBranchOfficeDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<BranchOffice, RussiaBranchOfficeDomainEntityDto>(
                            x =>
                                {
                                    var dto = new RussiaBranchOfficeDomainEntityDto
                                        {
                                            Id = x.Id,
                                            DgppId = x.DgppId,
                                            Name = x.Name,
                                            Inn = x.Inn,
                                            Annotation = x.Annotation,
                                            UsnNotificationText = x.UsnNotificationText,
                                            BargainTypeRef = new EntityReference { Id = x.BargainTypeId, Name = null },
                                            ContributionTypeRef = new EntityReference { Id = x.ContributionTypeId, Name = null },
                                            LegalAddress = x.LegalAddress,
                                            Timestamp = x.Timestamp,
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn
                                        };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<RussiaBranchOfficeDomainEntityDto, BranchOffice> Entity()
                    {
                        return new AssignSpecification<RussiaBranchOfficeDomainEntityDto, BranchOffice>(
                            (dto, entity) =>
                                {
                                    entity.Id = dto.Id;
                                    entity.Name = dto.Name;
                                    entity.Inn = dto.Inn;
                                    entity.Annotation = dto.Annotation;
                                    entity.UsnNotificationText = dto.UsnNotificationText;
                                    entity.BargainTypeId = dto.BargainTypeRef.Id;
                                    entity.ContributionTypeId = dto.ContributionTypeRef.Id;
                                    entity.LegalAddress = dto.LegalAddress;
                                    entity.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}