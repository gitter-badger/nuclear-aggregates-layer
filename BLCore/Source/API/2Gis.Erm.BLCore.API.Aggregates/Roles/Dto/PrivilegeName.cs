namespace DoubleGis.Erm.BLCore.API.Aggregates.Roles.Dto
{
    public sealed class PrivilegeName
    {
        public long PrivilegeId { get; set; }
        public string Name { get; set; }
        public string EntityType { get; set; }
        public bool IsFunctional { get; set; }
    }
}