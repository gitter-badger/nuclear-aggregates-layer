using System;

namespace NuClear.Storage.Specifications
{
    public class MapSpecification<TInput, TOutput> : IMapSpecification<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput> _projector;

        public MapSpecification(Func<TInput, TOutput> projector)
        {
            _projector = projector;
        }

        public static implicit operator MapSpecification<TInput, TOutput>(Func<TInput, TOutput> projector)
        {
            return new MapSpecification<TInput, TOutput>(projector);
        }

        public static implicit operator Func<TInput, TOutput>(MapSpecification<TInput, TOutput> specification)
        {
            return specification._projector;
        }

        public TOutput Map(TInput input)
        {
            return _projector(input);
        }
    }
}