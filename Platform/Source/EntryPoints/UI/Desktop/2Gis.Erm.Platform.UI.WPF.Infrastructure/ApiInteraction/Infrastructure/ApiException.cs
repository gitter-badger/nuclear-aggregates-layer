using System;
using System.Text;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public sealed class ApiException : Exception
    {
        public ApiException(string message)
            : this(message, null)
        {
        }

        public ApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ApiExceptionDescriptor ApiExceptionDescription { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (ApiExceptionDescription != null)
            {
                sb.AppendLine("ApiExceptionDescription:");
                sb.AppendLine(ApiExceptionDescription.ToString());
            }

            sb.AppendLine(base.ToString());
            return sb.ToString();
        }
    }
}
