﻿namespace DoubleGis.Erm.Platform.Model.Entities
{
    public enum EntityName
    {
        // composed значения, описывающие группу элементарных EntityName
        /// <summary>
        /// пустое значение - т.е. ему не соответствует никакой реальный EntityName
        /// </summary>
        None = 0,
        /// <summary>
        /// значение объединяет в группу все значения EntityName не являющиеся composed - т.е. все элементарные
        /// </summary>
        All = 1,

        // erm
        LegalPerson = 147,
        LegalPersonDeal = 612,
        ChileLegalPersonPart = 601,
        UkraineLegalPersonPart = 602,
        EmiratesLegalPersonPart = 604,
        KazakhstanLegalPersonPart = 613,

        Order = 151,
        OrderPosition = 150,
        OrderFile = 202,
        Deal = 199,
        BranchOfficeOrganizationUnit = 139,
        ChileBranchOfficeOrganizationUnitPart = 260,
        EmiratesBranchOfficeOrganizationUnitPart = 607,
        OperationType = 149,
        AccountDetail = 141,
        Price = 155,
        Firm = 146,
        FirmAddress = 164,
        FirmDeal = 610,
        EmiratesFirmAddressPart = 608,
        FirmContact = 165,
        BranchOffice = 156,
        UkraineBranchOfficePart = 603,
        OrganizationUnit = 157,
        Project = 158,
        Client = 200,
        ClientLink = 609,
        DenormalizedClientLink = 611,
        EmiratesClientPart = 605,

        Bargain = 198,
        BargainType = 178,
        BargainFile = 204,
        Currency = 144,
        CurrencyRate = 145,
        Platform = 182,
        PositionCategory = 181,
        AdvertisementTemplate = 184,
        PricePosition = 154,
        Account = 142,
        Limit = 192,
        Position = 153,
        PositionChildren = 183,
        AssociatedPositionsGroup = 176,
        AssociatedPosition = 177,
        DeniedPosition = 180,
        ContributionType = 179,
        Category = 160,
        CategoryOrganizationUnit = 161,
        CategoryGroup = 162,
        CategoryGroupMembership = 163,
        CategoryFirmAddress = 166,
        Country = 143,
        Advertisement = 186,
        AdvertisementElement = 187,
        AdvertisementElementTemplate = 185,
        AdvertisementElementDenialReason = 315,
        AdvertisementElementStatus = 316,
        AdsTemplatesAdsElementTemplate = 201,
        Bill = 188,
        Lock = 159,
        LockDetail = 148,
        Contact = 206,
        WithdrawalInfo = 209,
        ReleaseInfo = 203,
        LocalMessage = 189,
        PrintFormTemplate = 193,
        UserTerritoriesOrganizationUnits = 213,
        OrderReleaseTotal = 214,
        ReleaseWithdrawal = 215,
        OrderPositionAdvertisement = 216,
        Operation = 217,
        MessageType = 218,
        LegalPersonProfile = 219,
        ChileLegalPersonProfilePart = 258,
        RussiaLegalPersonProfilePart = 259,
        UkraineLegalPersonProfilePart = 263,
        EmiratesLegalPersonProfilePart = 606,
        KazakhstanLegalPersonProfilePart = 614,
        AdditionalFirmService = 220,
        Theme = 221,
        ThemeTemplate = 222,
        ThemeCategory = 223,
        ThemeOrganizationUnit = 224,
        ReleasesWithdrawalsPosition = 225,
        Charge = 226,
        ChargesHistory = 227,

        ActionsHistory = 230, 
        ActionsHistoryDetail = 242, 
        
        OrderValidationResult = 232,
        OrderValidationCacheEntry = 271,
        CityPhoneZone = 233,
        Reference = 234,
        ReferenceItem = 235,
        CardRelation = 236,
        FirmAddressService = 237,
        ReleaseValidationResult = 238, 
        UserEntity = 239,
        DepCard = 240,
        Building = 241,
        SalesModelCategoryRestriction = 272,

        PerformedBusinessOperation = 244,
        PerformedOperationPrimaryProcessing = 269,
        PerformedOperationFinalProcessing = 270,
        ExportFlowCardExtensionsCardCommercial = 246,
        ExportFlowFinancialDataLegalEntity = 247,
        ExportFlowOrdersAdvMaterial = 248,
        ExportFlowOrdersOrder = 249,
        ExportFlowOrdersResource = 250,
        ExportFlowOrdersTheme = 251,
        ExportFlowOrdersThemeBranch = 252,
        ImportedFirmAddress = 253,
        ExportFailedEntity = 254,
        HotClientRequest = 257,
        ExportFlowFinancialDataClient = 255,
        ExportFlowFinancialDataDebitsInfoInitial = 273,
        ExportFlowPriceListsPriceList = 261,
        ExportFlowPriceListsPriceListPosition = 262,
        ExportFlowOrdersInvoice = 264,
        
        ExportFlowNomenclaturesNomenclatureElement = 265,
        ExportFlowNomenclaturesNomenclatureElementRelation = 266,
        ExportFlowDeliveryDataLetterSendRequest = 267,
        ExportFlowOrdersDenialReason = 268,

        // security
        User = 53,
        UserRole = 56,
        UserTerritory = 172,
        UserOrganizationUnit = 174,
        Department = 51,
        Role = 54,
        Territory = 191,
        UserProfile = 211,
        RolePrivilege = 170,
        
        // shared
        File = 194,
        TimeZone = 212,
        Note = 207,

        // Simplified
        NotificationProcessing = 300,
        NotificationEmail = 301,
        NotificationAddress = 302,
        NotificationEmailCc = 303,
        NotificationEmailTo = 304,
        NotificationEmailAttachment = 305,
        Bank = 310,
        Commune = 311,
        AcceptanceReportsJournalRecord = 312,
        DenialReason = 313,
        BirthdayCongratulation = 314,

        // Ambivalent
        FileWithContent = 400,

        Activity = 500,
        Appointment = 501,
        AppointmentRegardingObject = 510,
        AppointmentAttendee = 511,
        AppointmentOrganizer = 512,
        Phonecall = 502,
        PhonecallRegardingObject = 521,
        PhonecallRecipient = 522,
        Task = 503,
        TaskRegardingObject = 531,
        Letter = 504,
        LetterRegardingObject = 541,
        LetterSender = 542,
        LetterRecipient = 543,

        // заявка на создание или продление заказа от Личного кабинета
        OrderProcessingRequest = 550,
        OrderProcessingRequestMessage = 551,

        // Dynamic Storage
        DictionaryEntityInstance = 560,
        DictionaryEntityPropertyInstance = 561,
        BusinessEntityInstance = 570,
        BusinessEntityPropertyInstance = 571,

        // Не имеет смысла как сущность, сделано, чтобы обдурить механизм карточки
        PositionSortingOrder = 10000,
    }
}
