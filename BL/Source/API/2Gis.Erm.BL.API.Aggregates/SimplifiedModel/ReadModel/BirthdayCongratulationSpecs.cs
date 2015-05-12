using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel
{
    public static class BirthdayCongratulationSpecs
    {
        public static class BirthdayCongratulations
        {
            public static class Find
            {
                public static FindSpecification<BirthdayCongratulation> ByCongratulationDate(DateTime congratulationDate)
                {
                    return new FindSpecification<BirthdayCongratulation>(x => x.CongratulationDate == congratulationDate);
                }
            }

            public static class Select
            {
            }
        }
    }
}