using System;

namespace DoubleGis.Erm.Platform.Core.EntityProjection
{
    // FIXME {d.ivanov, 15.09.2014}: Move to 2Gis.Erm.Platform.Common
    public class AssignSpecification<TSource, TTarget> : IAssignSpecification<TSource, TTarget>
    {
        private readonly Action<TSource, TTarget> _assigner;

        public AssignSpecification(Action<TSource, TTarget> assigner)
        {
            _assigner = assigner;
        }

        public void Assign(TSource source, TTarget target)
        {
            _assigner(source, target);
        }
    }
}