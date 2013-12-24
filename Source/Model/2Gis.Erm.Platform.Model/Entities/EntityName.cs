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
        Order = 151,
        OrderPosition = 150,
        OrderFile = 202,
        Deal = 199,
        BranchOfficeOrganizationUnit = 139,
        OperationType = 149,
        AccountDetail = 141,
        Price = 155,
        Firm = 146,
        FirmAddress = 164,
        FirmContact = 165,
        BranchOffice = 156,
        OrganizationUnit = 157,
        Project = 158,
        Client = 200,
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
        AdsTemplatesAdsElementTemplate = 201,
        Bill = 188,
        Lock = 159,
        LockDetail = 148,
        RegionalAdvertisingSharing = 210,
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
        AdditionalFirmService = 220,
        Theme = 221,
        ThemeTemplate = 222,
        ThemeCategory = 223,
        ThemeOrganizationUnit = 224,
        ReleasesWithdrawalsPosition = 225,

        ActionsHistory = 230, 
        ActionsHistoryDetail = 242, 
        AfterSaleServiceActivity = 231,
        OrderValidationResult = 232,
        CityPhoneZone = 233,
        Reference = 234,
        ReferenceItem = 235,
        CardRelation = 236,
        FirmAddressService = 237,
        ReleaseValidationResult = 238, 
        UserEntity = 239,
        DepCard = 240,
        
        PerformedBusinessOperation = 244,
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
        ExportToMsCrmHotClients = 256,
        ExportFlowFinancialDataClient = 255,

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

        // Ambivalent
        FileWithContent = 400,

        ActivityInstance = 498,
        ActivityPropertyInstance = 499,
        ActivityBase = 500,
        Appointment = 501,
        Phonecall = 502,
        Task = 503,

        // заявка на создание или продление заказа от Личного кабинета
        OrderProcessingRequest = 550,
        OrderProcessingRequestMessage = 551
    }
}
