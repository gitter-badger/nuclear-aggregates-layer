using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify.DomainEntityObtainers
{
    public class KazakhstanLegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, IKazakhstanAdapted
    {
        private readonly IFinder _finder;

        public KazakhstanLegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (KazakhstanLegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(dto.Id)).One()
                              ?? new LegalPerson { IsActive = true, Parts = new IEntityPart[] { new KazakhstanLegalPersonPart() } };

            LegalPersonFlexSpecs.LegalPersons.Kazakhstan.Assign.Entity().Assign(dto, legalPerson);

            return legalPerson;
        }
    }
}