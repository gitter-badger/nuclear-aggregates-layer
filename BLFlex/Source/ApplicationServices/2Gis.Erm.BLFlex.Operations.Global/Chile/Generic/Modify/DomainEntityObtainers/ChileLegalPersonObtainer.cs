using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileLegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, IChileAdapted
    {
        private readonly IFinder _finder;

        public ChileLegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (ChileLegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = dto.IsNew()
                                  ? new LegalPerson { IsActive = true, Parts = new[] { new ChileLegalPersonPart() } }
                                  : _finder.Find(Specs.Find.ById<LegalPerson>(dto.Id)).One();

            LegalPersonFlexSpecs.LegalPersons.Chile.Assign.Entity().Assign(dto, legalPerson);

            return legalPerson;
        }
    }
}