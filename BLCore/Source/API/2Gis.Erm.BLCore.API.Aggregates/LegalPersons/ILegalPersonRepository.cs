using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons
{
    public interface ILegalPersonRepository : IAggregateRootRepository<LegalPerson>, 
                                              IActivateAggregateRepository<LegalPerson>,
                                              IAssignAggregateRepository<LegalPerson>,
                                              ICheckAggregateForDebtsRepository<LegalPerson>,
                                              IChangeAggregateClientRepository<LegalPerson>
    {
        int Activate(LegalPerson legalPerson);
        int Assign(LegalPerson legalPerson, long ownerCode);
        int AssignWithRelatedEntities(long legalPersonId, long ownerCode, bool isPartialAssign);
        void ChangeLegalRequisites(LegalPerson legalPerson, string inn, string kpp, string legalAddress);
        void ChangeNaturalRequisites(LegalPerson legalPerson, string passportSeries, string passportNumber, string registrationAddress);

        [Obsolete("Используется только в ExportLegalPersonsHandler")]
        void SyncWith1C(IEnumerable<LegalPerson> legalPersons);
        CheckForDublicatesResultDto CheckIfExistsInnDuplicate(long legalPersonId, string inn);
        CheckForDublicatesResultDto CheckIfExistsInnAndKppDuplicate(long legalPersonId, string inn, string kpp);
        CheckForDublicatesResultDto CheckIfExistsInnOrIcDuplicate(long legalPersonId, string inn, string kpp);
        CheckForDublicatesResultDto CheckIfExistsPassportDuplicate(long legalPersonId, string passportSeries, string passportNumber);
        

        [Obsolete("Use ILegalPersonReadModel.GetLegalPerson")]
        LegalPerson FindLegalPerson(long entityId);
        
        void SetProfileAsMain(long profileId);
        
        LegalPerson FindLegalPersonByProfile(long profileId);
        LegalPerson FindLegalPerson(string syncCodeWith1C, string inn, string kpp);
        IEnumerable<LegalPerson> FindLegalPersonsByInnAndKpp(string inn, string kpp);
        IEnumerable<LegalPerson> FindBusinessmenByInn(string inn);
        IEnumerable<LegalPerson> FindNaturalPersonsByPassport(string passportSeries, string passportNumber);
        IEnumerable<LegalPerson> FindLegalPersons(string syncCodeWith1C, long branchOfficeOrganizationUnitId);
        IEnumerable<LegalPersonFor1CExportDto> GetLegalPersonsForExportTo1C(long organizationUnitId, DateTime startPeriod);
    }
}
