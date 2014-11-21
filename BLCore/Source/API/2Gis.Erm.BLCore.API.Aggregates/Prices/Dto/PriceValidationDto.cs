namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto
{
    public sealed class PriceValidationDto
    {
        public bool IsPriceDeleted { get; set; }
        public bool IsPricePositionsNotValid { get; set; }
        public bool IsAssociatedPositionsNotValid { get; set; }
        public bool IsDeniedPositionsNotValid { get; set; }
    }
}