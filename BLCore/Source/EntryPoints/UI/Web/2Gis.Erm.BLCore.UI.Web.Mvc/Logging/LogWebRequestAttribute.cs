using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Logging
{
    // TODO {all, 24.09.2013}: Нужно перенести логирование изменений с уровня entrypoint на уровень ApplicationServices
    [Obsolete("Нужно перенести логирование изменений с уровня entrypoint на уровень ApplicationServices")]
    public sealed class LogWebRequestAttribute : HandlerAttribute
    {
        private readonly IEntityType _entityType;
        private string _elementsToIgnore;
        private IEnumerable<string> _elementsToIgnoreCollection = Enumerable.Empty<string>();

        public LogWebRequestAttribute(string entityType)
        {
            EntityType.Instance.TryParse(entityType, out _entityType);
        }

        public string ElementsToIgnore
        {
            get
            {
                return _elementsToIgnore;
            }
            set
            {
                _elementsToIgnore = value;
                if (!string.IsNullOrEmpty(_elementsToIgnore))
                {
                    _elementsToIgnoreCollection = _elementsToIgnore.Split(',').Select(x => x.Trim()).ToArray();
                }
            }
        }

        public CompareObjectMode CompareObjectMode { get; set; }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogWebRequestHandler(
                container.Resolve<ITracer>(),
                container.Resolve<IActionLogger>(Mapping.SimplifiedModelConsumerScope),
                _entityType,
                CompareObjectMode,
                _elementsToIgnoreCollection,
                container.Resolve<IFinder>());
        }
    }
}