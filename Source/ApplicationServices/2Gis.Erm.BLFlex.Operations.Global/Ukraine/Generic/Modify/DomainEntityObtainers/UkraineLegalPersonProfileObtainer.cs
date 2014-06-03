using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineLegalPersonProfileObtainer : IBusinessModelEntityObtainer<LegalPersonProfile>, IAggregateReadModel<LegalPerson>, IUkraineAdapted
    {
        private readonly IFinder _finder;

        public UkraineLegalPersonProfileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPersonProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (UkraineLegalPersonProfileDomainEntityDto)domainEntityDto;

            var legalPerson = dto.IsNew()
                                  ? new LegalPersonProfile { IsActive = true, Parts = new[] { new UkraineLegalPersonProfilePart() } }
                                  : _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(dto.Id));

            LegalPersonFlexSpecs.LegalPersonProfiles.Ukraine.Assign.Entity().Assign(dto, legalPerson);

            return legalPerson;
        }
    }
}