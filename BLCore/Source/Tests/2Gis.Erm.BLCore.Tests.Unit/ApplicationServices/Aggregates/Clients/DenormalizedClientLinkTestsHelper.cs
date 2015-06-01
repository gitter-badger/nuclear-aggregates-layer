using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NUnit.Framework;

namespace DoubleGis.Erm.BLCore.Tests.Unit.ApplicationServices.Aggregates.Clients
{
    internal static class DenormalizedClientLinkTestsHelper
    {
        public static void ShouldBeEqualent(this IEnumerable<DenormalizedClientLink> firstCollection, IEnumerable<DenormalizedClientLink> secondCollection)
        {
            Assert.True(firstCollection.Count() == secondCollection.Count() && firstCollection.All(x => secondCollection.Any(y => AreEquals(x, y))));
        }

        public static IList<DenormalizedClientLink> AddDenormalizedLink(this IList<DenormalizedClientLink> links, long masterClientId, long childClientId, bool isLinkDirect)
        {
            return AddDenormalizedLink(links, masterClientId, childClientId, isLinkDirect, Guid.NewGuid());
        }

        public static IList<DenormalizedClientLink> AddDenormalizedLink(this IList<DenormalizedClientLink> links,
                                                                        long masterClientId,
                                                                        long childClientId,
                                                                        bool isLinkDirect,
                                                                        Guid graphKey)
        {
            links.Add(new DenormalizedClientLink
                {
                    Id = masterClientId * 10000 + childClientId * 10, // Id нужен, чтобы mock'и репозиториев с ума не сходили
                    MasterClientId = masterClientId,
                    ChildClientId = childClientId,
                    IsLinkedDirectly = isLinkDirect,
                    GraphKey = graphKey
                });

            return links;
        }

        public static IList<DenormalizedClientLink> GetGraphState(this IList<DenormalizedClientLink> links, long masterClientId, long childClientId)
        {
            var graphKeys =
                links.Where(x => x.MasterClientId == masterClientId || x.MasterClientId == childClientId || x.ChildClientId == masterClientId || x.ChildClientId == childClientId)
                     .Select(x => x.GraphKey)
                     .Distinct()
                     .ToArray();

            if (graphKeys.Count() > 2)
            {
                throw new InvalidDataException();
            }

            return links.Where(x => graphKeys.Contains(x.GraphKey)).ToList();
        }

        public static IEnumerable<DenormalizedLinksTestData> GetTestData(byte nodesAmount, bool dataToTestRemoval)
        {
            var listOfAdjacencyMatrix = new List<bool[,]>();
            var baseMatrix = new bool[nodesAmount, nodesAmount];
            GetAdjacencyMatrix(0, 0, nodesAmount, baseMatrix, listOfAdjacencyMatrix);

            var result = new List<DenormalizedLinksTestData>();

            foreach (var adjacencyMatrix in listOfAdjacencyMatrix)
            {
                for (int i = 0; i < nodesAmount; i++)
                {
                    for (int j = 0; j < nodesAmount; j++)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        if (adjacencyMatrix[i, j] == dataToTestRemoval)
                        {
                            var newAdjacencyMatrix = (bool[,])adjacencyMatrix.Clone();
                            newAdjacencyMatrix[i, j] = !dataToTestRemoval;

                            var reachabilityMatrix = GetReachabilityMatrix(nodesAmount, adjacencyMatrix);
                            var newReachabilityMatrix = GetReachabilityMatrix(nodesAmount, newAdjacencyMatrix);

                            var originalDirectLinks = TransformMatrixToDirectLinksList(nodesAmount, adjacencyMatrix);
                            var allOriginalLinks = TransformMatrixToLinksList(nodesAmount, reachabilityMatrix);

                            foreach (var link in allOriginalLinks.Intersect(originalDirectLinks, new DenormalizedClientLinkEqualityComparer()))
                            {
                                link.IsLinkedDirectly = true;
                            }

                            var expectedDirectLinks = TransformMatrixToDirectLinksList(nodesAmount, newAdjacencyMatrix);
                            var allExpectedLinks = TransformMatrixToLinksList(nodesAmount, newReachabilityMatrix);

                            foreach (var link in allExpectedLinks.Intersect(expectedDirectLinks, new DenormalizedClientLinkEqualityComparer()))
                            {
                                link.IsLinkedDirectly = true;
                            }


                            result.Add(new DenormalizedLinksTestData
                                {
                                    DenormalizedClientLinks = allOriginalLinks.ToList(),
                                    ExpectedLinks = allExpectedLinks.ToList(),
                                    MasterClientId = i,
                                    ChildClientId = j
                                });
                        }
                    }
                }
            }

            return result;
        }

        private static bool AreEquals(DenormalizedClientLink link1, DenormalizedClientLink link2)
        {
            if (link1.Equals(link2))
            {
                return true;
            }

            return link1.MasterClientId == link2.MasterClientId && link1.ChildClientId == link2.ChildClientId && link1.IsLinkedDirectly == link2.IsLinkedDirectly;
        }

