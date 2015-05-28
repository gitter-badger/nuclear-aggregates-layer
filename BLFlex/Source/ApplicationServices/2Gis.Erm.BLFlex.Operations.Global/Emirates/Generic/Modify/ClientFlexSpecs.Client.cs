using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify
{
    public static class ClientFlexSpecs
    {
        public static class Clients
        {
            public static class Emirates
            {
                public static class Project
                {
                    public static MapSpecification<Client, EmiratesClientDomainEntityDto> DomainEntityDto()
                    {
                        return new MapSpecification<Client, EmiratesClientDomainEntityDto>(
                            x =>
                                {
                                    if (x.IsNew())
                                    {
                                        return new EmiratesClientDomainEntityDto();
                                    }

                                    return new EmiratesClientDomainEntityDto()
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            MainPhoneNumber = x.MainPhoneNumber,
                                            AdditionalPhoneNumber1 = x.AdditionalPhoneNumber1,
                                            AdditionalPhoneNumber2 = x.AdditionalPhoneNumber2,
                                            Email = x.Email,
                                            Fax = x.Fax,
                                            Website = x.Website,
                                            InformationSource = x.InformationSource,
                                            Comment = x.Comment,
                                            MainAddress = x.MainAddress,
                                            LastQualifyTime = x.LastQualifyTime,
                                            LastDisqualifyTime = x.LastDisqualifyTime,
                                            MainFirmRef = new EntityReference { Id = x.MainFirmId },
                                            TerritoryRef = new EntityReference { Id = x.TerritoryId },
                                            OwnerRef = new EntityReference { Id = x.OwnerCode, Name = null },
                                            CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                            CreatedOn = x.CreatedOn,
                                            IsActive = x.IsActive,
                                            IsDeleted = x.IsDeleted,
                                            ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                            ModifiedOn = x.ModifiedOn,
                                            Timestamp = x.Timestamp,
                                            PoBox = x.Within<EmiratesClientPart>().GetPropertyValue(part => part.PoBox)
                                        };
                                });
                    }
                }

                public static class Assign
                {
                    public static IAssignSpecification<EmiratesClientDomainEntityDto, Client> Entity()
                    {
                        return new AssignSpecification<EmiratesClientDomainEntityDto, Client>(
                            (dto, client) =>
                                {
                                    client.Name = dto.Name;
                                    client.Comment = dto.Comment;
                                    client.MainAddress = dto.MainAddress;
                                    client.MainPhoneNumber = dto.MainPhoneNumber;
                                    client.AdditionalPhoneNumber1 = dto.AdditionalPhoneNumber1;
                                    client.AdditionalPhoneNumber2 = dto.AdditionalPhoneNumber2;
                                    client.Email = dto.Email;
                                    client.Fax = dto.Fax;
                                    client.Website = dto.Website;
                                    client.InformationSource = dto.InformationSource;
                                    client.LastQualifyTime = dto.LastQualifyTime;
                                    client.LastDisqualifyTime = dto.LastDisqualifyTime;
                                    client.MainFirmId = dto.MainFirmRef.Id;

                                    client.TerritoryId = dto.TerritoryRef.Id.Value;
                                    client.OwnerCode = dto.OwnerRef.Id.Value;

                                    client.Timestamp = dto.Timestamp;

                                    client.Within<EmiratesClientPart>()
                                               .SetPropertyValue(part => part.PoBox, dto.PoBox);
                                });
                    }
                }
            }
        }
    }
}