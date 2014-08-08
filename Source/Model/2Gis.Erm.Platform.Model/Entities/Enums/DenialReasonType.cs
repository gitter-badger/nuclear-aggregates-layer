namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum DenialReasonType
    {
        // Закон о рекламе
        LawDistanceSelling = 1,
        LawStimulatingActivities = 2,
        LawFinancialServices = 3,
        LawNewBuilding = 4,
        LawAlcohol = 5,
        LawMedicine = 6,
        LawWeapon = 7,
        LawMisleadingAdv = 8,
        LawInformationProduct = 9,
        LawForeignLanguage = 10,

        // Внутренние правила
        InternalRules = 200,

        // Другое
        Other = 300,
    }
}
