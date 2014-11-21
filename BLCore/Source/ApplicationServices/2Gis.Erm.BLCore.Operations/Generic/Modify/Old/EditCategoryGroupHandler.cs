using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditCategoryGroupHandler : RequestHandler<EditRequest<CategoryGroup>, EmptyResponse>
    {
        private readonly ICategoryService _categoryService;

        public EditCategoryGroupHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        protected override EmptyResponse Handle(EditRequest<CategoryGroup> request)
        {
            _categoryService.CreateOrUpdate(request.Entity);
            return Response.Empty;
        }
    }
}