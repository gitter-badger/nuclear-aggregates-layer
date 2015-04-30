using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel
{
    public static partial class AccountSpecs
    {
        public static class OperationTypes
        {
            public static class Find
            {
                public static FindSpecification<OperationType> BySyncCode1C(string syncCode1C)
                {
                    return new FindSpecification<OperationType>(x => x.SyncCode1C == syncCode1C);
                }
            }
        }
    }
}