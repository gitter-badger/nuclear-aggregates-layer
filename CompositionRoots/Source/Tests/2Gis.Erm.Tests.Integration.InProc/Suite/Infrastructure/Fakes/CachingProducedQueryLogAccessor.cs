using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using NuClear.Storage;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes
{
    public class CachingProducedQueryLogAccessor : IProducedQueryLogAccessor, IProducedQueryLogContainer
    {
        private const string QueryParamToken = "p__linq__";
        private const string QueryCommentToken = "--";
        private const string QueryMessageToken = "connection";

        private readonly Regex _regex = new Regex(string.Format("({0}\\d+):\\s('.+')", QueryParamToken));
        private readonly List<string> _messages = new List<string>();
        private readonly Dictionary<Guid, List<string>> _params = new Dictionary<Guid, List<string>>();
        private readonly Dictionary<Guid, string> _queries = new Dictionary<Guid, string>();

        private Guid _currentQueryId;

        public CachingProducedQueryLogAccessor()
        {
            Log = query =>
                {
                    if (query.Equals(Environment.NewLine))
                    {
                        return;
                    }

                    if (query.Contains(QueryMessageToken))
                    {
                        _messages.Add(query);
                        return;
                    }

                    if (query.StartsWith(QueryCommentToken))
                    {
                        if (query.Contains(QueryParamToken))
                        {
                            var param = query.Replace(QueryCommentToken, string.Empty);

                            List<string> @params;
                            if (!_params.TryGetValue(_currentQueryId, out @params))
                            {
                                _params.Add(_currentQueryId, new List<string>(new[] { param }));
                            }
                            else
                            {
                                @params.Add(param);
                            }
                        }

                        _messages.Add(query);
                        return;
                    }

                    _currentQueryId = Guid.NewGuid();
                    _queries.Add(_currentQueryId, query);
                };
        }

        public Action<string> Log
        {
            get; private set; 
        }

        public IEnumerable<string> Queries
        {
            get
            {
                var queries = new List<string>();
                foreach (var query in _queries)
                {
                    var @params = _params[query.Key];

                    var result = query.Value;
                    foreach (var param in @params)
                    {
                        var values = _regex.Match(param).Groups
                                           .Cast<Group>()
                                           .Select(x => x.Value)
                                           .ToArray();
                        result = result.Replace("@" + values[1], values[2]);
                    }

                    queries.Add(result);
                }

                return queries;
            }
        }

        public void Reset()
        {
            _messages.Clear();
            _params.Clear();
            _queries.Clear();
        }
    }
}