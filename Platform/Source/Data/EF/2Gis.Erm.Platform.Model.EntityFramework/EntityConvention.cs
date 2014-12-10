using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework
{
    public abstract class EntityConvention<TEntityContainer> : Convention, IEfDbModelConvention
                where TEntityContainer : IEntityContainer, new()
    {
        private static readonly Lazy<TEntityContainer> LazyContainer = new Lazy<TEntityContainer>(() => new TEntityContainer());
        private readonly Lazy<Convention> _lazyConvention;

        protected EntityConvention()
        {
            _lazyConvention = new Lazy<Convention>(CreateConvention);
        }

        protected abstract void Configure(Convention convention);

        void IEfDbModelConvention.Apply(DbModelBuilder builder)
        {
            builder.Conventions.Add(_lazyConvention.Value);
        }
        
        string IEfDbModelConvention.ContainerName
        {
            get { return LazyContainer.Value.Name; }
        }

        private Convention CreateConvention()
        {
            var convention = new Convention();
            Configure(convention);
            return convention;
        }
    }
}