using System;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class DependencyScope : Attribute
    {
        private readonly string _scopeName;

        public DependencyScope(string scopeName)
        {
            _scopeName = scopeName;
        }

        public string ScopeName
        {
            get { return _scopeName; }
        }
    }
}
