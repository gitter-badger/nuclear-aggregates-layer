using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Mode;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy
{
    public sealed class HierarchyMetadataBuilder : MetadataElementBuilder<HierarchyMetadataBuilder, HierarchyMetadata>
    {
        private readonly IdentifiableAspect<HierarchyMetadataBuilder, HierarchyMetadata> _id;
        private readonly TitleFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> _title;
        private readonly ImageFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> _icon;
        private readonly HandlerFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> _handler;
        private readonly OperationFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> _operation;
        private readonly SharedFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> _mode;

        public HierarchyMetadataBuilder()
        {
            _id = new IdentifiableAspect<HierarchyMetadataBuilder, HierarchyMetadata>(this);
            _title = new TitleFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata>(this);
            _icon = new ImageFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata>(this);
            _handler = new HandlerFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata>(this);
            _operation = new OperationFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata>(this);
            _mode = new SharedFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata>(this);
        }

        public TitleFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> Title
        {
            get { return _title; }
        }

        public ImageFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> Icon
        {
            get { return _icon; }
        }

        public HandlerFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> Handler
        {
            get { return _handler; }
        }

        public OperationFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> Operation
        {
            get { return _operation; }
        }

        public SharedFeatureAspect<HierarchyMetadataBuilder, HierarchyMetadata> Mode
        {
            get { return _mode; }
        }

        public IdentifiableAspect<HierarchyMetadataBuilder, HierarchyMetadata> Id
        {
            get { return _id; }
        }

        protected override HierarchyMetadata Create()
        {
            if (_id.HasValue)
            {
                return new HierarchyMetadata(_id.Value, Features.ToArray());
            }

            var title = Features.OfType<TitleFeature>().SingleOrDefault();
            if (title == null)
            {
                return new HierarchyMetadata(IdBuilder.StubUnique, Features.ToArray());
            }

            string relativePath = null;
            var resourceDescriptor = title.TitleDescriptor as ResourceTitleDescriptor;
            if (resourceDescriptor != null)
            {
                relativePath = resourceDescriptor.ResourceEntryKey.ResourceHostType.Name + "." + resourceDescriptor.ResourceEntryKey.ResourceEntryName;
            }
            else
            {
                var staticDescriptor = title.TitleDescriptor as StaticTitleDescriptor;
                if (staticDescriptor != null)
                {
                    relativePath = staticDescriptor.Title;
                }
            }

            if (string.IsNullOrEmpty(relativePath))
            {
                throw new NotSupportedException(title.TitleDescriptor.GetType().Name + " title descriptor is not supported");
            }

            return new HierarchyMetadata(new Uri(relativePath, UriKind.Relative), Features.ToArray());
        }
    }
}
