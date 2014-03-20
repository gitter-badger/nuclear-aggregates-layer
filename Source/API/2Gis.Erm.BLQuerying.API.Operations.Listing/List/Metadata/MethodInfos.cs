using System.Linq;
using System.Reflection;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public static class MethodInfos
    {
        public static class Queryable
        {
            public static readonly MethodInfo WhereMethodInfo = typeof(System.Linq.Queryable).GetMethods().First(x => x.Name == "Where");
            public static readonly MethodInfo OrderByMethodInfo = typeof(System.Linq.Queryable).GetMethods().First(x => x.Name == "OrderBy");
            public static readonly MethodInfo OrderByDescendingMethodInfo = typeof(System.Linq.Queryable).GetMethods().First(x => x.Name == "OrderByDescending");
        }

        public static class Enumerable
        {
            public static readonly MethodInfo ContainsMethodInfo = typeof(System.Linq.Enumerable).GetMethods().First(x => x.Name == "Contains");            
        }

        public static class String
        {
            public static readonly MethodInfo ContainsMethodInfo = typeof(string).GetMethods().First(x => x.Name == "Contains");
            public static readonly MethodInfo StartsWithMethodInfo = typeof(string).GetMethods().First(x => x.Name == "StartsWith");
        }
    }
}
