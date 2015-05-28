using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify
{
    public partial class LegalPersonFlexSpecs
    {
        public static class LegalPersons
        {
            public static class MultiCulture
            {
                public static class Project
                {
                    public static MapSpecification<LegalPerson, LegalPersonDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<LegalPerson, LegalPersonDomainEntityDto>(
                            x =>
                                {
                                    if (x.IsNew())
                                    {
                                        return new LegalPersonDomainEntityDto
                                            {
                                                ClientRef = x.ClientId != 0
                                                                ? new EntityReference { Id = x.ClientId, Name = null }
                                                                : new EntityReference(),
                                            };
                                    }

                                    var dto = new LegalPersonDomainEntityDto
                                        {
                                            Id = x.Id,
                                            LegalName = x.LegalName,
                                            ShortName = x.ShortName,
                                            LegalPersonTypeEnum = x.LegalPersonTypeEnum,
                                            LegalAddress = x.LegalAddress,
                                            Inn = x.Inn,
                                            Kpp = x.Kpp,
                                            VAT = x.VAT,
                                            Ic = x.Ic,
                                            PassportSeries = x.PassportSeries,
                                            PassportNumber = x.PassportNumber,
                                            PassportIssuedBy = x.PassportIssuedBy,
                                            RegistrationAddress = x.RegistrationAddress,
                                            ClientRef = new EntityReference { Id = x.ClientId, Name = null },
                                            IsInSyncWith1C = x.IsInSyncWith1C,
                                            ReplicationCode = x.ReplicationCode,
                                            Comment = x.Comment,
                                            OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn,
                                            CardNumber = x.CardNumber,
                                            Timestamp = x.Timestamp
                                        };

                                    return dto;
                                });
                    }
                }
            }
        }
    }
}