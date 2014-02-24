using DoubleGis.Erm.Qds;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public interface IElasticMeta
    {
        string GetIndexName(string typeName);
        PagedSearchDescriptor<TDoc> CreatePage<TDoc>(IDocsQuery query) where TDoc : class;
        PagedSearchDescriptor<TDoc> NextPage<TDoc>(PagedSearchDescriptor<TDoc> pagedSearchDescriptor, IQueryResponse<TDoc> response) where TDoc : class;
    }
}