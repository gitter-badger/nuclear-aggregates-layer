using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationsMetada : MetadataElement<ViewModelCustomizationsMetada, ViewModelCustomizationsMetadataBuilder>
    {
        private readonly Type _entityType;
        private readonly IMetadataElementIdentity _identity;

        public ViewModelCustomizationsMetada(Type entityType, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _entityType = entityType;
            _identity = new MetadataElementIdentity(IdBuilder.For<ViewModelCustomizationsIdentity>(entityType.Name));
        }

        public Type EntityType
        {
            get { return _entityType; }
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotImplementedException();
        }
    }
}