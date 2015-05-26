using System;

namespace NuClear.Storage.Specifications
{
    public class ProjectSpecification<TInput, TOutput> : IProjectSpecification<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> _projector;

        public ProjectSpecification(Func<TInput, TOutput> projector)
        {
            _projector = projector;
        }

        public static implicit operator ProjectSpecification<TInput, TOutput>(Func<TInput, TOutput> projector)
        {
            return new ProjectSpecification<TInput, TOutput>(projector);
        }

        public static implicit operator Func<TInput, TOutput>(ProjectSpecification<TInput, TOutput> specification)
        {
            return specification._projector;
        }

        public TOutput Project(TInput input)
        {
            return _projector(input);
        }
    }
}