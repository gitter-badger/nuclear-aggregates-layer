using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineLegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, IUkraineAdapted
    {
        private readonly IFinder _finder;

        public UkraineLegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (UkraineLegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = dto.IsNew()
                                  ? new LegalPerson { IsActive = true, Parts = new[] { new UkraineLegalPersonPart() } }
                                  : _finder.FindOne(Specs.Find.ById<LegalPerson>(dto.Id));

            LegalPersonFlexSpecs.LegalPersons.Ukraine.Assign.Entity().Assign(dto, legalPerson);

            return legalPerson;
        }
    }
}