using System;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public static class OperationIdentityIds
    {
        public const int WithdrawIdentity = 32;
        public const int RevertWithdrawalIdentity = 33;

        // concrete operations
        public const int PrintOrderIdentity = 1000;
        public const int StartReleaseIdentity = 1005;
        public const int StartSimplifiedReleaseIdentity = 1006;
        public const int FinishReleaseIdentity = 1007;
        public const int RevertReleaseIdentity = 1008;
        public const int AttachExternalReleaseProcessingMessagesIdentity = 1009;
        public const int ValidateOrdersForReleaseIdentity = 1010;
        public const int EnsureOrdersForReleaseCompletelyExportedIdentity = 1011;
        public const int ChangeDealStageIdentity = 1012;
        public const int ActualizeAccountsDuringWithdrawalIdentity = 1014;
        public const int WithdrawFromAccountsIdentity = 1015;
        public const int ActualizeOrdersDuringWithdrawalIdentity = 1016;
        public const int ActualizeDealsDuringWithdrawalIdentity = 1017;
        public const int RevertWithdrawFromAccountsIdentity = 1018;
        public const int ActualizeAccountsDuringRevertingWithdrawalIdentity = 1019;
        public const int ActualizeOrdersDuringRevertingWithdrawalIdentity = 1020;
        public const int ActualizeDealsDuringRevertingWithdrawalIdentity = 1021;
        public const int GetFirmInfoIdentity = 1022;
        public const int ValidateOrdersIdentity = 1023;
        public const int RegisterOrderStateChangesIdentity = 1024;

        // EntityType.Instance.ReleaseWithdrawal = 215
        public const int ActualizeOrderReleaseWithdrawalsIdentity = 21501;

        // EntityType.Instance.OrderValidationResult = 232
        public const int ResetValidationGroupIdentity = 23201;

        // EntityType.Instance.Advertisement = 186
        public const int SelectAdvertisementToWhitelistIdentity = 18601;

        // EntityType.Instance.Bargain = 198
        [Obsolete]
        public const int BindBargainToOrderIdentity = 19801;
        public const int BulkCloseClientBargainsIdentity = 19802;
        public const int DetermineOrderBargainIdentity = 19803;

        // EntityType.Instance.Firm = 146
        public const int ImportCardsFromServiceBusIdentity = 14601;
        public const int ImportFirmIdentity = 14602;
        public const int ImportTerritoriesIdentity = 14603;
        public const int ImportCardForErmIdentity = 14604;
        public const int ImportCardRelationIdentity = 14605;
        public const int CreateBlankFirmsIdentity = 14606;
        public const int ImportFirmsDuringImportCardsForErmIdentity = 14607;
        public const int ImportCardRelationForErmIdentity = 14608;

        // EntityType.Instance.FirmAddress = 164
        [Obsolete]
        public const int SpecifyFirmAddressAdditionalServicesIdentity = 16401;
        public const int ImportFirmAddressFromServiceBusIdentity = 16402;

        // EntityType.Instance.LegalPerson = 147
        public const int ChangeLegalPersonRequisitesIdentity = 14701;
        public const int ValidateLegalPersonsForExportIdentity = 14702;

        // EntityType.Instance.Client = 200
        public const int CreateClientByFirmIdentity = 20001;
        public const int SetMainFirmIdentity = 20002;
        public const int CalculateClientPromisingIdentity = 20003;

        // EntityType.Instance.LegalPersonProfile = 219
        public const int SetAsMainLegalPersonProfileIdentity = 21901;

        // EntityType.Instance.Order = 151
        public const int ExportAccountDetailsTo1CIdentity = 15102;
        [Obsolete]
        public const int PrintRegionalOrderIdentity = 15103;
        public const int ReportsServiceIdentity = 15104;
        [Obsolete]
        public const int RemoveBargainIdentity = 15105;
        public const int ChangeDealIdentity = 15106;
        public const int RepairOutdatedIdentity = 15107;
        public const int CloseWithDenialIdentity = 15108;
        public const int SetInspectorIdentity = 15109;
        public const int UpdateOrderFinancialPerfomanceIdentity = 15110;
        public const int WorkflowProcessingIdentity = 15111;
        public const int CalculateOrderCostIdentity = 15112;
        public const int CopyOrderIdentity = 15113;
        public const int ObtainDealForBizaccountOrderIdentity = 15114;
        public const int CreateOrderBillsIdentity = 15115;
        public const int DeleteOrderBillsIdentity = 15116;
        public const int ActualizeOrderAmountToWithdrawIdentity = 15117;
        public const int CheckIfOrderPositionCanBeCreatedForOrderIdentity = 15118;
        public const int CheckIfOrderPositionCanBeModifiedIdentity = 15119;
        public const int ChangeOrderLegalPersonProfileIdentity = 15120;
        public const int PrintBindingChangeAgreementIdentity = 15121;
        public const int PrintFirmNameChangeAgreementIdentity = 15122;
        public const int PrintCancellationAgreementIdentity = 15123;
        public const int GetOrderDocumentsDebtIdentity = 15124;
        public const int SetOrderDocumentsDebtIdentity = 15125;

        // EntityType.Instance.Bill = 188
        public const int CalculateBillsIdentity = 18801;

        // EntityType.Instance.OrderPosition = 150
        public const int CalculateOrderPositionCostIdentity = 15001;
        public const int ValidateOrderPositionAdvertisementsIdentity = 15002;
        public const int CalculateCategoryRateIdentity = 15003;
        public const int ViewOrderPositionIdentity = 15004;
        public const int ReplaceOrderPositionAdvertisementLinksIdentity = 15005;
        public const int ChangeOrderPositionBindingObjectsIdentity = 15006;
        public const int GetAvailableBinfingObjectsIdentity = 15007;
        public const int CalculateOrderPositionPricePerUnitIdentity = 15008;

        // EntityType.Instance.Position = 153
        public const int ChangeSortingOrderIdentity = 15301;

        // EntityType.Instance.Price = 155
        public const int CopyPriceIdentity = 15501;
        public const int GetRatedPricesForCategoryIdentity = 15502;
        public const int PublishPriceIdentity = 15503;
        public const int UnpublishPriceIdentity = 15504;
        public const int ReplacePriceIdentity = 15505;

        // EntityType.Instance.Price = 154
        public const int CopyPricePositionIdentity = 15401;

        // EntityType.Instance.Limit = 192
        public const int CloseLimitIdentity = 19201;
        public const int ReopenLimitIdentity = 19202;
        public const int RecalculateLimitIdentity = 19203;
        public const int SetLimitStatusIdentity = 19204;
        public const int CalculateLimitIncreasingIdentity = 19205;
        public const int IncreaseLimitIdentity = 19206;

        // EntityType.Instance.HotClientRequest = 257
        public const int ImportHotClientIdentity = 25701;
        public const int GetHotClientRequestIdentity = 25702;
        public const int ProcessHotClientRequestIdentity = 25704;
        public const int BindTaskToHotClientRequestIdentity = 25703;

        // EntityType.Instance.AccountDetail = 141
        public const int ImportOperationsInfoIdentity = 14101;
        public const int NotifyAboutAccountDetailModificationIdentity = 14102;
        public const int GetDebitsInfoInitialForExportIdentity = 14103;
        [Obsolete("Такой операции больше нет")]public const int GetAccountDetailsForExportContentIdentity = 14104;

        // EntityType.Instance.Account = 142
        public const int GetWithdrawalErrorsCsvReportIdentity = 14201;

        // EntityType.Instance.OrderProcessingRequest = 550
        public const int RequestOrderProlongationIdentity = 55001;
        public const int RequestOrderCreationIdentity = 55002;
        public const int CancelOrderProcessingRequestIdentity = 55003;
        public const int ProlongateOrderByRequestIdentity = 55004;     
        public const int CreateOrderByRequestIdentity = 55005;
        public const int ProlongateOrderForAllRequestsIdentity = 55006;
        public const int GetOrderRequestMessagesIdentity = 55007;
        public const int SelectOrderProcessingOwnerIdentity = 55008;

        // EntityType.Instance.Deal = 199
        public const int GenerateDealNameIdentity = 19901;
        public const int SetMainLegalPersonForDealIdentity = 19902;

        // EntityType.Instance.Charge = 226
        public const int ImportChargesInfoIdentity = 22601;
        public const int DeleteChargesForPeriodAndProjectIdentity = 22602;

        public const int NotifyAboutAdvertisementElementFileChangedIdentity = 19903;
        public const int NotifyAboutAdvertisementElementValidationStatusChangedIdentity = 19904;

        // EntityType.Instance.Project = 158
        public const int ImportBranchIdentity = 15801;

        // EntityType.Instance.Territory = 191
        public const int ImportSaleTerritoryIdentity = 19101;

        // EntityType.Instance.CityPhoneZone = 233
        public const int ImportCityPhoneZoneIdentity = 23301;

        // EntityType.Instance.Reference = 234
        public const int ImportReferenceIdentity = 23401;

        // EntityType.Instance.ReferenceItem = 23501
        public const int ImportReferenceItemIdentity = 23501;

        // EntityType.Instance.Category = 160
        public const int ImportRubricIdentity = 16001;

        // EntityType.Instance.FirmContact = 165
        public const int ImportFirmContactFromServiceBusIdentity = 16501;

        // EntityType.Instance.CategoryFirmAddress = 166
        public const int ImportCategoryFirmAddressFromServiceBusIdentity = 16601;

        // EntityType.Instance.Contact = 206
        public const int RequestBirthdayCongratulationsIdentity = 20601;

        // EntityType.Instance.DepCard = 240
        public const int ImportDepCardFromServiceBusIdentity = 24001;

        // EntityType.Instance.Building = 241
        public const int ImportBuildingIdentity = 24101;

        // EntityType.Instance.Lock = 159
        public const int CreateLockDetailsDuringWithdrawalIdentity = 15901;

        public const int DeleteLockDetailsDuringRevertingWithdrawalIdentity = 15902;

        public const int ImportFirmPromising = 19905;
        public const int ImportFirmAddresses = 19907;

        // EntityType.Instance.AdvertisementElementStatus = 316
        public const int ChangeAdvertisementElementStatus = 31601;
        public const int ApproveAdvertisementElementIdentity = 31602;
        public const int DenyAdvertisementElementIdentity = 31603;
        public const int ResetAdvertisementElementToDraftIdentity = 31604;
        public const int TransferAdvertisementElementToReadyForValidationIdentity = 31605;

        // EntityType.Instance.AdvertisementElement = 187
        public const int UpdateAdvertisementElementAndSetAsReadyForVerificationIdentity = 18701;

        // EntityType.Instance.BranchOfficeOrganizationUnit = 139
        public const int SetBranchOfficeOrganizationUnitAsPrimaryIdentity = 13901;
        public const int SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity = 13902;

        // EntityType.Instance.Activity = 500
        public const int CheckRelatedActivitiesIdentity = 50001;

        // EntityType.Instance.ClientLink = 609
        public const int UpdateOrganizationStructureDenormalization = 60901;

        public const int PerformedOperationProcessingAnalysisIdentity = 19500;

        // EntityType.Instance.SalesModelCategoryRestriction = 272
        public const int ImportAdvModelInRubricInfoIdentity = 27201;
    }
}
