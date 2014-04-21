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
    // TODO {all, 07.04.2014}: �  ����� �� obtainers ��. ������� � IBusinessModelEntityObtainerFlex, �� ��������� ���������� ������ ��������� ����������� �������� ��������� ���� ������ obtainers ������ ���� �� ��������������/�����������
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
                legalPerson.LegalName = dto.LegalName.Ensure�leanness();
                legalPerson.ShortName = dto.ShortName.Ensure�leanness();
                legalPerson.LegalAddress = dto.LegalAddress.Ensure�leanness();
                legalPerson.Kpp = dto.Kpp.Ensure�leanness();
                legalPerson.PassportNumber = dto.PassportNumber.Ensure�leanness();
                legalPerson.RegistrationAddress = dto.RegistrationAddress.Ensure�leanness();
            }

            legalPerson.Inn = dto.Inn.Ensure�leanness();
            legalPerson.Ic = dto.Ic.Ensure�leanness();
            legalPerson.VAT = dto.VAT.Ensure�leanness();
            legalPerson.PassportSeries = dto.PassportSeries.Ensure�leanness();
            legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
            legalPerson.ClientId = dto.ClientRef.Id;
            legalPerson.Comment = dto.Comment.Ensure�leanness();
            legalPerson.PassportIssuedBy = dto.PassportIssuedBy.Ensure�leanness();
            legalPerson.Timestamp = dto.Timestamp;
            legalPerson.CardNumber = dto.CardNumber.Ensure�leanness();

            legalPerson.Parts = _flexBehaviour.GetEntityParts(legalPerson);

            _flexBehaviour.CopyPartFields(legalPerson, dto);

            return legalPerson;
        }
    }
}