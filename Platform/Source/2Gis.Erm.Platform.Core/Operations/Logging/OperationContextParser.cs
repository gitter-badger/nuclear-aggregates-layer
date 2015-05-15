using System.IO;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils.Xml;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationContextParser : IOperationContextParser
    {
        public bool TryParse(string context, out EntityChangesContext changes, out string report)
        {
            changes = new EntityChangesContext();
            report = null;

            var sb = new StringBuilder();
            using (var reader = new StringReader(context))
            {
                var element = XElement.Load(reader);
                foreach (var entity in element.Elements("entity"))
                {
                    ChangesType changeType;
                    IEntityType entityName;
                    long id;

                    string parseAttributecReport;
                    if (!entity.TryGetAttributeValue("id", out id, out parseAttributecReport))
                    {
                        sb.AppendLine(parseAttributecReport);
                    }

                    if (!entity.TryGetAttributeValue("change", out changeType, out parseAttributecReport))
                    {
                        sb.AppendLine(parseAttributecReport);
                    }

                    if (!entity.TryGetAttributeValue("type", out entityName, out parseAttributecReport))
                    {
                        sb.AppendLine(parseAttributecReport);
                    }

                    if (sb.Length > 0)
                    {
                        report = sb.Length > 0 ? sb.ToString() : null;
                        return false;
                    }

                    var entityType = entityName.AsEntityType();
                    switch (changeType)
                    {
                        case ChangesType.Added:
                        {
                            changes.Added(entityType, new[] { id });
                            break;
                        }
                        case ChangesType.Updated:
                        {
                            changes.Updated(entityType, new[] { id });
                            break;
                        }
                        case ChangesType.Deleted:
                        {
                            changes.Deleted(entityType, new[] { id });
                            break;
                        }
                    }
                }
            }

            return true;
        }
    }
}