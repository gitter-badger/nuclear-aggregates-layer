using System;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Stickers;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowStickers.Deserializers
{
    public sealed class DeserializeHotClientService : IDeserializeServiceBusObjectService<HotClientServiceBusDto>
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            var hotClientRequest = new HotClientServiceBusDto();

            // SourceCode
            var sourceCodeAttr = xml.Attribute("SourceCode");
            if (sourceCodeAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "SourceCode"));
            }

            hotClientRequest.SourceCode = sourceCodeAttr.Value;

            // UserId
            var userIdAttr = xml.Attribute("UserCode");
            if (userIdAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "UserCode"));
            }

            hotClientRequest.UserCode = userIdAttr.Value;

            // UserName
            var userNameAttr = xml.Attribute("UserName");
            if (userNameAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "UserName"));
            }

            hotClientRequest.UserName = userNameAttr.Value;

            // CreationDate
            var dateAttr = xml.Attribute("CreationDate");
            if (dateAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "CreationDate"));
            }

            hotClientRequest.CreationDate = DateTime.Parse(dateAttr.Value);

            // ContactName
            var contactNameAttr = xml.Attribute("ContactName");
            if (contactNameAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "ContactName"));
            }

            hotClientRequest.ContactName = contactNameAttr.Value;

            // ContactPhone
            var contactPhoneAttr = xml.Attribute("ContactPhone");
            if (contactPhoneAttr == null)
            {
                throw new BusinessLogicException(string.Format(BLResources.ImportHotClientRequiredAttributeNotFound, "ContactPhone"));
            }

            hotClientRequest.ContactPhone = contactPhoneAttr.Value;

            // Description
            var descriptionAttr = xml.Attribute("Description");
            if (descriptionAttr != null)
            {
                hotClientRequest.Description = descriptionAttr.Value;
            }

            // CardCode
            var cardCodeElement = xml.Element("CardCode");
            if (cardCodeElement != null)
            {
                hotClientRequest.CardCode = (long)cardCodeElement.Attribute("CardCode");
            }

            // BranchCode
            var branchCodeElement = xml.Element("BranchCode");
            if (branchCodeElement != null)
            {
                hotClientRequest.BranchCode = (long)branchCodeElement.Attribute("BranchCode");
            }

            if (branchCodeElement == null && cardCodeElement == null)
            {
                throw new BusinessLogicException(BLResources.ImportHotClientCardCodeAndBranchCodeAreNull);
            }

            return hotClientRequest;
        }

        public bool Validate(XElement xml, out string error)
        {
            error = null;
            return true;
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}