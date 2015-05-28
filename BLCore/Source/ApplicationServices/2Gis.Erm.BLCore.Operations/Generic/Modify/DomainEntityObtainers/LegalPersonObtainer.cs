using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public class LegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>
    {
        private readonly IFinder _finder;

        public LegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(dto.Id)).One()
                ?? new LegalPerson { IsActive = true };

            if (!dto.IsInSyncWith1C)
            {
                legalPerson.LegalPersonTypeEnum = dto.LegalPersonTypeEnum;
                legalPerson.LegalName = dto.LegalName.Ensure—leanness();
                legalPerson.ShortName = dto.ShortName.Ensure—leanness();
                legalPerson.LegalAddress = dto.LegalAddress.Ensure—leanness();
                legalPerson.Kpp = dto.Kpp.Ensure—leanness();
                legalPerson.PassportNumber = dto.PassportNumber.Ensure—leanness();
                legalPerson.RegistrationAddress = dto.RegistrationAddress.Ensure—leanness();
            }

            legalPerson.Inn = dto.Inn.Ensure—leanness();
            legalPerson.Ic = dto.Ic.Ensure—leanness();
            legalPerson.VAT = dto.VAT.Ensure—leanness();
            legalPerson.PassportSeries = dto.PassportSeries.Ensure—leanness();
            legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
            legalPerson.ClientId = dto.ClientRef.Id;
            legalPerson.Comment = dto.Comment.Ensure—leanness();
            legalPerson.PassportIssuedBy = dto.PassportIssuedBy.Ensure—leanness();
            legalPerson.Timestamp = dto.Timestamp;
            legalPerson.CardNumber = dto.CardNumber.Ensure—leanness();

            return legalPerson;
        }
    }
}