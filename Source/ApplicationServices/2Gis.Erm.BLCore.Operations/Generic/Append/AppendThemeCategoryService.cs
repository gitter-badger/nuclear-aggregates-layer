using System;

using DoubleGis.Erm.BLCore.Aggregates.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendThemeCategoryService : IAppendGenericEntityService<Category, Theme>
    {
        private const int MaxCategoriesPerTheme = 5;
        private readonly IThemeRepository _themeRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public AppendThemeCategoryService(IThemeRepository themeRepository, IOperationScopeFactory scopeFactory)
        {
            _themeRepository = themeRepository;
            _scopeFactory = scopeFactory;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.UserIdOtTerritoryIdIsNotSpecified);
            }

            if (appendParams.ParentType != EntityName.Theme || appendParams.AppendedType != EntityName.Category)
            {
                throw new ArgumentException(BLResources.InvalidAppendThemeCategoryParamTypes);
            }

            var themeId = appendParams.ParentId.Value;
            var categoryId = appendParams.AppendedId.Value;

            var count = _themeRepository.CountThemeCategories(themeId);
            if (count >= MaxCategoriesPerTheme)
            {
                throw new ArgumentException(string.Format(BLResources.CannotAppendMoreCategories, MaxCategoriesPerTheme));
            }

            // FIXME {all, 05.08.2013}: реализация append, а декларируется операция createorupdate - исправить
            // DONE {i.maslennikov, 2013-08-07}: Да, было дело, вместо append двух сущностей логировался createorupdate одной, их связывающей. 
            //                                   Исправил в рамках разбиения на атомарные.
            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, Theme, Category>())
            {
                _themeRepository.AppendThemeToCategory(themeId, categoryId);
                scope.Updated<Theme>(themeId)
                     .Updated<Category>(categoryId);
                scope.Complete();
            }
        }
    }
}
