﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.PrintFormEngine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Russia;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms
{
    public sealed class PrintLetterOfGuaranteeHandler : RequestHandler<PrintLetterOfGuaranteeRequest, Response>, IRussiaAdapted, IKazakhstanAdapted
    {
        private readonly IFinder _finder;
        private readonly IFormatter _longDateFormatter;
        private readonly ISubRequestProcessor _requestProcessor;

        public PrintLetterOfGuaranteeHandler(ISubRequestProcessor requestProcessor, IFormatterFactory formatterFactory, IFinder finder)
        {
            _requestProcessor = requestProcessor;
            _finder = finder;
            _longDateFormatter = formatterFactory.Create(typeof(DateTime), FormatType.LongDate, 0);
        }

        protected override Response Handle(PrintLetterOfGuaranteeRequest request)
        {
            var data =
                _finder.Find(Specs.Find.ById<Order>(request.OrderId))
                       .Select(
                           order =>
                           new
                               {
                                   Order = order,
                                   ProfileId = order.LegalPersonProfileId,
                                   order.LegalPersonId, 
                                   order.BranchOfficeOrganizationUnitId, 
                               })
                       .Single();

            if (data.ProfileId == null)
            {
                throw new LegalPersonProfileMustBeSpecifiedException();
            }

            var profile = _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(data.ProfileId.Value));
            var printData = new
                {
                    data.Order,
                    SignupDate = _longDateFormatter.Format(data.Order.SignupDate),
                    ChangeDate = _longDateFormatter.Format(DateTime.Now.GetNextMonthFirstDate()),
                    Profile = new
                                  {
                                      ChiefFullNameInNominative = profile.Within<RussiaLegalPersonProfilePart>().GetPropertyValue(part => part.ChiefFullNameInNominative),
                                      ChiefNameInNominative = profile.ChiefNameInNominative,
                                      PositionInNominative = profile.PositionInNominative
                                  },
                    LegalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(data.LegalPersonId.Value)),
                    BranchOfficeOrganizationUnit = data.BranchOfficeOrganizationUnitId.HasValue
                                                       ? _finder.FindOne(Specs.Find.ById<BranchOfficeOrganizationUnit>(data.BranchOfficeOrganizationUnitId.Value))
                                                       : null,
                    data.BranchOfficeOrganizationUnitId,
                };

            return
                _requestProcessor.HandleSubRequest(
                    new PrintDocumentRequest
                        {
                            TemplateCode = request.IsChangingAdvMaterial ? TemplateCode.LetterOfGuaranteeAdvMaterial : TemplateCode.LetterOfGuarantee,
                            FileName = string.Format("{0}-Гарантийное письмо", printData.Order.Number),
                            BranchOfficeOrganizationUnitId = printData.BranchOfficeOrganizationUnitId, 
                            PrintData = printData
                        }, 
                    Context);
        }
    }
}
