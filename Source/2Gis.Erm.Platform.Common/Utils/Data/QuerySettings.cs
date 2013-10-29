using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Common.Utils.Data
{
    public sealed class QuerySettings
    {
        public int SkipCount { get; set; }
        public int TakeCount { get; set; }
        public string SortOrder { get; set; }
        public string SortDirection { get; set; }
        public string FilterPredicate { get; set; }
        public IEnumerable<QueryField> Fields { get; set; }
        public string DefaultFilter { get; set; }
        public string UserInputFilter { get; set; }
        public string MainAttribute { get; set; }
    }
}