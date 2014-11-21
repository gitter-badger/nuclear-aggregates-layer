using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.SourceSamples
{
    public static class UseCaseList
    {
        public static readonly IEnumerable<UseCase> UseCases = GetUseCases();

        private static IEnumerable<UseCase> GetSampleUseCases()
        {
            return new[]
            {
                new UseCase
                {
                    Description = "Description",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(RequestHandler<,>),
                        Request = typeof(Request),
                        ChildNodes = new[]
                        {
                                new UseCaseNode(0)
                                {
                                    ContainingClass = typeof(RequestHandler<,>),
                                    Request = typeof(Request)
                                },
                                new UseCaseNode(0)
                                {
                                    ContainingClass = typeof(RequestHandler<,>),
                                    Request = typeof(Request)
                                }
                        }
                    }
                },
                new UseCase
                {
                    Description = "Description",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(RequestHandler<,>),
                        Request = typeof(Request),

                    }
                },
            };
        }

        private static IEnumerable<UseCase> GetUseCases()
        {
            return new UseCase[] { };
        }
    }
}
