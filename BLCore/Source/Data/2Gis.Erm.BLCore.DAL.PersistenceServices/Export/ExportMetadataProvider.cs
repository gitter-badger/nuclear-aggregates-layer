using System.Linq;
using System.Reflection;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public class ExportMetadataProvider : IExportMetadataProvider
    {
        public QueryRuleContainer<TEntity> GetMetadata<TEntity>() where TEntity : class, IEntity, IEntityKey
        {
            // FIXME {a.rechkalov, 16.08.2013}: А как же инкапсуляция? См LambdaExtensions.GetPropertyValue, например
            var fields = typeof(ExportMetadata).GetFields(BindingFlags.Static | BindingFlags.Public);
            var metadataField = fields.SingleOrDefault(info => typeof(QueryRuleContainer<TEntity>).IsAssignableFrom(info.FieldType));
            return metadataField == null ? null : (QueryRuleContainer<TEntity>)metadataField.GetValue(null);
        }
    }
}
