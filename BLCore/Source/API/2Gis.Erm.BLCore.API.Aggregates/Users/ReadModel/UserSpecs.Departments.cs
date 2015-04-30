using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel
{
    public static partial class UserSpecs
    {
        public static class Departments
        {
            public static class Find
            {
                public static FindSpecification<Department> ChildrensOf(Department department)
                {
                    return new FindSpecification<Department>(x => x.LeftBorder > department.LeftBorder && x.RightBorder < department.RightBorder);
                }
            }
        }
    }
}