using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.MultiCulture;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify
{
    public class BargainTypeFlexSpecs
    {
        public static class BargainTypes
        {
            public static class MultiCulture
            {
                public static class Project
                {
                    public static ProjectSpecification<BargainType, MultiCultureBargainTypeDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<BargainType, MultiCultureBargainTypeDomainEntityDto>(
                            x =>
                                {
                                    var dto = new MultiCultureBargainTypeDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            VatRate = x.VatRate,
                                            Timestamp = x.Timestamp,
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy },
                                            ModifiedOn = x.ModifiedOn
                                        };

                                    return dto;
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<MultiCultureBargainTypeDomainEntityDto, BargainType> Entity()
                    {
                        return new AssignSpecification<MultiCultureBargainTypeDomainEntityDto, BargainType>(
                            (dto, entity) =>
                                {
                                    entity.Name = dto.Name;
                                    entity.VatRate = dto.VatRate;
                                    entity.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}