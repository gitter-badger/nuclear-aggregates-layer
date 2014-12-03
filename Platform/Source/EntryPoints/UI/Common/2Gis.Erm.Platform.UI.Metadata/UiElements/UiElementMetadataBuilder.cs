﻿using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Name;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
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

        public UiElementMetadataBuilder AccessWithPrivelege<TEntity>(EntityAccessTypes privilege)
            where TEntity : IEntity
        {
            AddFeatures(new SecuredByEntityPrivelegeFeature(privilege, typeof(TEntity).AsEntityName()));
            return this;
        }

        public UiElementMetadataBuilder ControlType(ControlType type)
        {
            AddFeatures(new ControlTypeFeature(new EnumControlTypeDescriptor(type)));
            return this;
        }

        public UiElementMetadataBuilder LockOnNew()
        {
            AddFeatures(new LockOnNewCardFeature());
            return this;
        }

        public UiElementMetadataBuilder LockOnInactive()
        {
            AddFeatures(new LockOnInactiveCardFeature());
            return this;
        }

        public UiElementMetadataBuilder DisableOn<T>(Expression<Func<T, bool>> expression)
            where T : IViewModelAbstract
        {
            AddFeatures(new DisableExpressionFeature<T>(expression));
            return this;
        }

        public UiElementMetadataBuilder HideOn<T>(Expression<Func<T, bool>> expression)
           where T : IViewModelAbstract
        {
            AddFeatures(new HideExpressionFeature<T>(expression));
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
