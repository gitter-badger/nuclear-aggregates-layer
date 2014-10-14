using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons
{
    public interface ILegalPersonRepository : IAggregateRootRepository<LegalPerson>, 
                                              IActivateAggregateRepository<LegalPerson>,
                                              IAssignAggregateRepository<LegalPerson>,
                                              IDeactivateAggregateRepository<LegalPerson>,
                                              ICheckAggregateForDebtsRepository<LegalPerson>,
                                              IChangeAggregateClientRepository<LegalPerson>
    {
        int Activate(LegalPerson legalPerson);
        int Assign(LegalPerson legalPerson, long ownerCode);
        int AssignWithRelatedEntities(long legalPersonId, long ownerCode, bool isPartialAssign);
        void ChangeLegalRequisites(LegalPerson legalPerson, string inn, string kpp, string legalAddress);
        void ChangeNaturalRequisites(LegalPerson legalPerson, string passportSeries, string passportNumber, string registrationAddress);
        int Deactivate(LegalPerson legalPerson);

        [Obsolete("������������ ������ � ExportLegalPersonsHandler")]
        void SyncWith1C(IEnumerable<LegalPerson> legalPersons);
        CheckForDublicatesResultDto CheckIfExistsInnDuplicate(long legalPersonId, string inn);
        CheckForDublicatesResultDto CheckIfExistsInnAndKppDuplicate(long legalPersonId, string inn, string kpp);
        CheckForDublicatesResultDto CheckIfExistsInnOrIcDuplicate(long legalPersonId, string inn, string kpp);
        CheckForDublicatesResultDto CheckIfExistsPassportDuplicate(long legalPersonId, string passportSeries, string passportNumber);
        string[] SelectNotUnique1CSyncCodes(IEnumerable<string> codes);

        [Obsolete("Use ILegalPersonReadModel.GetLegalPerson")]
        LegalPerson FindLegalPerson(long entityId);
        
        void SetProfileAsMain(long profileId);

        // FIXME {d.ivanov, 03.02.2014}: ����� �� ��������� ���������� LegalPersonWithProfile. ��������� � ILegalPersonReadModel + ������ ������ ������-������
        [Obsolete("��������� � ILegalPersonReadModel + ������ ������ ������-������")]
        LegalPersonWithProfiles GetLegalPersonWithProfiles(long legalPersonId);
        
        LegalPerson FindLegalPersonByProfile(long profileId);
        LegalPerson FindLegalPerson(string syncCodeWith1C, string inn, string kpp);
        IEnumerable<LegalPerson> FindLegalPersonsByInnAndKpp(string inn, string kpp);
        IEnumerable<LegalPerson> FindBusinessmenByInn(string inn);
        IEnumerable<LegalPerson> FindNaturalPersonsByPassport(string passportSeries, string passportNumber);
        IEnumerable<LegalPerson> FindLegalPersons(string syncCodeWith1C, long branchOfficeOrganizationUnitId);
        IEnumerable<LegalPersonFor1CExportDto> GetLegalPersonsForExportTo1C(long organizationUnitId, DateTime startPeriod);
    }
}
