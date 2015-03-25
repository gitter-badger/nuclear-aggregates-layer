using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.LegalPersons
{
    public sealed class NullValidateLegalPersonsForExportOperationService : IValidateLegalPersonsForExportOperationService,
                                                                            ICyprusAdapted,
                                                                            ICzechAdapted,
                                                                            IChileAdapted,
                                                                            IUkraineAdapted,
                                                                            IEmiratesAdapted,
                                                                            IKazakhstanAdapted
    {
        public IEnumerable<LegalPersonValidationForExportErrorDto> Validate(IEnumerable<ValidateLegalPersonDto> legalPersonsToValidate)
        {
            return Enumerable.Empty<LegalPersonValidationForExportErrorDto>();
        }
    }
}