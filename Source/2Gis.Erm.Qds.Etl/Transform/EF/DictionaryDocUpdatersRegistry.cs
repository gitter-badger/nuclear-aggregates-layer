using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class DictionaryDocUpdatersRegistry : IDocUpdatersRegistry
    {
        readonly IDictionary<Type, IDocsUpdater> _updaters = new Dictionary<Type, IDocsUpdater>();

        public IDocsUpdater GetUpdater(Type docType)
        {
            if (docType == null)
            {
                throw new ArgumentNullException("docType");
            }

            return _updaters[docType];
        }

        public void AddUpdater(IDocsUpdater updater)
        {
            if (updater == null)
            {
                throw new ArgumentNullException("updater");
            }

            _updaters.Add(updater.SupportedDocType, updater);
        }
    }
}