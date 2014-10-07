using System;

namespace DoubleGis.Erm.Platform.Core.EntityProjection
{
    // FIXME {d.ivanov, 15.09.2014}: Move to 2Gis.Erm.Platform.Common
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