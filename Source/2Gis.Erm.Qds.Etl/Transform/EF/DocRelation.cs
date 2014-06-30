using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public class DocRelation : IDocRelation
    {
        public Type DocType { get; private set; }
        private readonly List<Link> _links;

        public DocRelation(Type docType, params Link[] links)
        {
            if (docType == null)
            {
                throw new ArgumentNullException("docType");
            }

            DocType = docType;
            _links = new List<Link>(links);
        }

        public IEnumerable<Type> GetPartTypes()
        {
            return _links.Select(x => x.PartType);
        }

        public DocRelation LinkPart<TPart>(IDocsQueryBuilder queryBuilder)
        {
            if (queryBuilder == null)
            {
                throw new ArgumentNullException("queryBuilder");
            }

            _links.Add(new Link(typeof(TPart), queryBuilder));

            return this;
        }

        public static DocRelation ForDoc<TDoc>(params Link[] links)
        {
            return new DocRelation(typeof(TDoc), links);
        }

        public IDocsQuery GetByPartQuery(IEntityKey part)
        {
            if (part == null)
            {
                throw new ArgumentNullException("part");
            }

            var type = part.GetType();

            var link = _links.FirstOrDefault(l => l.PartType == type);
            if (link == null)
                throw new NotSupportedException(string.Format("No link between '{0}' and '{1}'.", DocType.FullName, type.FullName));

            return link.QueryBuilder.CreateQuery(part);
        }
    }
}