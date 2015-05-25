using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Elements.Identities.Concrete;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationsMetadata : MetadataElement
    {
        private readonly Type _entityType;
        private readonly IMetadataElementIdentity _identity;

        public ViewModelCustomizationsMetadata(Type entityType, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _entityType = entityType;
            _identity = new MetadataElementIdentity(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ViewModelCustomizationsIdentity>(entityType.Name));
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public static ViewModelCustomizationsMetadataBuilder<TViewModel> For<TEntity, TViewModel>()
            where TEntity : IEntity
            where TViewModel : IEntityViewModelBase
        {
            return new ViewModelCustomizationsMetadataBuilder<TViewModel>(typeof(TEntity));
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotImplementedException();
        }
    }
}