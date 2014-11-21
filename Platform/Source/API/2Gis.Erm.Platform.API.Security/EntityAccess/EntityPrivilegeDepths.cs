namespace DoubleGis.Erm.Platform.API.Security.EntityAccess
{
    // todo: пометить как Flags и посмотреть что будет
    public enum EntityPrivilegeDepthState
    {
        None = 0,

        User = 1,
        Department = 4,
        DepartmentAndChilds = 8,
        Organization = 16
    }
}