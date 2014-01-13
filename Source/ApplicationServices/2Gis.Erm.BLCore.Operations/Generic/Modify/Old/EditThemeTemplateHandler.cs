using DoubleGis.Erm.BLCore.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditThemeTemplateHandler : RequestHandler<EditRequest<ThemeTemplate>, EmptyResponse>
    {
        private readonly IThemeRepository _themeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public EditThemeTemplateHandler(IThemeRepository themeRepository, IOperationScopeFactory scopeFactory)
        {
            _themeRepository = themeRepository;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(EditRequest<ThemeTemplate> request)
        {
            var template = request.Entity;

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(template))
            {
                var existingTemplate = _themeRepository.FindThemeTemplateByThemplateCode(template.TemplateCode);
                if (existingTemplate != null && existingTemplate.Id != template.Id)
                {
                    throw new NotificationException(BLResources.CannotCreateTemplateAlreadyExists);
                }

                var isNew = template.IsNew();

                _themeRepository.CreateOrUpdate(template);

                if (isNew)
                {
                    operationScope.Added<ThemeTemplate>(template.Id);
                }
                else
                {
                    operationScope.Updated<ThemeTemplate>(template.Id);
                }

                operationScope.Complete();
            }

            return Response.Empty;
        }
    }
}
