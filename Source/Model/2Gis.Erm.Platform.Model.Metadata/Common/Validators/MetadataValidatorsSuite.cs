using System;
using System.Text;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Validators
{
    public sealed class MetadataValidatorsSuite : IMetadataValidatorsSuite
    {
        private readonly IMetadataValidator[] _validators;

        public MetadataValidatorsSuite(IMetadataValidator[] validators)
        {
            _validators = validators;
        }

        public void Validate()
        {
            var aggregatedReport = new StringBuilder();
            foreach (var validator in _validators)
            {
                string report;
                if (!validator.IsValid(out report))
                {
                    aggregatedReport.AppendLine("Validator" + validator.GetType().Name + " detect invalid metadata settings. Details: " + report);
                }
            }

            if (aggregatedReport.Length > 0)
            {
                throw new InvalidOperationException("Detected metadata invalid settings: " + aggregatedReport);
            }
        }
    }
}