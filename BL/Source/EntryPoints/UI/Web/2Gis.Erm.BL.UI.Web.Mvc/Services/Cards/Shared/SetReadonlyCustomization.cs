using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Metadata.Config;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features.Expressions;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class SetReadonlyCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IMetadataProvider _metadataProvider;

        public SetReadonlyCustomization(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CardMetadata metadata;
            var metadataId = IdBuilder.For<MetadataCardsIdentity>(viewModel.ViewConfig.EntityName.ToString()).AsIdentity();
            if (!_metadataProvider.TryGetMetadata(metadataId.Id, out metadata))
            {
                throw new MetadataNotFoundException(metadataId);
            }

            foreach (var feature in metadata.Features<IDisableExpressionFeature>())
            {
                bool expressionResult;

                if (!feature.Expression.TryExecuteAspectLambda((IAspect)viewModel, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute disable expression for {0} card with {1} viewmodel", metadata.Entity, viewModel.GetType()));
                }

                viewModel.ViewConfig.ReadOnly |= expressionResult;
            }

            foreach (var feature in metadata.Features<DisableExpressionsFeature>())
            {
                bool expressionResult;

                if (!feature.Expressions.TryExecuteAspectBoolLambdas((IAspect)viewModel, feature.LogicalOperation, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute disable expressions for {0} card with {1} viewmodel", metadata.Entity, viewModel.GetType()));
                }

                viewModel.ViewConfig.ReadOnly |= expressionResult;
            }

            viewModel.ViewConfig.ReadOnly |= metadata.Uses<ReadOnlyFeature>();
        }
    }
}