﻿using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify
{
    public static class FirmFlexSpecs
    {
        public static class FirmAddresses
        {
            public static class Emirates
            {
                public static class Project
                {
                    public static IProjectSpecification<FirmAddress, EmiratesFirmAddressDomainEntityDto> DomainEntityDto()
                    {
                        return new ProjectSpecification<FirmAddress, EmiratesFirmAddressDomainEntityDto>(
                            x => new EmiratesFirmAddressDomainEntityDto
                                {
                                    Id = x.Id,
                                    Address = x.Address + ((x.ReferencePoint == null) ? string.Empty : " — " + x.ReferencePoint),
                                    PaymentMethods = x.PaymentMethods,
                                    WorkingTime = x.WorkingTime,
                                    ClosedForAscertainment = x.ClosedForAscertainment,
                                    IsLocatedOnTheMap = x.IsLocatedOnTheMap,
                                    FirmRef = new EntityReference { Id = x.FirmId, Name = null },
                                    CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                    CreatedOn = x.CreatedOn,
                                    IsActive = x.IsActive,
                                    IsDeleted = x.IsDeleted,
                                    ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                    ModifiedOn = x.ModifiedOn,
                                    Timestamp = x.Timestamp,
                                    PoBox = x.Within<EmiratesFirmAddressPart>().GetPropertyValue(part => part.PoBox)
                                });
                    }
                }
            }
        }
    }
}