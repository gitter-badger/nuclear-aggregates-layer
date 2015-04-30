using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.EntityFramework;

namespace DoubleGis.Erm.Platform.Model.EntityFramework
{
    public abstract class EntityConvention<TEntityContainer> : Convention, IEFDbModelConvention
                where TEntityContainer : IEntityContainer, new()
    {
        private static readonly Lazy<TEntityContainer> LazyContainer = new Lazy<TEntityContainer>(() => new TEntityContainer());
        private readonly Lazy<Convention> _lazyConvention;

        protected EntityConvention()
        {
            _lazyConvention = new Lazy<Convention>(CreateConvention);
        }

        protected abstract void Configure(Convention convention);

        void IEFDbModelConvention.Apply(DbModelBuilder builder)
        {
            builder.Conventions.Add(_lazyConvention.Value);
        }

        string IEFDbModelConvention.ContainerName
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