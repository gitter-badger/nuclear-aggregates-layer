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
    public class EmiratesLegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, IEmiratesAdapted
    {
        private readonly IFinder _finder;

        public EmiratesLegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (EmiratesLegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(dto.Id))
                ?? new LegalPerson { IsActive = true, Parts = new[] { new EmiratesLegalPersonPart() } };

            LegalPersonFlexSpecs.LegalPersons.Emirates.Assign.Entity().Assign(dto, legalPerson);

            return legalPerson;
        }
    }
}