using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Handler;
using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Operations;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Images;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Name;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.UI.Metadata.UIElements
{
    public sealed class UIElementMetadataBuilder : MetadataElementBuilder<UIElementMetadataBuilder, UIElementMetadata>
    {
        private readonly IdentifiableAspect<UIElementMetadataBuilder, UIElementMetadata> _id;
        private readonly NameFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> _name;
        private readonly TitleFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> _title;
        private readonly ImageFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> _icon;
        private readonly HandlerFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> _handler;
        private readonly OperationFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> _operation;

        public UIElementMetadataBuilder()
        {
            _id = new IdentifiableAspect<UIElementMetadataBuilder, UIElementMetadata>(this);
            _name = new NameFeatureAspect<UIElementMetadataBuilder, UIElementMetadata>(this);
            _title = new TitleFeatureAspect<UIElementMetadataBuilder, UIElementMetadata>(this);
            _icon = new ImageFeatureAspect<UIElementMetadataBuilder, UIElementMetadata>(this);
            _handler = new HandlerFeatureAspect<UIElementMetadataBuilder, UIElementMetadata>(this);
            _operation = new OperationFeatureAspect<UIElementMetadataBuilder, UIElementMetadata>(this);
        }

        public NameFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> Name
        {
            get { return _name; }
        }

        public TitleFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> Title
        {
            get { return _title; }
        }

        public ImageFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> Icon
        {
            get { return _icon; }
        }

        public HandlerFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> Handler
        {
            get { return _handler; }
        }

        public OperationFeatureAspect<UIElementMetadataBuilder, UIElementMetadata> Operation
        {
            get { return _operation; }
        }

        public IdentifiableAspect<UIElementMetadataBuilder, UIElementMetadata> Id
        {
            get { return _id; }
        }

        public UIElementMetadataBuilder AccessWithPrivelege(FunctionalPrivilegeName privilege)
        {
            AddFeatures(new SecuredByFunctionalPrivelegeFeature(privilege));
            return this;
        }

        [Obsolete("Данный подход не удовлетворяет бизнес-логике валидации прав. Нужно использовать AccessWithEntityTypePrivilege если нужна проверка прав в рамках сущности и AccessWithEntityInstancePrivilege если нужна проверка прав в рамках экземпляра сущности.")]
        public UIElementMetadataBuilder AccessWithPrivelege<TEntity>(EntityAccessTypes privilege)
            where TEntity : IEntity
        {
            AddFeatures(new SecuredByEntityPrivelegeFeature(privilege, typeof(TEntity).AsEntityName()));
            return this;
        }

        public UIElementMetadataBuilder AccessWithEntityTypePrivilege<TEntity>(EntityAccessTypes privilege)
            where TEntity : IEntity
        {
            AddFeatures(new SecuredByEntityTypePrivilegeFeature(privilege, typeof(TEntity).AsEntityName()));
            return this;
        }
        public UIElementMetadataBuilder AccessWithEntityInstancePrivilege<TEntity>(EntityAccessTypes privilege)
            where TEntity : IEntity
        {
            AddFeatures(new SecuredByEntityInstancePrivilegeFeature(privilege, typeof(TEntity).AsEntityName()));
            return this;
        }

        public UIElementMetadataBuilder ControlType(ControlType type)
        {
            AddFeatures(new ControlTypeFeature(new EnumControlTypeDescriptor(type)));
            return this;
        }

        public UIElementMetadataBuilder LockOnNew()
        {
            AddFeatures(new LockOnNewCardFeature());
            return this;
        }

        public UIElementMetadataBuilder LockOnInactive()
        {
            AddFeatures(new LockOnInactiveCardFeature());
            return this;
        }

        public UIElementMetadataBuilder DisableOn<T>(params Expression<Func<T, bool>>[] expressions)
            where T : IAspect
        {
            AddFeatures(expressions.Select(expression => new DisableExpressionFeature<T>(expression)).ToArray());
            return this;
        }

        public UIElementMetadataBuilder DisableOn(LambdaExpressionsSequence expressionsSequence)
        {
            AddFeatures(new DisableExpressionsFeature(expressionsSequence.LogicalOperation, expressionsSequence.Expressions));
            return this;
        }

        public UIElementMetadataBuilder HideOn<T>(params Expression<Func<T, bool>>[] expressions)
            where T : IAspect
        {
            AddFeatures(expressions.Select(expression => new HideExpressionFeature<T>(expression)).ToArray());
            return this;
        }

        public UIElementMetadataBuilder HideOn(LambdaExpressionsSequence expressionsSequence)
        {
            AddFeatures(new HideExpressionsFeature(expressionsSequence.LogicalOperation, expressionsSequence.Expressions));
            return this;
        }

        protected override UIElementMetadata Create()
        {
            if (_id.HasValue)
            {
                return new UIElementMetadata(_id.Value, Features.ToArray());
            }

            var name = Features.OfType<NameFeature>().SingleOrDefault();
            var title = Features.OfType<TitleFeature>().SingleOrDefault();
            if (name == null && title == null)
            {
                return new UIElementMetadata(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.Stub().Unique(), Features.ToArray());
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

            return new UIElementMetadata(new Uri(relativePath, UriKind.Relative), Features.ToArray());
        }
    }
}
