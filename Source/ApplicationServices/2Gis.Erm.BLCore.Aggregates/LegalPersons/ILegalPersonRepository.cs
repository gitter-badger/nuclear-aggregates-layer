using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons
{
    public interface ILegalPersonRepository : IAggregateRootRepository<LegalPerson>, 
                                              IDeleteAggregateRepository<LegalPerson>, 
                                              IDeleteAggregateRepository<LegalPersonProfile>,
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
        int Delete(LegalPerson legalPerson);
        int Delete(LegalPersonProfile legalPersonProfile);
        void CreateOrUpdate(LegalPerson legalPerson);
        void CreateOrUpdate(LegalPersonProfile legalPersonProfile);
        void SyncWith1C(IEnumerable<LegalPerson> legalPersons);
        LegalPersonForMergeDto GetInfoForMerging(long legalPersonId);
        CheckForDublicatesResultDto CheckIfExistsInnDuplicate(long legalPersonId, string inn);
        CheckForDublicatesResultDto CheckIfExistsInnAndKppDuplicate(long legalPersonId, string inn, string kpp);
        CheckForDublicatesResultDto CheckIfExistsInnOrIcDuplicate(long legalPersonId, string inn, string kpp);
        CheckForDublicatesResultDto CheckIfExistsPassportDuplicate(long legalPersonId, string passportSeries, string passportNumber);
        string[] SelectNotUnique1CSyncCodes(IEnumerable<string> codes);
        LegalPerson FindLegalPerson(long entityId);
        LegalPersonProfile FindLegalPersonProfile(long entityId);
        void SetProfileAsMain(long profileId);
        LegalPersonWithProfiles GetLegalPersonWithProfiles(long legalPersonId);
        LegalPerson FindLegalPersonByProfile(long profileId);
        LegalPerson FindLegalPerson(string syncCodeWith1C, string inn, string kpp);
        IEnumerable<LegalPerson> FindLegalPersonsByInnAndKpp(string inn, string kpp);
        IEnumerable<LegalPerson> FindBusinessmenByInn(string inn);
        IEnumerable<LegalPerson> FindNaturalPersonsByPassport(string passportSeries, string passportNumber);
        IEnumerable<LegalPerson> FindLegalPersons(string syncCodeWith1C, long branchOfficeOrganizationUnitId);
        LegalPersonName GetLegalPersonNameByClientId(long clientId);
        IEnumerable<LegalPersonFor1CExportDto> GetLegalPersonsForExportTo1C(long organizationUnitId, DateTime startPeriod);
    }
}
