using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify
{
    public class BargainTypeFlexSpecs
    {
        public static class BargainTypes
        {
            public static class Russia
            {
                public static class Project
                {
                    public static IProjectSpecification<BargainType, BargainTypeDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<BargainType, BargainTypeDomainEntityDto>(
                            x =>
                                {
                                    var dto = new BargainTypeDomainEntityDto
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            SyncCode1C = x.SyncCode1C,
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
                    public static IAssignSpecification<BargainTypeDomainEntityDto, BargainType> Entity()
                    {
                        return new AssignSpecification<BargainTypeDomainEntityDto, BargainType>(
                            (dto, entity) =>
                                {
                                    entity.Name = dto.Name;
                                    entity.SyncCode1C = dto.SyncCode1C;
                                    entity.VatRate = dto.VatRate;
                                    entity.Timestamp = dto.Timestamp;
                                });
                    }
                }
            }
        }
    }
}