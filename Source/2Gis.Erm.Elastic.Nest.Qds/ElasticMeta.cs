using System;

using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common.ElasticClient;
using DoubleGis.Erm.Qds.Docs;

using Nest;
using Nest.Resolvers;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class ElasticMeta : IElasticMeta
    {
        public const string DataIndexName = "data";
        public const string CatalogIndexName = "metadata";
        public const int PageSize = 40;

        private readonly IElasticConnectionSettingsFactory _settingsFactory;

        public ElasticMeta(IElasticConnectionSettingsFactory settingsFactory)
        {
            if (settingsFactory == null)
            {
                throw new ArgumentNullException("settingsFactory");
            }

            _settingsFactory = settingsFactory;
        }

        public string GetIndexName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // TODO убрать это в нужное место, как-то связанное с миграциями и метаданными.
            if (type == typeof(ClientGridDoc))
                return _settingsFactory.GetIsolatedIndexName(DataIndexName);

            return _settingsFactory.GetIsolatedIndexName(CatalogIndexName);
        }

        public string GetTypeName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.Name.MakePlural().ToLowerInvariant();
        }

        public PagedSearchDescriptor<TDoc> CreatePage<TDoc>(IDocsQuery query) where TDoc : class
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var fieldQuery = query as FieldValueQuery;
            if (fieldQuery == null)
                throw new NotSupportedException(query.GetType().FullName);

            var searchDescr = new SearchDescriptor<TDoc>().Index(GetIndexName(typeof(TDoc)))
                    .From(0)
                    .Size(PageSize)
                    .Query(qd => qd.Term(fieldQuery.FieldName, fieldQuery.FieldValue));

            return new PagedSearchDescriptor<TDoc>(searchDescr, 0, PageSize);
        }

        public PagedSearchDescriptor<TDoc> NextPage<TDoc>(PagedSearchDescriptor<TDoc> pagedSearchDescriptor, IQueryResponse<TDoc> response) where TDoc : class
        {
            if (pagedSearchDescriptor == null)
            {
                throw new ArgumentNullException("pagedSearchDescriptor");
            }
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (pagedSearchDescriptor.From + pagedSearchDescriptor.Size >= response.Total)
                return null;

            return new PagedSearchDescriptor<TDoc>(pagedSearchDescriptor.SearchDescriptor, pagedSearchDescriptor.From + PageSize, PageSize);
        }
    }
}