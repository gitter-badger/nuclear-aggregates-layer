using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Georgaphy;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BL.Operations.Concrete.Integration.Import
{
    public sealed class DeserializeSaleTerritoryService : IDeserializeServiceBusObjectService<SaleTerritoryServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var territoryDto = new SaleTerritoryServiceBusDto();

            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Code");
            }

            territoryDto.Code = (long)codeAttr;

            // IsDeleted
            var isDeletedAttr = xml.Attribute("IsDeleted");
            if (isDeletedAttr != null)
            {
                territoryDto.IsDeleted = (bool)isDeletedAttr;

                // При выставленном IsDeleted = true, остальные поля не приходят
                if (territoryDto.IsDeleted)
                {
                    return territoryDto;
                }
            }

            // Name
            var nameAttr = xml.Attribute("Name");
            if (nameAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут Name");
            }

            territoryDto.Name = nameAttr.Value;

            // BranchCode
            var branchCodeAttr = xml.Attribute("BranchCode");
            if (branchCodeAttr == null)
            {
                throw new BusinessLogicException("Не найден обязательный атрибут BranchCode");
            }

            territoryDto.OrganizationUnitDgppId = (int)branchCodeAttr;

            return territoryDto;
        }

        public bool Validate(XElement xml, out string error)
        {
            var errors = new List<string>();

            // Code
            var codeAttr = xml.Attribute("Code");
            if (codeAttr == null)
            {
                errors.Add("Не найден обязательный атрибут Code");
            }

            var isDeletedAttr = xml.Attribute("IsDeleted");
            if (isDeletedAttr == null || !(bool)isDeletedAttr)
            {
                // Name
                var nameAttr = xml.Attribute("Name");
                if (nameAttr == null)
                {
                    errors.Add("Не найден обязательный атрибут Name");
                }

                var branchCodeAttr = xml.Attribute("BranchCode");
                if (branchCodeAttr == null)
                {
                    errors.Add("Не найден обязательный атрибут BranchCode");
                }
            }

            error = string.Join("; ", errors);

            return !errors.Any();
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}