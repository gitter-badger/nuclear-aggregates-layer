using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Common.Utils;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public sealed class LegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, IRussiaAdapted
    {
        private readonly IFinder _finder;

        public LegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = dto.Id == 0
                                  ? new LegalPerson { IsActive = true }
                                  : _finder.Find(Specs.Find.ById<LegalPerson>(dto.Id)).Single();

            if (!dto.IsInSyncWith1C)
            {
                legalPerson.LegalPersonTypeEnum = (int)dto.LegalPersonTypeEnum;
                legalPerson.LegalName = dto.LegalName.EnsureСleanness();
                legalPerson.ShortName = dto.ShortName.EnsureСleanness();
                legalPerson.LegalAddress = dto.LegalAddress.EnsureСleanness();
                legalPerson.Kpp = dto.Kpp.EnsureСleanness();
                legalPerson.Ic = dto.Ic.EnsureСleanness();
                legalPerson.PassportNumber = dto.PassportNumber.EnsureСleanness();
                legalPerson.RegistrationAddress = dto.RegistrationAddress.EnsureСleanness();
            }

            legalPerson.Inn = (dto.LegalPersonTypeEnum == LegalPersonType.Businessman ? dto.BusinessmanInn : dto.Inn).EnsureСleanness();
            legalPerson.PassportSeries = dto.PassportSeries.EnsureСleanness();
            legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
            legalPerson.ClientId = dto.ClientRef.Id;
            legalPerson.Comment = dto.Comment.EnsureСleanness();
            legalPerson.PassportIssuedBy = dto.PassportIssuedBy.EnsureСleanness();
            legalPerson.Timestamp = dto.Timestamp;

            return legalPerson;
        }
    }
}