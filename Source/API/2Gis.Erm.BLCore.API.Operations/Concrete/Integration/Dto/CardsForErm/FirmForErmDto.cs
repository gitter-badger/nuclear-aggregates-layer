namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm
{
    public class FirmForErmDto
    {
        public long Code { get; set; }
        public string Name { get; set; }
        public int BranchCode { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public bool IsActive { get; set; }
    }
}