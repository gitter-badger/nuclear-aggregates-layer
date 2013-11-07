using System.Linq;

using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: �������� �� BL.Operations - ��� ����� � ������ �������, ������ �� ������������ ������ � TFS ��-�� �������������� merge - ���� ��������� ��� �����, ��� RI �� 1.0 ����� �������� �������� ����� ������� ���� ���������� �� 2��
    // ������ ����������� ������� internal, ����� �� ������������� massprocessor
    internal sealed class CzechLegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, ICzechAdapted
    {
        private readonly IFinder _finder;

        public CzechLegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = dto.Id == 0
                                  ? new LegalPerson { IsActive = true }
                                  : _finder.Find(LegalPersonSpecifications.Find.ById(dto.Id)).Single();

            if (!dto.IsInSyncWith1C)
            {
                legalPerson.LegalPersonTypeEnum = (int)dto.LegalPersonTypeEnum;
                legalPerson.LegalName = dto.LegalName;
                legalPerson.ShortName = dto.ShortName;
                legalPerson.LegalAddress = dto.LegalAddress;
                legalPerson.Kpp = dto.Kpp;
                legalPerson.PassportNumber = dto.PassportNumber;
            }

            legalPerson.Inn = dto.LegalPersonTypeEnum == LegalPersonType.Businessman ? dto.BusinessmanInn : dto.Inn;
            legalPerson.Ic = dto.Ic;
            legalPerson.PassportSeries = dto.PassportSeries;
            legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
            legalPerson.ClientId = dto.ClientRef.Id;
            legalPerson.Comment = dto.Comment;
            legalPerson.PassportIssuedBy = dto.PassportIssuedBy;
            legalPerson.RegistrationAddress = dto.RegistrationAddress;
            legalPerson.Timestamp = dto.Timestamp;
            legalPerson.CardNumber = dto.CardNumber;

            return legalPerson;
        }
    }
}