using System;

namespace DoubleGis.Erm.Platform.Model
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class ContainedTypesAttribute : Attribute
    {
        private readonly Type[] _types;

        public ContainedTypesAttribute(params Type[] types)
        {
            _types = types;
        }

        public Type[] Types 
        {
            get
            {
                return _types;
            }
        }
    }
}