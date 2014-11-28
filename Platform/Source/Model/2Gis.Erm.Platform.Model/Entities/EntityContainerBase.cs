using System;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public abstract class EntityContainerBase<TEntityContainer> : IEntityContainer
        where TEntityContainer : IEntityContainer, new()
    {
        private static readonly Lazy<TEntityContainer> LazyInstance = new Lazy<TEntityContainer>(() => new TEntityContainer()); 
        public static TEntityContainer Instance
        {
            get { return LazyInstance.Value; }
        }

        public abstract string Name { get; }
    }
}