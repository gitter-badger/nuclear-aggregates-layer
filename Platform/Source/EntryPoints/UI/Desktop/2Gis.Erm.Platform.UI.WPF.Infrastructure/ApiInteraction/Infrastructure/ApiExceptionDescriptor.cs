using System.Text;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure
{
    public sealed class ApiExceptionDescriptor
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Title: " + Title);
            sb.AppendLine("Description: " + Description);
            return sb.ToString();
        }
    }
}
