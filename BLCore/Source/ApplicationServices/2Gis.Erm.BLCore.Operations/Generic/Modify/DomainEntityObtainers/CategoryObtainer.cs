using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class CategoryObtainer : ISimplifiedModelEntityObtainer<Category>
    {
        private readonly IFinder _finder;

        public CategoryObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Category ObtainSimplifiedModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (CategoryDomainEntityDto)domainEntityDto;

            var category = _finder.FindOne(Specs.Find.ById<Category>(dto.Id)) 
                ?? new Category { IsActive = true };

            category.Id = dto.Id;
            category.Name = dto.Name;
            category.Level = dto.Level;
            category.ParentId = dto.ParentRef.Id;
            category.Comment = dto.Comment;
            category.Timestamp = dto.Timestamp;

            return category;
        }
    }
}