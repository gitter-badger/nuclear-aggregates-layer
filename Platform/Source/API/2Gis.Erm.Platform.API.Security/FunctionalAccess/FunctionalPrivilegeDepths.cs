namespace DoubleGis.Erm.Platform.API.Security.FunctionalAccess
{
    // эти enums нужны для тех привилегий, у которых определено понятие глубины (территория, подразделение и т.д.)
    // 'Security.FunctionalPrivilegeDepths' 'Mask'

    public enum LegalPersonChangeRequisitesAccess
    {
        None = 0,
        GrantedLimited = 121,
        Granted = 111,
    }

    public enum MergeClientsAccess
    {
        None = 0,
        User = 116,
        Department = 117,
        DepartmentWithChilds = 118,
        Full = 119
    }

    public enum MergeLegalPersonsAccess
    {
        None = 0,
        Granted = 134
    }

    public enum OrderChangeDocumentsDebtAccess
    {
        None = 0,
        OrganizationUnit = 113,
        Full = 114
    }

    public enum ReserveAccess
    {
        None = 0,
        Territory = 101,
        OrganizationUnit = 102,
        Full = 103
    }

    public enum WithdrawalAccess
    {
        None = 0,
        OrganizationUnit = 123,
        Full = 124
    }
}
