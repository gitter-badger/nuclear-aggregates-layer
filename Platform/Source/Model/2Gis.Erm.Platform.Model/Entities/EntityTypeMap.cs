using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Russia;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.Entities
{
    public static class EntityTypeMap
    {
        private static readonly Dictionary<EntityName, Type> TypeMap = new Dictionary<EntityName, Type>
            {
                // ERM
                { EntityName.Deal, typeof(Deal) },
                { EntityName.BranchOfficeOrganizationUnit, typeof(BranchOfficeOrganizationUnit) },
                { EntityName.ChileBranchOfficeOrganizationUnitPart, typeof(ChileBranchOfficeOrganizationUnitPart) },
                { EntityName.EmiratesBranchOfficeOrganizationUnitPart, typeof(EmiratesBranchOfficeOrganizationUnitPart) },
                { EntityName.LegalPerson, typeof(LegalPerson) },
                { EntityName.LegalPersonDeal, typeof(LegalPersonDeal) },
                { EntityName.ChileLegalPersonPart, typeof(ChileLegalPersonPart) },
                { EntityName.UkraineLegalPersonPart, typeof(UkraineLegalPersonPart) },
                { EntityName.EmiratesLegalPersonPart, typeof(EmiratesLegalPersonPart) },
                { EntityName.KazakhstanLegalPersonPart, typeof(KazakhstanLegalPersonPart) },
                { EntityName.KazakhstanLegalPersonProfilePart, typeof(KazakhstanLegalPersonProfilePart) },
                { EntityName.OperationType, typeof(OperationType) },
                { EntityName.Order, typeof(Order) },
                { EntityName.OrderPosition, typeof(OrderPosition) },
                { EntityName.OrderProcessingRequest, typeof(OrderProcessingRequest) },
                { EntityName.OrderProcessingRequestMessage, typeof(OrderProcessingRequestMessage) },
                { EntityName.OrderFile, typeof(OrderFile) },
                { EntityName.AccountDetail, typeof(AccountDetail) },
                { EntityName.Price, typeof(Price) },
                { EntityName.Firm, typeof(Firm) },
                { EntityName.FirmDeal, typeof(FirmDeal) },
                { EntityName.FirmAddress, typeof(FirmAddress) },
                { EntityName.EmiratesFirmAddressPart, typeof(EmiratesFirmAddressPart) },
                { EntityName.FirmContact, typeof(FirmContact) },
                { EntityName.BranchOffice, typeof(BranchOffice) },
                { EntityName.UkraineBranchOfficePart, typeof(UkraineBranchOfficePart) },
                { EntityName.OrganizationUnit, typeof(OrganizationUnit) },
                { EntityName.Project, typeof(Project) },
                { EntityName.Client, typeof(Client) },
                { EntityName.ClientLink, typeof(ClientLink) },
                { EntityName.DenormalizedClientLink, typeof(DenormalizedClientLink) },
                { EntityName.EmiratesClientPart, typeof(EmiratesClientPart) },
                { EntityName.Bargain, typeof(Bargain) },
                { EntityName.BargainType, typeof(BargainType) },
                { EntityName.BargainFile, typeof(BargainFile) },
                { EntityName.Currency, typeof(Currency) },
                { EntityName.CurrencyRate, typeof(CurrencyRate) },
                { EntityName.Platform, typeof(Erm.Platform) },
                { EntityName.PositionCategory, typeof(PositionCategory) },
                { EntityName.PricePosition, typeof(PricePosition) },
                { EntityName.Account, typeof(Account) },
                { EntityName.Limit, typeof(Limit) },
                { EntityName.Position, typeof(Position) },
                { EntityName.PositionChildren, typeof(PositionChildren) },
                { EntityName.AssociatedPositionsGroup, typeof(AssociatedPositionsGroup) },
                { EntityName.AssociatedPosition, typeof(AssociatedPosition) },
                { EntityName.DeniedPosition, typeof(DeniedPosition) },
                { EntityName.ContributionType, typeof(ContributionType) },
                { EntityName.Category, typeof(Category) },
                { EntityName.CategoryOrganizationUnit, typeof(CategoryOrganizationUnit) },
                { EntityName.CategoryGroup, typeof(CategoryGroup) },
                { EntityName.CategoryFirmAddress, typeof(CategoryFirmAddress) },
                { EntityName.SalesModelCategoryRestriction, typeof(SalesModelCategoryRestriction) },
                { EntityName.Country, typeof(Country) },
                { EntityName.Advertisement, typeof(Advertisement) },
                { EntityName.AdvertisementTemplate, typeof(AdvertisementTemplate) },
                { EntityName.AdvertisementElement, typeof(AdvertisementElement) },
                { EntityName.AdvertisementElementDenialReason, typeof(AdvertisementElementDenialReason) },
                { EntityName.AdvertisementElementStatus, typeof(AdvertisementElementStatus) },
                { EntityName.AdvertisementElementTemplate, typeof(AdvertisementElementTemplate) },
                { EntityName.AdsTemplatesAdsElementTemplate, typeof(AdsTemplatesAdsElementTemplate) },
                { EntityName.Bill, typeof(Bill) },
                { EntityName.Lock, typeof(Lock) },
                { EntityName.LockDetail, typeof(LockDetail) },
                { EntityName.Contact, typeof(Contact) },
                { EntityName.WithdrawalInfo, typeof(WithdrawalInfo) },
                { EntityName.ReleaseInfo, typeof(ReleaseInfo) },
                { EntityName.LocalMessage, typeof(LocalMessage) },
                { EntityName.PrintFormTemplate, typeof(PrintFormTemplate) },
                { EntityName.TimeZone, typeof(Security.TimeZone) },
                { EntityName.UserTerritoriesOrganizationUnits, typeof(UserTerritoriesOrganizationUnits) },
                { EntityName.File, typeof(File) },
                { EntityName.OrderReleaseTotal, typeof(OrderReleaseTotal) },
                { EntityName.ReleaseWithdrawal, typeof(ReleaseWithdrawal) },
                { EntityName.OrderPositionAdvertisement, typeof(OrderPositionAdvertisement) },
                { EntityName.Note, typeof(Note) },
                { EntityName.Operation, typeof(Operation) },
                { EntityName.MessageType, typeof(MessageType) },
                { EntityName.LegalPersonProfile, typeof(LegalPersonProfile) },
                { EntityName.ChileLegalPersonProfilePart, typeof(ChileLegalPersonProfilePart) },
                { EntityName.RussiaLegalPersonProfilePart, typeof(RussiaLegalPersonProfilePart) },
                { EntityName.UkraineLegalPersonProfilePart, typeof(UkraineLegalPersonProfilePart) },
                { EntityName.EmiratesLegalPersonProfilePart, typeof(EmiratesLegalPersonProfilePart) },
                { EntityName.Theme, typeof(Theme) },
                { EntityName.ThemeCategory, typeof(ThemeCategory) },
                { EntityName.ThemeOrganizationUnit, typeof(ThemeOrganizationUnit) },
                { EntityName.ThemeTemplate, typeof(ThemeTemplate) },
                { EntityName.ActionsHistory, typeof(ActionsHistory) },
                { EntityName.ActionsHistoryDetail, typeof(ActionsHistoryDetail) },
                { EntityName.CityPhoneZone, typeof(CityPhoneZone) },
                { EntityName.Reference, typeof(Reference) },
                { EntityName.ReferenceItem, typeof(ReferenceItem) },
                { EntityName.CardRelation, typeof(CardRelation) },
                { EntityName.DepCard, typeof(DepCard) },
                { EntityName.ReleaseValidationResult, typeof(ReleaseValidationResult) },
                { EntityName.ReleasesWithdrawalsPosition, typeof(ReleasesWithdrawalsPosition) },
                { EntityName.Charge, typeof(Charge) },
                { EntityName.ChargesHistory, typeof(ChargesHistory) },
                { EntityName.Building, typeof(Building) },

                // Activity subsystem
                { EntityName.Activity, typeof(Activity.Activity) },
                { EntityName.Appointment, typeof(Appointment) },
                { EntityName.AppointmentRegardingObject, typeof(AppointmentRegardingObject) },
                { EntityName.AppointmentAttendee, typeof(AppointmentAttendee) },
                { EntityName.AppointmentOrganizer,typeof(AppointmentOrganizer)},
                { EntityName.Phonecall, typeof(Phonecall) },
                { EntityName.PhonecallRegardingObject, typeof(PhonecallRegardingObject) },
                { EntityName.PhonecallRecipient, typeof(PhonecallRecipient) },
                { EntityName.Task, typeof(Task) },
                { EntityName.TaskRegardingObject, typeof(TaskRegardingObject) },
                { EntityName.Letter, typeof(Letter) },
                { EntityName.LetterRegardingObject, typeof(LetterRegardingObject) },
                { EntityName.LetterSender, typeof(LetterSender) },
                { EntityName.LetterRecipient, typeof(LetterRecipient) },

                // Security
                { EntityName.User, typeof(User) },
                { EntityName.UserRole, typeof(UserRole) },
                { EntityName.UserTerritory, typeof(UserTerritory) },
                { EntityName.UserOrganizationUnit, typeof(UserOrganizationUnit) },
                { EntityName.UserBranchOffice, typeof(UserBranchOffice) },
                { EntityName.Department, typeof(Department) },
                { EntityName.Role, typeof(Role) },
                { EntityName.Territory, typeof(Territory) },
                { EntityName.RolePrivilege, typeof(RolePrivilege) },
                { EntityName.UserProfile, typeof(UserProfile) },

                // Simplified
                { EntityName.NotificationProcessing, typeof(NotificationProcessings) },
                { EntityName.NotificationEmail, typeof(NotificationEmails) },
                { EntityName.NotificationAddress, typeof(NotificationAddresses) },
                { EntityName.NotificationEmailCc, typeof(NotificationEmailsCc) },
                { EntityName.NotificationEmailTo, typeof(NotificationEmailsTo) },
                { EntityName.NotificationEmailAttachment, typeof(NotificationEmailsAttachments) },
                { EntityName.FileWithContent, typeof(FileWithContent) },
                { EntityName.HotClientRequest, typeof(HotClientRequest) },
                { EntityName.PerformedBusinessOperation, typeof(PerformedBusinessOperation) },
                { EntityName.PerformedOperationPrimaryProcessing, typeof(PerformedOperationPrimaryProcessing) },
                { EntityName.PerformedOperationFinalProcessing, typeof(PerformedOperationFinalProcessing) },
                { EntityName.ExportFlowFinancialDataLegalEntity, typeof(ExportFlowFinancialDataLegalEntity) },
                { EntityName.ExportFlowOrdersAdvMaterial, typeof(ExportFlowOrdersAdvMaterial) },
                { EntityName.ExportFlowOrdersOrder, typeof(ExportFlowOrdersOrder) },
                { EntityName.ExportFlowOrdersResource, typeof(ExportFlowOrdersResource) },
                { EntityName.ExportFlowOrdersTheme, typeof(ExportFlowOrdersTheme) },
                { EntityName.ExportFlowOrdersThemeBranch, typeof(ExportFlowOrdersThemeBranch) },
                { EntityName.ExportFlowFinancialDataClient, typeof(ExportFlowFinancialDataClient) },
                { EntityName.ExportFlowFinancialDataDebitsInfoInitial, typeof(ExportFlowFinancialDataDebitsInfoInitial) },
                { EntityName.ExportFlowPriceListsPriceList, typeof(ExportFlowPriceListsPriceList) },
                { EntityName.ExportFlowPriceListsPriceListPosition, typeof(ExportFlowPriceListsPriceListPosition) },
                { EntityName.ExportFlowOrdersInvoice, typeof(ExportFlowOrdersInvoice) },
                { EntityName.ExportFlowNomenclaturesNomenclatureElement, typeof(ExportFlowNomenclatures_NomenclatureElement) },
                { EntityName.ExportFlowNomenclaturesNomenclatureElementRelation, typeof(ExportFlowNomenclatures_NomenclatureElementRelation) },
                { EntityName.ExportFlowDeliveryDataLetterSendRequest, typeof(ExportFlowDeliveryData_LetterSendRequest) },
                { EntityName.ExportFlowOrdersDenialReason, typeof(ExportFlowOrders_DenialReason) },
                { EntityName.ExportFailedEntity, typeof(ExportFailedEntity) },
                { EntityName.ImportedFirmAddress, typeof(ImportedFirmAddress) },
                { EntityName.UserEntity, typeof(UserEntity) },
                { EntityName.Bank, typeof(Bank) },
                { EntityName.Commune, typeof(Commune) },
                { EntityName.AcceptanceReportsJournalRecord, typeof(AcceptanceReportsJournalRecord) },
                { EntityName.DenialReason, typeof(DenialReason) },
                { EntityName.BirthdayCongratulation, typeof(BirthdayCongratulation) },
                { EntityName.OrderValidationResult, typeof(OrderValidationResult) },
                { EntityName.OrderValidationCacheEntry, typeof(OrderValidationCacheEntry) },

                // Dynamic Storage
                { EntityName.DictionaryEntityInstance, typeof(DictionaryEntityInstance) },
                { EntityName.DictionaryEntityPropertyInstance, typeof(DictionaryEntityPropertyInstance) },
                { EntityName.BusinessEntityInstance, typeof(BusinessEntityInstance) },
                { EntityName.BusinessEntityPropertyInstance, typeof(BusinessEntityPropertyInstance) },
               
            };

        private static readonly Dictionary<Type, EntityName> ReverseTypeMap = TypeMap.ToDictionary(x => x.Value, x => x.Key);

        public static IReadOnlyDictionary<EntityName, Type> EntitiesMapping
        {
            get { return TypeMap; }
        }

        public static Type AsEntityType(this EntityName entityName)
        {
            Type type;
            if (!entityName.TryGetEntityType(out type))
            {
                throw new ArgumentException(string.Format("Cannot find type mapped to EntityName {0}", entityName.ToString()));
            }

            return type;
        }

        public static EntityName AsEntityName(this Type entityType)
        {
            EntityName entityName;
            if (!entityType.TryGetEntityName(out entityName))
            {
                throw new ArgumentException(string.Format("Cannot find EntityName mapped to type {0}", entityType.Name));
            }

            return entityName;
         }

        public static bool TryGetEntityType(this EntityName entityName, out Type entityType)
        {
            entityType = null;
            return !entityName.IsVirtual() && TypeMap.TryGetValue(entityName, out entityType);
        }

        public static bool TryGetEntityName(this Type type, out EntityName entityName)
        {
            entityName = EntityName.None;
            return !type.IsPersistenceOnly() && ReverseTypeMap.TryGetValue(type, out entityName);
        }

        public static EntityName[] Convert2EntityNames(params Type[] entitiesTypes)
        {
            var sb = new StringBuilder();
            var entityNames = new EntityName[entitiesTypes.Length];
            for (var index = 0; index < entitiesTypes.Length; index++)
            {
                var entitiesType = entitiesTypes[index];
                EntityName entityName;
                if (!entitiesType.TryGetEntityName(out entityName))
                {
                    sb.AppendFormat(
                        "Can't find entity name for specified type {0}. Entity type is persistance only:{1}. Check domain model", 
                        entitiesType,
                        entitiesType.IsPersistenceOnly());
                }

                entityNames[index] = entityName;
            }

            if (sb.Length > 0)
            {
                throw new InvalidOperationException(sb.ToString());
            }

            return entityNames;
        }

        public static EntityName[] AsEntityNames(this Type[] entitiesTypes)
        {
            return Convert2EntityNames(entitiesTypes);
        }

        public static EntitySet AsEntitySet(this Type[] entitiesTypes)
        {
            return Convert2EntityNames(entitiesTypes).ToEntitySet();
        }
    }
}
