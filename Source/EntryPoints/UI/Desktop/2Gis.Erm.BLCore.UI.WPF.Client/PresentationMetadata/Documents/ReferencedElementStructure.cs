using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class ReferencedElementStructure :
        ViewModelStructure<ReferencedElementStructure, OrdinaryConfigElementIdentity, ReferencedElementStructureBuilder>,
        IReferencedElementStructure, 
        IReferenceEvaluator
    {
        private bool _isEvaluated;
        private IConfigElement _referencedConfigElement;
        private readonly IConfigElementIdentity _referencedElementIdentity;
        private readonly OrdinaryConfigElementIdentity _identity = new OrdinaryConfigElementIdentity();

        public ReferencedElementStructure(IConfigElementIdentity referencedElementIdentity, IEnumerable<IConfigFeature> features)
            : base(features)
        {
            _referencedElementIdentity = referencedElementIdentity;
        }

        public IConfigElementIdentity ReferencedElementIdentity
        {
            get
            {
                return _referencedElementIdentity;
            }
        }

        public bool IsReferenceEvaluated 
        {
            get
            {
                return _isEvaluated;
            }
        }

        public IConfigElement ReferencedElement 
        {
            get
            {
                return _referencedConfigElement;
            }
        }

        public ICondition Condition { get; set; }

        bool IReferenceEvaluator.TryEvaluate(IConfigElement referencedConfigElement)
        {
            if (_isEvaluated)
            {
                return false;
            }

            _isEvaluated = true;
            _referencedConfigElement = referencedConfigElement;
            return true;
        }

        protected override OrdinaryConfigElementIdentity GetIdentity()
        {
            return _identity;
        }
    }
}