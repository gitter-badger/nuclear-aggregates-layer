namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms
{
    public sealed class AdditionalServicesDto
    {
        public string ServiceCode { get; set; }
        public string Description { get; set; }
        public AdditionalServiceDisplay DisplayService { get; set; }
    }
}
