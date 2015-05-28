using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify
{
    public partial class BranchOfficeFlexSpecs
    {
        public static class BranchOffices
        {
            public static class Ukraine
            {
                public static class Project
                {
                    public static MapSpecification<BranchOffice, UkraineBranchOfficeDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<BranchOffice, UkraineBranchOfficeDomainEntityDto>(
                            x =>
                                {
                                    var dto = new UkraineBranchOfficeDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            Egrpou = x.Inn,
                                            Ipn = x.Within<UkraineBranchOfficePart>().GetPropertyValue<string>(part => part.Ipn),
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
                    public static IAssignSpecification<UkraineBranchOfficeDomainEntityDto, BranchOffice> Entity()
                    {
                        return new AssignSpecification<UkraineBranchOfficeDomainEntityDto, BranchOffice>(
                            (dto, entity) =>
                                {
                                    entity.Id = dto.Id;
                                    entity.Name = dto.Name;
                                    entity.Inn = dto.Egrpou;
                                    entity.Within<UkraineBranchOfficePart>().SetPropertyValue(part => part.Ipn, dto.Ipn);
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