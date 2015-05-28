using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify.DomainEntityObtainers
{
    public class EmiratesLegalPersonProfileObtainer : IBusinessModelEntityObtainer<LegalPersonProfile>, IAggregateReadModel<LegalPerson>, IEmiratesAdapted
    {
        private readonly IFinder _finder;

        public EmiratesLegalPersonProfileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPersonProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (EmiratesLegalPersonProfileDomainEntityDto)domainEntityDto;

            var legalPersonProfile = _finder.Find(Specs.Find.ById<LegalPersonProfile>(dto.Id)).One() ??
                                     new LegalPersonProfile
                                         {
                                             IsActive = true,
                                             Parts = new[] { new EmiratesLegalPersonProfilePart() }
                                         };

            LegalPersonFlexSpecs.LegalPersonProfiles.Emirates.Assign.Entity().Assign(dto, legalPersonProfile);

            return legalPersonProfile;
        }
    }
}