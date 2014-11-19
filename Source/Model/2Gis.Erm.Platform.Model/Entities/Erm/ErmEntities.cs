using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public static class ErmEntities
    {
        public static Type[] Entities =
            {
    		typeof(Account),
    		typeof(AccountDetail),
    		typeof(ActionsHistory),
    		typeof(ActionsHistoryDetail),
    		typeof(AdditionalFirmService),
    		typeof(AdsTemplatesAdsElementTemplate),
    		typeof(Advertisement),
    		typeof(AdvertisementElement),
    		typeof(AdvertisementElementDenialReason),
    		typeof(AdvertisementElementStatus),
    		typeof(AdvertisementElementTemplate),
    		typeof(AdvertisementTemplate),
    		typeof(AfterSaleServiceActivity),
    		typeof(AppointmentBase),
    		typeof(AppointmentReference),
    		typeof(AssociatedPosition),
    		typeof(AssociatedPositionsGroup),
    		typeof(Bargain),
    		typeof(BargainFile),
    		typeof(BargainType),
    		typeof(Bill),
    		typeof(BirthdayCongratulation),
    		typeof(BranchOffice),
    		typeof(BranchOfficeOrganizationUnit),
    		typeof(Building),
    		typeof(BusinessEntityInstance),
    		typeof(BusinessEntityPropertyInstance),
    		typeof(BusinessOperationService),
    		typeof(CardRelation),
    		typeof(Category),
    		typeof(CategoryFirmAddress),
    		typeof(CategoryGroup),
    		typeof(CategoryOrganizationUnit),
    		typeof(Charge),
    		typeof(ChargesHistory),
    		typeof(CityPhoneZone),
    		typeof(Client),
    		typeof(ClientLink),
    		typeof(Contact),
    		typeof(ContributionType),
    		typeof(Country),
    		typeof(Currency),
    		typeof(CurrencyRate),
    		typeof(Deal),
    		typeof(DenialReason),
    		typeof(DeniedPosition),
    		typeof(DenormalizedClientLink),
    		typeof(DepCard),
    		typeof(DictionaryEntityInstance),
    		typeof(DictionaryEntityPropertyInstance),
    		typeof(ExportFailedEntity),
    		typeof(ExportFlowCardExtensionsCardCommercial),
    		typeof(ExportFlowDeliveryData_LetterSendRequest),
    		typeof(ExportFlowFinancialDataClient),
    		typeof(ExportFlowFinancialDataLegalEntity),
    		typeof(ExportFlowNomenclatures_NomenclatureElement),
    		typeof(ExportFlowNomenclatures_NomenclatureElementRelation),
    		typeof(ExportFlowOrders_DenialReason),
    		typeof(ExportFlowOrdersAdvMaterial),
    		typeof(ExportFlowOrdersInvoice),
    		typeof(ExportFlowOrdersOrder),
    		typeof(ExportFlowOrdersResource),
    		typeof(ExportFlowOrdersTheme),
    		typeof(ExportFlowOrdersThemeBranch),
    		typeof(ExportFlowPriceListsPriceList),
    		typeof(ExportFlowPriceListsPriceListPosition),
    		typeof(File),
    		typeof(Firm),
    		typeof(FirmAddress),
    		typeof(FirmAddressService),
    		typeof(FirmContact),
    		typeof(FirmDeal),
    		typeof(HotClientRequest),
    		typeof(ImportedFirmAddress),
    		typeof(LegalPerson),
    		typeof(LegalPersonDeal),
    		typeof(LegalPersonProfile),
    		typeof(Limit),
    		typeof(LocalMessage),
    		typeof(Lock),
    		typeof(LockDetail),
    		typeof(MessageType),
    		typeof(Note),
    		typeof(NotificationAddresses),
    		typeof(NotificationEmails),
    		typeof(NotificationEmailsAttachments),
    		typeof(NotificationEmailsCc),
    		typeof(NotificationEmailsTo),
    		typeof(NotificationProcessings),
    		typeof(Operation),
    		typeof(OperationType),
    		typeof(Order),
    		typeof(OrderFile),
    		typeof(OrderPosition),
    		typeof(OrderPositionAdvertisement),
    		typeof(OrderProcessingRequest),
    		typeof(OrderProcessingRequestMessage),
    		typeof(OrderReleaseTotal),
    		typeof(OrdersRegionalAdvertisingSharing),
    		typeof(OrderValidationCacheEntry),
    		typeof(OrderValidationResult),
    		typeof(OrganizationUnit),
    		typeof(PerformedBusinessOperation),
    		typeof(PerformedOperationFinalProcessing),
    		typeof(PerformedOperationPrimaryProcessing),
    		typeof(PhonecallBase),
    		typeof(PhonecallReference),
    		typeof(Platform),
    		typeof(Position),
    		typeof(PositionCategory),
    		typeof(PositionChildren),
    		typeof(Price),
    		typeof(PricePosition),
    		typeof(PrintFormTemplate),
    		typeof(Project),
    		typeof(Reference),
    		typeof(ReferenceItem),
    		typeof(RegionalAdvertisingSharing),
    		typeof(ReleaseInfo),
    		typeof(ReleasesWithdrawalsPosition),
    		typeof(ReleaseValidationResult),
    		typeof(ReleaseWithdrawal),
    		typeof(SecurityAccelerator),
    		typeof(TaskBase),
    		typeof(TaskReference),
    		typeof(Territory),
    		typeof(Theme),
    		typeof(ThemeCategory),
    		typeof(ThemeOrganizationUnit),
    		typeof(ThemeTemplate),
    		typeof(UsersDescendant),
    		typeof(UserTerritoriesOrganizationUnits),
                typeof(WithdrawalInfo)
    	};
    }
}
