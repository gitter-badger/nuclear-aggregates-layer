using System;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class PagedSearchDescriptor<TDoc> where TDoc : class
    {
        public PagedSearchDescriptor(Func<SearchDescriptor<TDoc>, SearchDescriptor<TDoc>> searchDescriptor, int @from, int size)
        {
            if (searchDescriptor == null)
            {
                throw new ArgumentNullException("searchDescriptor");
            }

            SearchDescriptor = x => searchDescriptor(x).From(From).Size(Size);
            Size = size;
            From = @from;
        }

        public Func<SearchDescriptor<TDoc>, SearchDescriptor<TDoc>> SearchDescriptor { get; private set; }
        public int From { get; private set; }
        public int Size { get; private set; }
    }
}