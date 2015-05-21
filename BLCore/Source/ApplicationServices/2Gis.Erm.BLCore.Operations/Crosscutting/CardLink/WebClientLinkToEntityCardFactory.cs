using System;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting.CardLink
{
    /// <summary>
    /// Специфичная для веб-клиента фабрика ссылок на карточки сущностей.
    /// Специфичность обусловлена генерируемыми ссылками, переход по ним повлечет открытие через web клиент ERM нужной карточки.
    /// </summary>
    public sealed class WebClientLinkToEntityCardFactory : ILinkToEntityCardFactory
    {
        private readonly IAPIWebClientServiceSettings _webClientServiceSettings;
        private const string CardUrlTemplate = "/CreateOrUpdate/{0}/{1}";

        public WebClientLinkToEntityCardFactory(IAPIWebClientServiceSettings webClientServiceSettings)
        {
            _webClientServiceSettings = webClientServiceSettings;
        }
        
        public Uri CreateLink<TEntity>(long entityId) where TEntity : class, IEntity
        {
            return new Uri(_webClientServiceSettings.Url, string.Format(CardUrlTemplate, typeof(TEntity).AsEntityName().Description, entityId));
        }
    }
}