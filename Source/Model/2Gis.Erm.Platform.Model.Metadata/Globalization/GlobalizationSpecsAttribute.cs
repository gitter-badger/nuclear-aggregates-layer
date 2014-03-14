using System;
using System.Globalization;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class GlobalizationSpecsAttribute : Attribute
    {
        private readonly BusinessModel _businessModel;
        private readonly CultureInfo[] _culturesSequence;

        public GlobalizationSpecsAttribute(BusinessModel businessModel, params string[] culturesSequence)
        {
            if (culturesSequence == null)
            {
                throw new ArgumentNullException("culturesSequence");
            }

            _businessModel = businessModel;
            _culturesSequence = culturesSequence.Select(x => new CultureInfo(x)).ToArray();
        }

        public BusinessModel BusinessModel
        {
            get { return _businessModel; }
        }

        public CultureInfo[] CulturesSequence
        {
            get { return _culturesSequence; }
        }
    }
}