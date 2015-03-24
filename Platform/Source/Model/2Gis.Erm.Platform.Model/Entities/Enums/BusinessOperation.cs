using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum BusinessOperation
    {
        None = 0,

        Create = 1,
        Read = 2,
        Edit = 3,
        Delete = 4,
        Assign = 5,
        Activate = 6,
        Deactivate = 7,

        Qualify = 100,
        Disqualify = 101,
        ChangeTerritory = 102,
        ChangeClient = 103,
        Cancel = 104,

        [Obsolete] AfterSaleServiceActivitiesCreation = 150,
        ExportAccountDetailsTo1CForFranchisees = 151,
        CheckOrdersReadinessForRelease = 152,
        
        [Obsolete]
        MakeRegionalAdsDocs = 153,

        GetOrdersWithDummyAdvertisements = 160,
        Withdrawal = 170
    }

    [Obsolete("Слит в BusinessOperation. Пока оставлен как хранилище старых значений элементов enum, если придется менять значения, чтобы реализовать конвертацию. См табл. Shared.Operations")]
    public enum OldBusinessOperationType
    {
        None = 0,
        Qualify = 100,
        Disqualify = 101,
        ChangeTerritory = 102,
        ChangeClient = 103,

        [Obsolete] AfterSaleServiceActivitiesCreation = 150,
        ExportAccountDetailsTo1CForFranchisees = 151,
        CheckOrdersReadinessForRelease = 152,
        MakeRegionalAdsDocs = 153,

        GetOrdersWithDummyAdvertisements = 160,
    }

    [Obsolete("Слит в BusinessOperation. Пока оставлен как хранилище старых значений элементов enum, если придется менять значения, чтобы реализовать конвертацию.")]
    public enum OldEntityOperation
    {
        Create = 1,
        Read = 2,
        Edit = 3,
        Delete = 4,
        Assign = 5,
        Activate = 6,
        Deactivate = 7
    }
}