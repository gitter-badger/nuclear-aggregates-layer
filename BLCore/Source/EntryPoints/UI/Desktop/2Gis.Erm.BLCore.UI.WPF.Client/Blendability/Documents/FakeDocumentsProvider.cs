using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels.Contextual;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Blendability.Documents
{
    public static class FakeDocumentsProvider
    {
        public static IUnityContainer FillDocuments(this IUnityContainer container)
        {
            var documents = new IDocument[5];
            documents[0] = new ContextualDocumentViewModel(null);
            for (int i = 1; i < documents.Length; i++)
            {
                var doc = new NullDocumentViewModel(i + 1);
                var actions = new List<INavigationItem>();
                for (int j = 0; j < 5; j++)
                {
                    //actions.Add(new ToolbarAction(doc.DocumentName + "Action" + j));
                }

                //actions.Add(FakeToolbarActionsProvider.GetActionsForDocument(((IDocument)doc).DocumentName));
                doc.Actions = actions;
                documents[i] = doc;
            }

            var documentManager = container.Resolve<IDocumentManager>();
            documentManager.Add(documents);

            return container;
        }
    }
}
