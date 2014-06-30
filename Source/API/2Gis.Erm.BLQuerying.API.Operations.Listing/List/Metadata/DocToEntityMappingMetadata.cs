using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    interface IDocByEntitySelector<out TDocument, in TEntity>
    {
        IEnumerable<TDocument> SelectDoc(IQueryable<TEntity> query);
    }

    public static class DocToEntityMappingMetadata
    {
        public static Dictionary<Tuple<Type, Type>, Delegate> GridDocMap = new Dictionary<Tuple<Type, Type>, Delegate>()
            .RegisterGridDocMapping<Client, ClientGridDoc>(null)
            .RegisterGridDocMapping<Firm, FirmGridDoc>(null)
            ;

        private static Dictionary<Tuple<Type, Type>, Delegate> RegisterGridDocMapping<TEntity, TDocument>(this Dictionary<Tuple<Type, Type>, Delegate> map, Func<IDocByEntitySelector<TDocument, TEntity>> selector)
        {
            var key = Tuple.Create(typeof(TEntity), typeof(TDocument));
            var value = (Delegate)selector;
            map.Add(key, value);
            return map;
        }

        public static bool TryGetGridDocType(Type key, out Type docType)
        {
            var docTypeProbe = GridDocMap.Keys.Where(x => x.Item1 == key).Select(x => x.Item2).FirstOrDefault();
            if (docTypeProbe == null)
            {
                docType = null;
                return false;
            }

            docType = docTypeProbe;
            return true;
        }

        public static bool TryGetGridDocType<TEntity>(out Type docType)
        {
            var key = typeof(TEntity);
            return TryGetGridDocType(key, out docType);
        }
    }
}
