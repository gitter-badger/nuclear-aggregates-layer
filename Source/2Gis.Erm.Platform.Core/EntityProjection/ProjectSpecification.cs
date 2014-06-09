using System;

namespace DoubleGis.Erm.Platform.Core.EntityProjection
{
    public class ProjectSpecification<TInput, TOutput> : IProjectSpecification<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> _projector;

        public ProjectSpecification(Func<TInput, TOutput> projector)
        {
            _projector = projector;
        }

        public TOutput Project(TInput input)
        {
            return _projector(input);
        }
    }
}