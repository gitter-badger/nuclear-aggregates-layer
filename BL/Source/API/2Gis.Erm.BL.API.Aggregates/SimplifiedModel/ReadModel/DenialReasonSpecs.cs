using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel
{
    public static class DenialReasonSpecs
    {
        public static class DenialReasons
        {
            public static class Find
            {
                public static FindSpecification<DenialReason> DuplicateByName(long denialReasonId, string name)
                {
                    return new FindSpecification<DenialReason>(x => x.Id != denialReasonId && x.Name == name);
                }
            }

            public static class Select
            {
            }
        }
    }
}