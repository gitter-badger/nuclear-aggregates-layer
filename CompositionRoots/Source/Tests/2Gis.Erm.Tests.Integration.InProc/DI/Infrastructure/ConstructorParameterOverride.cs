using System;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI.Infrastructure
{
    internal class ConstructorParameterOverride
    {
        private readonly Type _parameterType;
        private readonly object _parameterValue;

        public ConstructorParameterOverride(Type parameterType, object parameterValue)
        {
            _parameterType = parameterType;
            _parameterValue = parameterValue;
        }

        public Type ParameterType
        {
            get { return _parameterType; }
        }

        public object ParameterValue
        {
            get { return _parameterValue; }
        }
    }
}