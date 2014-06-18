﻿using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileLegalPersonProfileObtainer : IBusinessModelEntityObtainer<LegalPersonProfile>, IAggregateReadModel<LegalPerson>, IChileAdapted
    {
        private readonly IFinder _finder;

        public ChileLegalPersonProfileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPersonProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ChileLegalPersonProfileDomainEntityDto)domainEntityDto;

            var legalPersonProfile = dto.IsNew()
                                         ? new LegalPersonProfile
                                             {
                                                 IsActive = true,
                                                 Parts = new[]
                                                     {
                                                         new ChileLegalPersonProfilePart
                                                             {
                                                                 IsActive = true
                                                             }
                                                     }
                                             }
                                         : _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(dto.Id));

            LegalPersonFlexSpecs.LegalPersonProfiles.Chile.Assign.Entity().Assign(dto, legalPersonProfile);

            return legalPersonProfile;
        }
    }
}