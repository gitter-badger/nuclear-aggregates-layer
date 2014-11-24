using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Name;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements
{
    public sealed class UiElementMetadataBuilder : MetadataElementBuilder<UiElementMetadataBuilder, UiElementMetadata>
    {
        private readonly IdentifiableAspect<UiElementMetadataBuilder, UiElementMetadata> _id;
        private readonly NameFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> _name;
        private readonly TitleFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> _title;
        private readonly ImageFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> _icon;
        private readonly HandlerFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> _handler;
        private readonly OperationFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> _operation;

        public UiElementMetadataBuilder()
        {
            _id = new IdentifiableAspect<UiElementMetadataBuilder, UiElementMetadata>(this);
            _name = new NameFeatureAspect<UiElementMetadataBuilder, UiElementMetadata>(this);
            _title = new TitleFeatureAspect<UiElementMetadataBuilder, UiElementMetadata>(this);
            _icon = new ImageFeatureAspect<UiElementMetadataBuilder, UiElementMetadata>(this);
            _handler = new HandlerFeatureAspect<UiElementMetadataBuilder, UiElementMetadata>(this);
            _operation = new OperationFeatureAspect<UiElementMetadataBuilder, UiElementMetadata>(this);
        }

        public NameFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> Name
        {
            get { return _name; }
        }

        public TitleFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> Title
        {
            get { return _title; }
        }

        public ImageFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> Icon
        {
            get { return _icon; }
        }

        public HandlerFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> Handler
        {
            get { return _handler; }
        }

        public OperationFeatureAspect<UiElementMetadataBuilder, UiElementMetadata> Operation
        {
            get { return _operation; }
        }

        public IdentifiableAspect<UiElementMetadataBuilder, UiElementMetadata> Id
        {
            get { return _id; }
        }

        public UiElementMetadataBuilder AccessWithPrivelege(FunctionalPrivilegeName privilege)
        {
            AddFeatures(new SecuredByFunctionalPrivelegeFeature(privilege));
            return this;
        }

        public UiElementMetadataBuilder AccessWithPrivelege(EntityAccessTypes privilege, EntityName entity)
        {
            AddFeatures(new SecuredByEntityPrivelegeFeature(privilege, entity));
            return this;
        }

        public UiElementMetadataBuilder ControlType(ControlType type)
        {
            AddFeatures(new ControlTypeFeature(new EnumControlTypeDescriptor(type)));
            return this;
        }

        public UiElementMetadataBuilder LockOnNew()
        {
            AddFeatures(new LockOnNewFeature());
            return this;
        }

        public UiElementMetadataBuilder LockOnInactive()
        {
            AddFeatures(new LockOnInactiveFeature());
            return this;
        }

        protected override UiElementMetadata Create()
        {
            if (_id.HasValue)
            {
                return new UiElementMetadata(_id.Value, Features.ToArray());
            }

            var name = Features.OfType<NameFeature>().SingleOrDefault();
            var title = Features.OfType<TitleFeature>().SingleOrDefault();
            if (name == null && title == null)
            {
                return new UiElementMetadata(IdBuilder.StubUnique, Features.ToArray());
            }

            string relativePath = null;

            if (name != null)
            {
                relativePath = name.NameDescriptor.ResourceKeyToString(); 
            }
            else
            {
                relativePath = title.TitleDescriptor.ResourceKeyToString();

                if (string.IsNullOrEmpty(relativePath))
                {
                    throw new NotSupportedException(title.TitleDescriptor.GetType().Name + " title descriptor is not supported");
                }
            }

            return new UiElementMetadata(new Uri(relativePath, UriKind.Relative), Features.ToArray());
        }
    }
}
