using System;

using DoubleGis.Erm.Qds;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public interface IElasticMeta
    {
        string GetIndexName(Type type);
        string GetTypeName(Type type);

        PagedSearchDescriptor<TDoc> CreatePage<TDoc>(IDocsQuery query) where TDoc : class;
        PagedSearchDescriptor<TDoc> NextPage<TDoc>(PagedSearchDescriptor<TDoc> pagedSearchDescriptor, IQueryResponse<TDoc> response) where TDoc : class;
    }
}