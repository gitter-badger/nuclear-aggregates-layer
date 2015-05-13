using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Russia;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public class RussiaLegalPersonProfileObtainer : IBusinessModelEntityObtainer<LegalPersonProfile>, IAggregateReadModel<LegalPerson>, IRussiaAdapted
    {
        private readonly IFinder _finder;

        public RussiaLegalPersonProfileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPersonProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (RussiaLegalPersonProfileDomainEntityDto)domainEntityDto;

            var legalPersonProfile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(dto.Id))
                                  ?? new LegalPersonProfile { IsActive = true, Parts = new IEntityPart[] { new RussiaLegalPersonProfilePart() } };

            LegalPersonFlexSpecs.LegalPersonProfiles.Russia.Assign.Entity().Assign(dto, legalPersonProfile);

            return legalPersonProfile;
        }
    }
}