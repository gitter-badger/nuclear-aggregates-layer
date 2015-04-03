using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CategoryViewModel : EntityViewModelBase<Category>
    {
        [DisplayNameLocalized("CategoryName")]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [DisplayNameLocalized("CategoryLevel")]
        public int Level { get; set; }

        [DisplayNameLocalized("ParentCategoryName")]
        public LookupField ParentCategory { get; set; }

        public string Comment { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (CategoryDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Level = modelDto.Level;
            ParentCategory = LookupField.FromReference(modelDto.ParentRef);
            Comment = modelDto.Comment;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            if (IsNew)
            {
                throw new NotificationException(BLResources.CategoryCreationIsNotSupported);
            }

            return new CategoryDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Level = Level,
                    ParentRef = new EntityReference(ParentCategory.Key, ParentCategory.Value),
                    Comment = Comment,
                    Timestamp = Timestamp
                };
        }
    }
}