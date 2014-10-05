using System;
using System.Collections.Generic;
using System.Linq;
using GraphId = System.Object;

namespace DoubleGis.Erm.BLCore.Aggregates.Clients.Operations
{
    public static class GraphDenormalizationExtensions
    {
        public static HashSet<Tuple<long, long>> Denormalize(this IEnumerable<Tuple<long, long>> graph)
        {
            // Далее используется вариация алгоритма Уолшера для построения матрицы достижимости:
            // for k = 1 to n
            //    for i = 1 to n
            //       for j = 1 to n
            //          W[i][j] = W[i][j] or (W[i][k] and W[k][j])
            var nodes = graph.Select(link => link.Item1).Concat(graph.Select(link => link.Item2)).Distinct().ToArray();
            var denormalization = new HashSet<Tuple<long, long>>(graph);

            foreach (var k in nodes)
            {
                foreach (var i in nodes)
                {
                    foreach (var j in nodes)
                    {
                        if (i == j)
                        {
                            // Циклические ссылки приводят к тому, что узел становится доступен сам от себя - этого избегаем
                            continue;
                        }

                        var key1 = Tuple.Create(i, j);
                        var key2 = Tuple.Create(i, k);
                        var key3 = Tuple.Create(k, j);

                        if (!denormalization.Contains(key1) && (denormalization.Contains(key2) && denormalization.Contains(key3)))
                        {
                            denormalization.Add(key1);
                        }
                    }
                }
            }

            return denormalization;
        }

        public static IEnumerable<IEnumerable<long>> SplitGraphs(this IEnumerable<Tuple<long, long>> graph)
        {
            var ids = new Dictionary<long, GraphId>();
            foreach (var link in graph)
            {
                MarkSameGraphNodes(ids, link.Item1, link.Item2);
            }

            return ids.GroupBy(pair => pair.Value, pair => pair.Key);
        }

        private static void MarkSameGraphNodes(Dictionary<long, GraphId> ids, long node1, long node2)
        {
            var graphForNode1 = GetGraphIdForNode(ids, node1);
            var graphForNode2 = GetGraphIdForNode(ids, node2);

            if (graphForNode1 == null && graphForNode2 == null)
            {
                var newGraph = new GraphId();
                ids.Add(node1, newGraph);
                ids.Add(node2, newGraph);
            }
            else if (graphForNode1 == null)
            {
                ids.Add(node1, graphForNode2);

            }
            else if (graphForNode2 == null)
            {
                ids.Add(node2, graphForNode1);
            }
            else
            {
                var movingNodes = ids.Where(pair => pair.Value == graphForNode2).Select(pair => pair.Key).ToArray();
                foreach (var node in movingNodes)
                {
                    ids[node] = graphForNode1;
                }
            }
        }

        private static GraphId GetGraphIdForNode(IDictionary<long, GraphId> ids, long node)
        {
            GraphId result;
            ids.TryGetValue(node, out result);
            return result;
        }
    }
}