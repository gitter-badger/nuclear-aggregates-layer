using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients
{
    public class HotClientTaskDto
    {
        public UserDto TaskOwner { get; set; }
        public HotClientRequestDto HotClientDto { get; set; }
        public RegardingObject Regarding { get; set; }
    }
}