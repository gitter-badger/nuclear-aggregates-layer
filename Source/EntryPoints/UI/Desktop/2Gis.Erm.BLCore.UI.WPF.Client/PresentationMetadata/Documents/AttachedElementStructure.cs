using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class AttachedElementStructure : 
        ViewModelStructure<AttachedElementStructure, OrdinaryConfigElementIdentity, AttachedElementStructureBuilder>, 
        IAttachedElementStructure
    {
        private readonly OrdinaryConfigElementIdentity _identity = new OrdinaryConfigElementIdentity();
        private readonly Lazy<IHandlerFeature> _handler;

        public AttachedElementStructure(IEnumerable<IConfigFeature> features) 
            : base(features)
        {
            _handler = new Lazy<IHandlerFeature>(() => ElementFeatures.OfType<IHandlerFeature>().SingleOrDefault());
        }

        public IHandlerFeature Handler
        {
            get
            {
                return _handler.Value;
            }
        }

        public bool HasHandler
        {
            get
            {
                return _handler.Value != null;
            }
        }
        
        public ICondition Condition { get; set; }

        protected override OrdinaryConfigElementIdentity GetIdentity()
        {
            return _identity;
        }
    }
}