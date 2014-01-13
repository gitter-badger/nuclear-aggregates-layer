using DoubleGis.Erm.BLCore.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditThemeHandler : RequestHandler<EditRequest<Theme>, EmptyResponse>
    {
        private readonly IThemeRepository _themeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public EditThemeHandler(IThemeRepository themeRepository, IOperationScopeFactory scopeFactory)
        {
            _themeRepository = themeRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<Theme> request)
        {
            var theme = request.Entity;
            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(theme))
            {
                bool isNew = theme.IsNew();

                _themeRepository.CreateOrUpdate(theme);

                if (isNew)
                {
                    operationScope.Added<Theme>(theme.Id);
                }
                else
                {
                    operationScope.Updated<Theme>(theme.Id);
                }

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}