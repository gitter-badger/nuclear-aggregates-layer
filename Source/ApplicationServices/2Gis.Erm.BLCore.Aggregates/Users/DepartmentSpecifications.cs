using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Aggregates.Users
{
    public class DepartmentSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Department> ById(long id)
            {
                return new FindSpecification<Department>(x => x.Id == id);
            }

            public static FindSpecification<Department> ChildrenDepartments(Department department)
            {
                return new FindSpecification<Department>(x => x.IsActive && !x.IsDeleted &&
                                                              x.LeftBorder > department.LeftBorder && x.RightBorder < department.RightBorder);
            }
        }
    }
}