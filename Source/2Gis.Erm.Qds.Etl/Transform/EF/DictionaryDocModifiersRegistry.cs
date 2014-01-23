using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class DictionaryDocModifiersRegistry : IDocModifiersRegistry
    {
        readonly IDictionary<Type, IDocsSelector> _modifiers = new Dictionary<Type, IDocsSelector>();

        public IDocsSelector GetModifier(Type docType)
        {
            return _modifiers[docType];
        }

        public void AddModifier(IDocsSelector modifier)
        {
            if (modifier == null)
            {
                throw new ArgumentNullException("modifier");
            }

            _modifiers.Add(modifier.SupportedDocType, modifier);
        }
    }
}