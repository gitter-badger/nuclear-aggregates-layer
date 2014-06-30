using System;

using DoubleGis.Erm.Qds;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class SearchDescriptorPaging : ISearchDescriptorPaging
    {
        public const int PageSize = 40;

        public PagedSearchDescriptor<TDoc> CreatePage<TDoc>(IDocsQuery query) where TDoc : class
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var nestQuery = CreateQuery<TDoc>(query);
            return new PagedSearchDescriptor<TDoc>(sd => sd.Query(nestQuery), 0, PageSize);
        }

        BaseQuery CreateQuery<TDoc>(IDocsQuery query, string nestingPath = "") where TDoc : class
        {
            // FIXME {f.zaharov, 06.06.2014}: Если данный функционал выжевет, то лучше заменить на Visitor
            var valueQuery = query as FieldValueQuery;
            if (valueQuery != null)
            {
                return CreateFieldQuery<TDoc>(valueQuery, nestingPath);
            }

            var fieldInQuery = query as FieldInQuery;
            if (fieldInQuery != null)
            {
                return CreateFieldInQuery<TDoc>(fieldInQuery, nestingPath);
            }

            var objectQuery = query as NestedObjectQuery;
            if (objectQuery != null)
            {
                return CreateNestedQuery<TDoc>(objectQuery, nestingPath);
            }

            var orObjectQuery = query as OrObjectQuery;
            if (orObjectQuery != null)
            {
                return CreateOrQuery<TDoc>(orObjectQuery, nestingPath);
            }

            throw new NotSupportedException(query.GetType().FullName);
        }

        BaseQuery CreateNestedQuery<TDoc>(NestedObjectQuery query, string nestingPath = "") where TDoc : class
        {
            var nested = CreateQuery<TDoc>(query.NestedQuery, nestingPath + query.NestedObjectName + ".");

            return Query<TDoc>.Nested(n => n.Path(nestingPath + query.NestedObjectName).Query(q => nested));
        }

        BaseQuery CreateFieldQuery<TDoc>(FieldValueQuery query, string nestingPath = "") where TDoc : class
        {
            return Query<TDoc>.Term(nestingPath + query.FieldName, query.FieldValue);
        }

        BaseQuery CreateFieldInQuery<TDoc>(FieldInQuery fieldInQuery, string nestingPath) where TDoc : class
        {
            return Query<TDoc>.Terms(nestingPath + fieldInQuery.FieldName, fieldInQuery.Terms);
        }

        BaseQuery CreateOrQuery<TDoc>(OrObjectQuery query, string nestingPath = "") where TDoc : class
        {
            var left = CreateQuery<TDoc>(query.Left, nestingPath);
            var right = CreateQuery<TDoc>(query.Right, nestingPath);

            return Query<TDoc>.Filtered(fqd => fqd.Filter(fd => fd.Or(l =>l.Query(q=>left) , r =>r.Query(q=>right))));
        }

        public PagedSearchDescriptor<TDoc> NextPage<TDoc>(PagedSearchDescriptor<TDoc> pagedSearchDescriptor, ISearchResponse<TDoc> response) where TDoc : class
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