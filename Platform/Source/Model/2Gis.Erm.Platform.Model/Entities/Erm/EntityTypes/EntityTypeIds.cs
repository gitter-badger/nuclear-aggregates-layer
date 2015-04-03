namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    internal static class EntityTypeIds
    {
        public const int LegalPerson = 147;
        public const int LegalPersonDeal = 612;
        public const int ChileLegalPersonPart = 601;
        public const int UkraineLegalPersonPart = 602;
        public const int EmiratesLegalPersonPart = 604;
        public const int KazakhstanLegalPersonPart = 613;
        public const int RussiaLegalPersonProfilePart = 259;

        public const int Order = 151;
        public const int OrderPosition = 150;
        public const int OrderFile = 202;
        public const int Deal = 199;
        public const int BranchOfficeOrganizationUnit = 139;
        public const int ChileBranchOfficeOrganizationUnitPart = 260;
        public const int EmiratesBranchOfficeOrganizationUnitPart = 607;
        public const int OperationType = 149;
        public const int AccountDetail = 141;
        public const int Price = 155;
        public const int Firm = 146;
        public const int FirmAddress = 164;
        public const int FirmDeal = 610;
        public const int EmiratesFirmAddressPart = 608;
        public const int FirmContact = 165;
        public const int BranchOffice = 156;
        public const int UkraineBranchOfficePart = 603;
        public const int OrganizationUnit = 157;
        public const int Project = 158;
        public const int Client = 200;
        public const int ClientLink = 609;
        public const int DenormalizedClientLink = 611;
        public const int EmiratesClientPart = 605;

        public const int Bargain = 198;
        public const int BargainType = 178;
        public const int BargainFile = 204;
        public const int Currency = 144;
        public const int CurrencyRate = 145;
        public const int Platform = 182;
        public const int PositionCategory = 181;
        public const int AdvertisementTemplate = 184;
        public const int PricePosition = 154;
        public const int Account = 142;
        public const int Limit = 192;
        public const int Position = 153;
        public const int PositionChildren = 183;
        public const int AssociatedPositionsGroup = 176;
        public const int AssociatedPosition = 177;
        public const int DeniedPosition = 180;
        public const int ContributionType = 179;
        public const int Category = 160;
        public const int CategoryOrganizationUnit = 161;
        public const int CategoryGroup = 162;
        public const int CategoryGroupMembership = 163;
        public const int CategoryFirmAddress = 166;
        public const int SalesModelCategoryRestriction = 272;
        public const int Country = 143;
        public const int Advertisement = 186;
        public const int AdvertisementElement = 187;
        public const int AdvertisementElementTemplate = 185;
        public const int AdvertisementElementDenialReason = 315;
        public const int AdvertisementElementStatus = 316;
        public const int AdsTemplatesAdsElementTemplate = 201;
        public const int Bill = 188;
        public const int Lock = 159;
        public const int LockDetail = 148;
        public const int RegionalAdvertisingSharing = 210;
        public const int Contact = 206;
        public const int WithdrawalInfo = 209;
        public const int ReleaseInfo = 203;
        public const int LocalMessage = 189;
        public const int PrintFormTemplate = 193;
        public const int UserTerritoriesOrganizationUnits = 213;
        public const int OrderReleaseTotal = 214;
        public const int ReleaseWithdrawal = 215;
        public const int OrderPositionAdvertisement = 216;
        public const int Operation = 217;
        public const int MessageType = 218;
        public const int LegalPersonProfile = 219;
        public const int ChileLegalPersonProfilePart = 258;
        public const int UkraineLegalPersonProfilePart = 263;
        public const int EmiratesLegalPersonProfilePart = 606;
        public const int KazakhstanLegalPersonProfilePart = 614;
        public const int AdditionalFirmService = 220;
        public const int Theme = 221;
        public const int ThemeTemplate = 222;
        public const int ThemeCategory = 223;
        public const int ThemeOrganizationUnit = 224;
        public const int ReleasesWithdrawalsPosition = 225;
        public const int Charge = 226;
        public const int ChargesHistory = 227;

        public const int ActionsHistory = 230;
        public const int ActionsHistoryDetail = 242;
        public const int AfterSaleServiceActivity = 231;
        public const int OrderValidationResult = 232;
        public const int OrderValidationCacheEntry = 271;
        public const int CityPhoneZone = 233;
        public const int Reference = 234;
        public const int ReferenceItem = 235;
        public const int CardRelation = 236;
        public const int FirmAddressService = 237;
        public const int ReleaseValidationResult = 238;
        public const int UserEntity = 239;
        public const int DepCard = 240;
        public const int Building = 241;
        
        public const int PerformedBusinessOperation = 244;
        public const int PerformedOperationPrimaryProcessing = 269;
        public const int PerformedOperationFinalProcessing = 270;
        public const int ExportFlowCardExtensionsCardCommercial = 246;
        public const int ExportFlowFinancialDataLegalEntity = 247;
        public const int ExportFlowOrdersAdvMaterial = 248;
        public const int ExportFlowOrdersOrder = 249;
        public const int ExportFlowOrdersResource = 250;
        public const int ExportFlowOrdersTheme = 251;
        public const int ExportFlowOrdersThemeBranch = 252;
        public const int ImportedFirmAddress = 253;
        public const int ExportFailedEntity = 254;
        public const int HotClientRequest = 257;
        public const int ExportFlowFinancialDataClient = 255;
        public const int ExportFlowFinancialDataDebitsInfoInitial = 273;
        public const int ExportFlowPriceListsPriceList = 261;
        public const int ExportFlowPriceListsPriceListPosition = 262;
        public const int ExportFlowOrdersInvoice = 264;

        public const int ExportFlowNomenclaturesNomenclatureElement = 265;
        public const int ExportFlowNomenclaturesNomenclatureElementRelation = 266;
        public const int ExportFlowDeliveryDataLetterSendRequest = 267;
        public const int ExportFlowOrdersDenialReason = 268;

        // security
        public const int User = 53;
        public const int UserRole = 56;
        public const int UserTerritory = 172;
        public const int UserOrganizationUnit = 174;
        public const int Department = 51;
        public const int Role = 54;
        public const int Territory = 191;
        public const int UserProfile = 211;
        public const int RolePrivilege = 170;

        // shared
        public const int File = 194;
        public const int TimeZone = 212;
        public const int Note = 207;

        // Simplified
        public const int NotificationProcessing = 300;
        public const int NotificationEmail = 301;
        public const int NotificationAddress = 302;
        public const int NotificationEmailCc = 303;
        public const int NotificationEmailTo = 304;
        public const int NotificationEmailAttachment = 305;
        public const int Bank = 310;
        public const int Commune = 311;
        public const int AcceptanceReportsJournalRecord = 312;
        public const int DenialReason = 313;
        public const int BirthdayCongratulation = 314;

        // Ambivalent
        public const int FileWithContent = 400;

        public const int Activity = 500;
        public const int Appointment = 501;
        public const int AppointmentRegardingObject = 510;
        public const int AppointmentAttendee = 511;
        public const int AppointmentOrganizer = 512;
        public const int Phonecall = 502;
        public const int PhonecallRegardingObject = 521;
        public const int PhonecallRecipient = 522;
        public const int Task = 503;
        public const int TaskRegardingObject = 531;
        public const int Letter = 504;
        public const int LetterRegardingObject = 541;
        public const int LetterSender = 542;
        public const int LetterRecipient = 543;

        // заявка на создание или продление заказа от Личного кабинета
        public const int OrderProcessingRequest = 550;
        public const int OrderProcessingRequestMessage = 551;

        // Dynamic Storage
        public const int DictionaryEntityInstance = 560;
        public const int DictionaryEntityPropertyInstance = 561;
        public const int BusinessEntityInstance = 570;
        public const int BusinessEntityPropertyInstance = 571;

        // Не имеет смысла как сущность, сделано, чтобы обдурить механизм карточки
        public const int PositionSortingOrder = 10000;
    }
}