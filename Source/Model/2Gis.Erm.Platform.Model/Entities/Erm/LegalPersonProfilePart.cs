using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class LegalPersonProfilePart : IEntity, IEntityKey, IEntityPart
    {
        public long Id { get; set; }
        public BusinessModel BusinessModel { get; set; }
        public AccountType AccountType { get; set; }
        public long BankId { get; set; }
        public string Rut { get; set; }
        public DateTime IssuedOn { get; set; }
        public string IssuedBy { get; set; }
    }
}