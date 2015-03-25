using System;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public static class OperationIdentityIds
    {
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

        // EntityName.ReleaseWithdrawal = 215
        public const int ActualizeOrderReleaseWithdrawalsIdentity = 21501;

        // EntityName.OrderValidationResult = 232
        public const int ResetValidationGroupIdentity = 23201;

        // EntityName.Advertisement = 186
        public const int SelectAdvertisementToWhitelistIdentity = 18601;

        // EntityName.Bargain = 198
        [Obsolete]
        public const int BindBargainToOrderIdentity = 19801;
        public const int BulkCloseClientBargainsIdentity = 19802;
        public const int DetermineOrderBargainIdentity = 19803;

        // EntityName.Firm = 146
        public const int ImportCardsFromServiceBusIdentity = 14601;
        public const int ImportFirmIdentity = 14602;
        public const int ImportTerritoriesIdentity = 14603;
        public const int ImportCardForErmIdentity = 14604;
        public const int ImportCardRelationIdentity = 14605;
        public const int CreateBlankFirmsIdentity = 14606;
        public const int ImportFirmsDuringImportCardsForErmIdentity = 14607;
        public const int ImportCardRelationForErmIdentity = 14608;

        // EntityName.FirmAddress = 164
        public const int SpecifyFirmAddressAdditionalServicesIdentity = 16401;
        public const int ImportFirmAddressFromServiceBusIdentity = 16402;

        // EntityName.LegalPerson = 147
        public const int ChangeLegalPersonRequisitesIdentity = 14701;
        public const int ValidateLegalPersonsForExportIdentity = 14702;

        // EntityName.Client = 200
        public const int CreateClientByFirmIdentity = 20001;
        public const int SetMainFirmIdentity = 20002;
        public const int CalculateClientPromisingIdentity = 20003;

        // EntityName.LegalPersonProfile = 219
        public const int SetAsMainLegalPersonProfileIdentity = 21901;

        // EntityName.Order = 151
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

        // EntityName.Bill = 188
        public const int CalculateBillsIdentity = 18801;

        // EntityName.OrderPosition = 150
        public const int CalculateOrderPositionCostIdentity = 15001;
        public const int ValidateOrderPositionAdvertisementsIdentity = 15002;
        public const int CalculateCategoryRateIdentity = 15003;
        public const int ViewOrderPositionIdentity = 15004;
        public const int ReplaceOrderPositionAdvertisementLinksIdentity = 15005;
        public const int ChangeOrderPositionBindingObjectsIdentity = 15006;
        public const int GetAvailableBinfingObjectsIdentity = 15007;

        // EntityName.Price = 155
        public const int CopyPriceIdentity = 15501;
        public const int GetRatedPricesForCategoryIdentity = 15502;
        public const int PublishPriceIdentity = 15503;
        public const int UnpublishPriceIdentity = 15504;
        public const int ReplacePriceIdentity = 15505;

        // EntityName.Price = 154
        public const int CopyPricePositionIdentity = 15401;

        // EntityName.Limit = 192
        public const int CloseLimitIdentity = 19201;
        public const int ReopenLimitIdentity = 19202;
        public const int RecalculateLimitIdentity = 19203;
        public const int SetLimitStatusIdentity = 19204;
        public const int CalculateLimitIncreasingIdentity = 19205;
        public const int IncreaseLimitIdentity = 19206;

        // EntityName.HotClientRequest = 257
        public const int ImportHotClientIdentity = 25701;
        public const int GetHotClientRequestIdentity = 25702;
        public const int ProcessHotClientRequestIdentity = 25704;
        public const int BindTaskToHotClientRequestIdentity = 25703;

        // EntityName.AccountDetail = 141
        public const int ImportOperationsInfoIdentity = 14101;
        public const int NotifyAboutAccountDetailModificationIdentity = 14102;
        public const int GetDebitsInfoInitialForExportIdentity = 14103;
        [Obsolete("Такой операции больше нет")]public const int GetAccountDetailsForExportContentIdentity = 14104;

        // EntityName.Account = 142
        public const int GetWithdrawalErrorsCsvReportIdentity = 14201;

        // EntityName.OrderProcessingRequest = 550
        public const int RequestOrderProlongationIdentity = 55001;
        public const int RequestOrderCreationIdentity = 55002;
        public const int CancelOrderProcessingRequestIdentity = 55003;
        public const int ProlongateOrderByRequestIdentity = 55004;     
        public const int CreateOrderByRequestIdentity = 55005;
        public const int ProlongateOrderForAllRequestsIdentity = 55006;
        public const int GetOrderRequestMessagesIdentity = 55007;
        public const int SelectOrderProcessingOwnerIdentity = 55008;

        // EntityName.Deal = 199
        public const int GenerateDealNameIdentity = 19901;
        public const int SetMainLegalPersonForDealIdentity = 19902;

        // EntityName.Charge = 226
        public const int ImportChargesInfoIdentity = 22601;
        public const int DeleteChargesForPeriodAndProjectIdentity = 22602;

        public const int NotifyAboutAdvertisementElementFileChangedIdentity = 19903;
        public const int NotifyAboutAdvertisementElementValidationStatusChangedIdentity = 19904;

        // EntityName.Project = 158
        public const int ImportBranchIdentity = 15801;

        // EntityName.Territory = 191
        public const int ImportSaleTerritoryIdentity = 19101;

        // EntityName.CityPhoneZone = 233
        public const int ImportCityPhoneZoneIdentity = 23301;

        // EntityName.Reference = 234
        public const int ImportReferenceIdentity = 23401;

        // EntityName.ReferenceItem = 23501
        public const int ImportReferenceItemIdentity = 23501;

        // EntityName.Category = 160
        public const int ImportRubricIdentity = 16001;

        // EntityName.FirmContact = 165
        public const int ImportFirmContactFromServiceBusIdentity = 16501;

        // EntityName.CategoryFirmAddress = 166
        public const int ImportCategoryFirmAddressFromServiceBusIdentity = 16601;

        // EntityName.Contact = 206
        public const int RequestBirthdayCongratulationsIdentity = 20601;

        // EntityName.DepCard = 240
        public const int ImportDepCardFromServiceBusIdentity = 24001;

        // EntityName.Building = 241
        public const int ImportBuildingIdentity = 24101;

        // EntityName.Lock = 159
        public const int CreateLockDetailsDuringWithdrawalIdentity = 15901;

        public const int DeleteLockDetailsDuringRevertingWithdrawalIdentity = 15902;

        public const int ImportFirmPromising = 19905;
        public const int ImportFirmAddresses = 19907;

        // EntityName.AdvertisementElementStatus = 316
        public const int ChangeAdvertisementElementStatus = 31601;
        public const int ApproveAdvertisementElementIdentity = 31602;
        public const int DenyAdvertisementElementIdentity = 31603;
        public const int ResetAdvertisementElementToDraftIdentity = 31604;
        public const int TransferAdvertisementElementToReadyForValidationIdentity = 31605;

        // EntityName.AdvertisementElement = 187
        public const int UpdateAdvertisementElementAndSetAsReadyForVerificationIdentity = 18701;

        // EntityName.BranchOfficeOrganizationUnit = 139
        public const int SetBranchOfficeOrganizationUnitAsPrimaryIdentity = 13901;
        public const int SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesIdentity = 13902;

        // EntityName.Activity = 500
        public const int CheckRelatedActivitiesIdentity = 50001;

        // EntityName.ClientLink = 609
        public const int UpdateOrganizationStructureDenormalization = 60901;

        public const int PerformedOperationProcessingAnalysisIdentity = 19500;

        // EntityName.SalesModelCategoryRestriction = 272
        public const int ImportAdvModelInRubricInfoIdentity = 27201;
    }
}
