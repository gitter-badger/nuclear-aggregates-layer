using System;

namespace DoubleGis.Erm.Platform.API.Security.EntityAccess
{
    // enum синхронизован с Dynamics CRM - таблица PrincipalObjectAccess, колонка AccessRightsMask
    // значения enum FunctionalPrivilegeName не должны пересекаться со значениями в этом enum
    [Flags]
    public enum EntityAccessTypes
    {
        None = 0,

        Read = 1,
        Update = 2,
        Append = 4,
        AppendTo = 16,
        Create = 32,

        Delete = 65536,
        Share = 262144,
        Assign = 524288,

        All = Read | Update | Append | AppendTo | Create | Delete | Share | Assign,
    }
}