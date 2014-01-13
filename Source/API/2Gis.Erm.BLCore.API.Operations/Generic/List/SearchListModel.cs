using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    // TODO: renaming and refactoring?
    [DataContract]
    public sealed class SearchListModel
    {
        private IDictionary<string, string> _extendedInfoEntries;
        private IDictionary<string, string> _filterInputEntries;

        [DataMember]
        public string FilterInput { get; set; }

        [DataMember]
        public string WhereExp { get; set; }

        [DataMember]
        public string PType { get; set; }

        [DataMember]
        public long PId { get; set; }

        [DataMember]
        public string ExtendedInfo { get; set; }

        [DataMember]
        public string NameLocaleResourceId { get; set; }

        [DataMember]
        public int Start { get; set; }

        [DataMember]
        public int Limit { get; set; }

        [DataMember]
        public string Dir { get; set; }

        [DataMember]
        public string Sort { get; set; }

        public bool HasExtendedProperty(string name)
        {
            if (string.IsNullOrWhiteSpace(ExtendedInfo))
            {
                return false;
            }

            EvaluateEntriesMap(ref _extendedInfoEntries, ExtendedInfo);

            return _extendedInfoEntries.ContainsKey(name);
        }

        public T GetExtendedProperty<T>(string name)
        {
            return GetEntry<T>(ref _extendedInfoEntries, ExtendedInfo, name);
        }


        private static T GetEntry<T>(ref IDictionary<string, string> entriesContainer, string expressionContainer, string name)
        {
            if (string.IsNullOrWhiteSpace(expressionContainer))
            {
                return default(T);
            }

            EvaluateEntriesMap(ref entriesContainer, expressionContainer);

            string result;
            if (entriesContainer.TryGetValue(name, out result) && !string.IsNullOrWhiteSpace(result) && !string.Equals("null", result, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var t = typeof(T);
                    var u = Nullable.GetUnderlyingType(t);

                    return (T)Convert.ChangeType(result, u ?? t);
                }
                catch
                {
                    return default(T);
                }
            }

            return default(T);
        }

        private static void EvaluateEntriesMap(ref IDictionary<string, string> entriesContainer, string expressionContainer)
        {
            if (entriesContainer != null)
            {
                return;
            }

            var values = expressionContainer.Trim(new[] { '\'' }).Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries);
            entriesContainer = new Dictionary<string, string>();

            if (values.Length > 0)
            {
                foreach (var prms in values.Select(x => x.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)).Where(prms => prms.Length == 2))
                {
                    entriesContainer.Add(prms[0], prms[1]);
                }
            }
        }
    }
}