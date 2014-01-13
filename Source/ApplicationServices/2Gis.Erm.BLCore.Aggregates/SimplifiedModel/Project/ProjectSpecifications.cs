using DoubleGis.Erm.Platform.DAL.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Project
{
    public class ProjectSpecs
    {
        public static class Find
        {
            public static FindSpecification<Platform.Model.Entities.Erm.Project> ByCode(long code)
            {
                return new FindSpecification<Platform.Model.Entities.Erm.Project>(x => x.Code == code);
            }
        }
    }
}
