using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents.Settings;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Titles;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class DocumentStructuresProvider : IDocumentStructuresProvider
    {
        private readonly ICardStructuresProvider _cardStructuresProvider;
        private readonly DocumentStructure[] _documentsSettings;

        public DocumentStructuresProvider(ICardStructuresProvider cardStructuresProvider)
        {
            _cardStructuresProvider = cardStructuresProvider;
            _documentsSettings = DocumentStructures.Settings;
            EvaluateReferences(_documentsSettings);
            EvaluateTitles(_documentsSettings);
        }

        public DocumentStructure[] Structures
        {
            get
            {
                return _documentsSettings;
            }
        }
        
        private void EvaluateReferences(IEnumerable<DocumentStructure> documentStructures)
        {
            foreach (var documentStructure in documentStructures)
            {
                if (documentStructure.Elements == null)
                {
                    continue;
                }

                foreach (var referenceElement in documentStructure.Elements.OfType<IReferencedElementStructure>())
                {
                    var referenceElementEvaluator = referenceElement as IReferenceEvaluator;
                    if (referenceElementEvaluator == null)
                    {
                        throw new InvalidOperationException("Document element of reference type doesn't support reference evaluation");
                    }

                    var cardStructureIdentity = referenceElement.ReferencedElementIdentity as ICardStructureIdentity;
                    if (cardStructureIdentity == null)
                    {
                        throw new InvalidOperationException("Not supported reference target was specified");
                    }

                    CardStructure cardStructure;
                    if (!_cardStructuresProvider.TryGetDescriptor(cardStructureIdentity.EntityName, out cardStructure))
                    {
                        throw new InvalidOperationException("Can't find card structure referenced in document. EntityName: " + cardStructureIdentity.EntityName);
                    }

                    referenceElementEvaluator.TryEvaluate(cardStructure);
                }
            }
        }

        private void EvaluateTitles(IEnumerable<DocumentStructure> documentStructures)
        {
            foreach (var documentStructure in documentStructures.Where(s => !s.ElementFeatures.Any(f => f is TitleFeature)))
            {
                if (documentStructure.Elements == null)
                {
                    continue;
                }

                foreach (var documentElement in documentStructure.Elements)
                {
                    var referenceElement = documentElement as IReferencedElementStructure;
                    if (referenceElement != null && referenceElement.ReferencedElement != null)
                    {
                        var titleFeature = referenceElement.ReferencedElement.ElementFeatures.OfType<TitleFeature>().SingleOrDefault();
                        if (titleFeature == null)
                        {
                            continue;
                        }

                        var updater = (IConfigElementUpdater)documentStructure;
                        updater.AddFeature(titleFeature);
                        break;
                    }

                    var attachedElement = documentElement as IAttachedElementStructure;
                    if (attachedElement != null)
                    {
                        var titleFeature = attachedElement.ElementFeatures.OfType<TitleFeature>().SingleOrDefault();
                        if (titleFeature == null)
                        {
                            continue;
                        }

                        var updater = (IConfigElementUpdater)documentStructure;
                        updater.AddFeature(titleFeature);
                    }
                }
            }
        }
    }
}