using System;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity
{
    public static class OperationIdentityIds
    {
        // generic operations
        public const int ActionHistoryIdentity = 1;
        public const int ActivateIdentity = 2;
        public const int AppendIdentity = 3;
        public const int AssignIdentity = 4;
        public const int ChangeClientIdentity = 5;
        public const int ChangeTerritoryIdentity = 6;
        public const int CheckForDebtsIdentity = 7;
        public const int DeactivateIdentity = 8;
        public const int DeleteIdentity = 9;
        public const int DisqualifyIdentity = 10;
        public const int DownloadIdentity = 11;
        public const int UploadIdentity = 12;
        public const int GetDomainEntityDtoIdentity = 13;
        public const int ModifyBusinessModelEntityIdentity = 14;
        public const int ListIdentity = 15;
        public const int QualifyIdentity = 16;
        public const int ListNonGenericIdentity = 19;
        public const int ModifySimplifiedModelEntityIdentity = 20;
        public const int ExportIdentity = 22;

        [Obsolete("CreateIdentity or UpdateIdentity must be used")]
        public const int CreateOrUpdateIdentity = 23;

        [Obsolete("CopyOrderIdentity or CopyPriceIdentity must be used")]
        public const int CopyIdentity = 24;
        public const int MergeIdentity = 25;
        public const int SetAsDefaultThemeIdentity = 29;
        public const int CreateIdentity = 30;
        public const int UpdateIdentity = 31;
        public const int WithdrawalIdentity = 32;
        public const int RevertWithdrawalIdentity = 33;
        public const int DetachIdentity = 34;
        public const int MsCrmExportIdentity = 35;
        public const int PrintIdentity = 36;   
        public const int BulkDeactivateIdentity = 37;
        public const int BulkActivateIdentity = 38;
        public const int BulkCreateIdentity = 39;
        public const int BulkDeleteIdentity = 40;


        // concrete operations
        public const int PrintOrderIdentity = 1000;
        public const int StartReleaseIdentity = 1005;
        public const int StartSimplifiedReleaseIdentity = 1006;
        public const int FinishReleaseIdentity = 1007;
        public const int RevertReleaseIdentity = 1008;
        public const int AttachExternalReleaseProcessingMessagesIdentity = 1009;
        public const int ValidateOrdersForReleaseIdentity = 1010;
        public const int EnsureOrdersForReleaseCompletelyExportedIdentity = 1011;
        public const int ReplicateDealStageIdentity = 1012;
        public const int UpdateAfterSaleServiceIdentity = 1013;
        public const int ActualizeAccountsDuringWithdrawalIdentity = 1014;
        public const int WithdrawFromAccountsIdentity = 1015;
        public const int ActualizeOrdersDuringWithdrawalIdentity = 1016;
        public const int ActualizeDealsDuringWithdrawalIdentity = 1017;
        public const int RevertWithdrawFromAccountsIdentity = 1018;
        public const int ActualizeAccountsDuringRevertingWithdrawalIdentity = 1019;
        public const int ActualizeOrdersDuringRevertingWithdrawalIdentity = 1020;
        public const int ActualizeDealsDuringRevertingWithdrawalIdentity = 1021;
        public const int GetFirmInfoIdentity = 1022;

        // EntityName.ReleaseWithdrawal = 215
        public const int CalculateReleaseWithdrawalsIdentity = 21501;

        // EntityName.OrderValidationResult = 232
        public const int ResetValidationGroupIdentity = 23201;

        // EntityName.Advertisement = 186
        public const int SelectAdvertisementToWhitelistIdentity = 18601;

        // EntityName.Bargain = 198
        public const int BindBargainToOrderIdentity = 19801;

        // EntityName.Firm = 146
        public const int ImportCardsFromServiceBusIdentity = 14601;
        public const int ImportFirmIdentity = 14602;
        public const int ImportTerritoriesFromDgppIdentity = 14603;
        public const int ImportFlowCardsForErmIdentity = 14604;

        // EntityName.FirmAddress = 164
        public const int SpecifyFirmAddressAdditionalServicesIdentity = 16401;

        // EntityName.LegalPerson = 147
        public const int ChangeLegalPersonRequisitesIdentity = 14701;

        // EntityName.LegalPersonProfile = 219
        public const int SetAsMainLegalPersonProfileIdentity = 21901;

        // EntityName.Order = 151
        public const int ExportAccountDetailsTo1CIdentity = 15102;
        public const int PrintRegionalOrderIdentity = 15103;
        public const int ReportsServiceIdentity = 15104;
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

        // EntityName.OrderPosition = 150
        public const int CalculateOrderPositionCostIdentity = 15001;
        public const int ValidateOrderPositionAdvertisementsIdentity = 15002;

        // Price = 155
        public const int CopyPriceIdentity = 15501;

        // EntityName.Limit = 192
        public const int CloseLimitIdentity = 19201;
        public const int ReopenLimitIdentity = 19202;

        // EntityName.HotClientRequest = 257
        public const int ImportFlowStickersIdentity = 25701;

        // EntityName.AccountDetail = 141
        public const int ImportFlowFinancialData1CIdentity = 14101;

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


        public const int NotifyAboutAdvertisementElementFileChangedIdentity = 19903;
        public const int NotifyAboutAdvertisementElementValidationStatusChangedIdentity = 19904;
    }
}
