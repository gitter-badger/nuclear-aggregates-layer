namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum OrderTerminationReason
    {
        None = 0,
        TemporaryRejectionBadTerms = 1,
        TemporaryRejectionWrongSeason = 2,
        TemporaryRejectionPlaceInOtherMedia = 3,
        TemporaryRejectionAdvertisementTypesAreUnacceptable = 4,
        TemporaryRejectionOther = 5,
        RejectionLowEfficiency = 6,
        RejectionDecisionMakerChanging = 7,
        RejectionServiceError = 8,
        RejectionLiquidation = 9,
        RejectionLoanWriteOffs = 10,
        RejectionOther = 11,
        RejectionTechnical = 12
    }
}