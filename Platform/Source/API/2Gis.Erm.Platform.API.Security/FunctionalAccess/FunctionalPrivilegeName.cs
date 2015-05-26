using System;

namespace DoubleGis.Erm.Platform.API.Security.FunctionalAccess
{
    // refers to table 'Security.Privileges', column 'Id'
    public enum FunctionalPrivilegeName
    {
        /// !!!Соглашение!!! При добавлении элементов в этот enum нужно, чтобы их значения 
        /// НЕ пересекались со значениями элементов enum EntityAccessTypes.См. EntityControllerBaseMetadata.SetToolbarActionsState
        Undefined = 0,
        ReserveAccess = 5,
        OrderStatesAccess = 3,
        LimitManagement = 189,
        OrderCreationExtended = 190,
        OrderBranchOfficeOrganizationUnitSelection = 191,
        LegalPersonChangeClient = 192,
        LegalPersonChangeRequisites = 193,
        OrderChangeDealExtended = 496,
        OrderChangeDocumentsDebt = 497,
        OrderCreationForFuture = 498,
        MergeClients = 531,
        CreateAccountDetails = 532,
        WithdrawalAccess = 533,
        ReleaseAccess = 534,
        CorporateQueueAccess = 535,
        ProcessAccountsWithDebts = 537,
        CloseBargainOperationalPeriod = 538,
        ServiceUserAssign = 539,
        SendMassEmail = 540,
        ChangeFirmClient = 541,
        LeaveClientWithNoFirms = 542,
        PrereleaseOrderValidationExecution = 543,
        EditOrderType = 544,
        ChangeFirmTerritory = 545,
        MergeLegalPersons = 546,
        FranchiseesWithdrawalExport = 604,
        // FIXME {all, 15.10.2014}: Выпилить эту привилегию из базы после релиза ERM-5100 (ветка $/ERM.BL/Dev/Features/ERM-4827-LegalPersonDeactivation)
        DeleteLegalPersonProfile = 606,
        LimitRecalculation = 638,
        LimitPeriodChanging = 639,
        CascadeLegalPersonAssign = 640,
        PositionAdministrationCode = 641,
        AdvertisementVerification = 642,
        PublishAdvertisementTemplate = 643,
        UnpublishAdvertisementTemplate = 644,
        EditDummyAdvertisement = 645,
        HotClientProcessing = 646,
        HotClientTelemarketingProcessingFranchisee = 647,
        AdvertisementAgencyManagement = 648,
        HotClientTelemarketingProcessingBranch = 649,
        LegalPersonDeactivationOrActivation = 650,
        TelephonyAccess = 652, 
        EditAdvertisementAgencyOrderType = 651,
    }
}