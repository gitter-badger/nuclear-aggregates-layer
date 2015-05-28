using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Czech;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOffices
        {
            public static class Czech
            {
                public static class Project
                {
                    public static MapSpecification<BranchOffice, CzechBranchOfficeDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<BranchOffice, CzechBranchOfficeDomainEntityDto>(
                            x =>
                                {
                                    var dto = new CzechBranchOfficeDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            Dic = x.Inn,
                                            Ic = x.Ic,
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
                    public static IAssignSpecification<CzechBranchOfficeDomainEntityDto, BranchOffice> Entity()
                    {
                        return new AssignSpecification<CzechBranchOfficeDomainEntityDto, BranchOffice>(
                            (dto, entity) =>
                                {
                                    entity.Id = dto.Id;
                                    entity.Name = dto.Name;
                                    entity.Inn = dto.Dic;
                                    entity.Ic = dto.Ic;
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