        private static void GetAdjacencyMatrix(int rowIndex, int columnIndex, byte n, bool[,] currentMatrix, List<bool[,]> resultList)
        {
            var matrixWithTrueNode = (bool[,])currentMatrix.Clone();
            var matrixWithFalseNode = (bool[,])currentMatrix.Clone();

            matrixWithTrueNode[rowIndex, columnIndex] = true;
            matrixWithFalseNode[rowIndex, columnIndex] = false;

            if (rowIndex + 1 == n && columnIndex + 1 == n)
            {
                if (rowIndex != columnIndex)
                {
                    resultList.Add(matrixWithTrueNode);
                }

                resultList.Add(matrixWithFalseNode);
                return;
            }

            var j = columnIndex + 1;
            var i = rowIndex;
            if (j == n)
            {
                i += 1;
                j = 0;
            }

            if (rowIndex != columnIndex)
            {
                GetAdjacencyMatrix(i, j, n, matrixWithTrueNode, resultList);
            }

            GetAdjacencyMatrix(i, j, n, matrixWithFalseNode, resultList);
        }

        private static bool[,] GetReachabilityMatrix(int n, bool[,] adjacencyMatrix)
        {
            var r = (bool[,])adjacencyMatrix.Clone();
            var t = (bool[,])adjacencyMatrix.Clone();

            for (int i = 0; i < n; i++)
            {
                t = MatrixMultiplication(n, t, adjacencyMatrix);
                r = MatrixAddition(n, r, t);
            }

            for (int i = 0; i < n; i++)
            {
                r[i, i] = false;
            }

            return r;
        }

        private static bool[,] MatrixMultiplication(int n, bool[,] matrix1, bool[,] matrix2)
        {
            var r = new bool[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        r[i, j] |= matrix1[i, k] && matrix2[k, j];
                    }
                }
            }

            return r;
        }

        private static bool[,] MatrixAddition(int n, bool[,] matrix1, bool[,] matrix2)
        {
            var r = new bool[n, n];

            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    r[i, j] = matrix1[i, j] || matrix2[i, j];
                }
            }

            return r;
        }

        private static IEnumerable<DenormalizedClientLink> TransformMatrixToLinksList(int n, bool[,] reachabilityMatrix)
        {
            var result = new List<DenormalizedClientLink>();

            var proccessedNodes = new bool[n, n];

            for (int i = 0; i < n; i++)
            {
                var key = Guid.NewGuid();
                for (int j = 0; j < n; j++)
                {
                    if (!proccessedNodes[i, j] && reachabilityMatrix[i, j])
                    {
                        result.Add(
                            new DenormalizedClientLink
                                {
                                    MasterClientId = i,
                                    ChildClientId = j,
                                    Id = (i + 1) * 1000 + j + 1,
                                    GraphKey = key
                                });

                        proccessedNodes[i, j] = true;

                        SetKeys(n, key, j, proccessedNodes, reachabilityMatrix, result);
                    }

                    if (!proccessedNodes[j, i] && reachabilityMatrix[j, i])
                    {
                        result.Add(
                            new DenormalizedClientLink
                                {
                                    MasterClientId = j,
                                    ChildClientId = i,
                                    Id = (j + 1) * 1000 + i + 1,
                                    GraphKey = key
                                });

                        proccessedNodes[j, i] = true;

                        SetKeys(n, key, j, proccessedNodes, reachabilityMatrix, result);
                    }
                }
            }

            return result;
        }

        private static IEnumerable<DenormalizedClientLink> TransformMatrixToDirectLinksList(int n, bool[,] adjacencyMatrix)
        {
            var result = new List<DenormalizedClientLink>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (adjacencyMatrix[i, j])
                    {
                        result.Add(
                            new DenormalizedClientLink
                                {
                                    MasterClientId = i,
                                    ChildClientId = j,
                                    Id = (i + 1) * 1000 + j + 1,
                                    IsLinkedDirectly = true
                                });
                    }
                }
            }

            return result;
        }

        private static void SetKeys(int n, Guid key, int node, bool[,] proccessedNodes, bool[,] reachabilityMatrix, List<DenormalizedClientLink> results)
        {
            for (int k = 0; k < n; k++)
            {
                if (!proccessedNodes[node, k] && reachabilityMatrix[node, k])
                {
                    results.Add(new DenormalizedClientLink
                        {
                            MasterClientId = node,
                            ChildClientId = k,
                            Id = (node + 1) * 1000 + k + 1,
                            GraphKey = key
                        });

                    proccessedNodes[node, k] = true;

                    SetKeys(n, key, k, proccessedNodes, reachabilityMatrix, results);
                }

                if (!proccessedNodes[k, node] && reachabilityMatrix[k, node])
                {
                    results.Add(new DenormalizedClientLink
                        {
                            MasterClientId = k,
                            ChildClientId = node,
                            Id = (k + 1) * 1000 + node + 1,
                            GraphKey = key
                        });

                    proccessedNodes[k, node] = true;

                    SetKeys(n, key, k, proccessedNodes, reachabilityMatrix, results);
                }
            }
        }
    }
}