using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Images;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Mode;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy
{
    public sealed class HierarchyElementsBuilder : ConfigElementBuilder<HierarchyElementsBuilder, HierarchyElement>
    {
        private readonly TitleFeatureAspect<HierarchyElementsBuilder, HierarchyElement> _title;
        private readonly ImageFeatureAspect<HierarchyElementsBuilder, HierarchyElement> _icon;
        private readonly HandlerFeatureAspect<HierarchyElementsBuilder, HierarchyElement> _handler;
        private readonly OperationBoundFeatureAspect<HierarchyElementsBuilder, HierarchyElement> _operation;
        private readonly SharedFeatureAspect<HierarchyElementsBuilder, HierarchyElement> _mode;

        public HierarchyElementsBuilder()
        {
            _title = new TitleFeatureAspect<HierarchyElementsBuilder, HierarchyElement>(this);
            _icon = new ImageFeatureAspect<HierarchyElementsBuilder, HierarchyElement>(this);
            _handler = new HandlerFeatureAspect<HierarchyElementsBuilder, HierarchyElement>(this);
            _operation = new OperationBoundFeatureAspect<HierarchyElementsBuilder, HierarchyElement>(this);
            _mode = new SharedFeatureAspect<HierarchyElementsBuilder, HierarchyElement>(this);
        }

        public TitleFeatureAspect<HierarchyElementsBuilder, HierarchyElement> Title
        {
            get { return _title; }
        }

        public ImageFeatureAspect<HierarchyElementsBuilder, HierarchyElement> Icon
        {
            get { return _icon; }
        }

        public HandlerFeatureAspect<HierarchyElementsBuilder, HierarchyElement> Handler
        {
            get { return _handler; }
        }

        public OperationBoundFeatureAspect<HierarchyElementsBuilder, HierarchyElement> Operation
        {
            get { return _operation; }
        }

        public SharedFeatureAspect<HierarchyElementsBuilder, HierarchyElement> Mode
        {
            get { return _mode; }
        }

        protected override HierarchyElement Create()
        {
            return new HierarchyElement(Features.ToArray());
        }
    }
}
