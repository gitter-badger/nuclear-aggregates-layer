namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients
{
    public class HotClientTaskDto
    {
        public long TaskOwner { get; set; }
        public HotClientRequestDto HotClientDto { get; set; }
        public RegardingObject Regarding { get; set; }
    }
}