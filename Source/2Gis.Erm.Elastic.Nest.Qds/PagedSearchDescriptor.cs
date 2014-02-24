using System;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class PagedSearchDescriptor<TDoc> where TDoc : class
    {
        public PagedSearchDescriptor(SearchDescriptor<TDoc> searchDescriptor, int @from, int size)
        {
            if (searchDescriptor == null)
            {
                throw new ArgumentNullException("searchDescriptor");
            }

            Size = size;
            From = @from;
            SearchDescriptor = searchDescriptor;

            SearchDescriptor.From(From).Size(Size);
        }

        public SearchDescriptor<TDoc> SearchDescriptor { get; private set; }
        public int From { get; private set; }
        public int Size { get; private set; }
    }
}