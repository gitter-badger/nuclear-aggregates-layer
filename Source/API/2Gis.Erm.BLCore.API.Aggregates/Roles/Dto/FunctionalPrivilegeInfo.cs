namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto
{
    public sealed class FunctionalPrivilegeInfo
    {
        public long PrivilegeId { get; set; }
        public string NameLocalized { get; set; }

        public int Mask { get; set; }
        public byte Priority { get; set; }
    }
}