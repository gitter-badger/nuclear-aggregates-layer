using System;

namespace DoubleGis.Erm.Platform.Core.EntityProjection
{
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