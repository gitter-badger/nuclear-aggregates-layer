using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    // TODO {all, 07.04.2014}: â  öåëîì ïî obtainers ñì. êîììåíò ê IBusinessModelEntityObtainerFlex, äî âûğàáîòêè áîëååìåíåå ÷åòêîé èäåîëîãèè äàëüíåéøåãî ğàçâèòèÿ ïğåäëàãàş ïîêà äàëüøå obtainers òàêîãî òèïà íå ìàñøòàáèğîâàòü/êëîíèğîâàòü
    public class LegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>
    {
        private readonly IFinder _finder;
        private readonly IBusinessModelEntityObtainerFlex<LegalPerson> _flexBehaviour;

        public LegalPersonObtainer(IFinder finder, IBusinessModelEntityObtainerFlex<LegalPerson> flexBehaviour)
        {
            _finder = finder;
            _flexBehaviour = flexBehaviour;
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
                legalPerson.LegalName = dto.LegalName.EnsureÑleanness();
                legalPerson.ShortName = dto.ShortName.EnsureÑleanness();
                legalPerson.LegalAddress = dto.LegalAddress.EnsureÑleanness();
                legalPerson.Kpp = dto.Kpp.EnsureÑleanness();
                legalPerson.PassportNumber = dto.PassportNumber.EnsureÑleanness();
                legalPerson.RegistrationAddress = dto.RegistrationAddress.EnsureÑleanness();
            }

            legalPerson.Inn = dto.Inn.EnsureÑleanness();
            legalPerson.Ic = dto.Ic.EnsureÑleanness();
            legalPerson.VAT = dto.VAT.EnsureÑleanness();
            legalPerson.PassportSeries = dto.PassportSeries.EnsureÑleanness();
            legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
            legalPerson.ClientId = dto.ClientRef.Id;
            legalPerson.Comment = dto.Comment.EnsureÑleanness();
            legalPerson.PassportIssuedBy = dto.PassportIssuedBy.EnsureÑleanness();
            legalPerson.Timestamp = dto.Timestamp;
            legalPerson.CardNumber = dto.CardNumber.EnsureÑleanness();

            legalPerson.Parts = _flexBehaviour.GetEntityParts(legalPerson);

            _flexBehaviour.CopyPartFields(legalPerson, dto);

            return legalPerson;
        }
    }
}