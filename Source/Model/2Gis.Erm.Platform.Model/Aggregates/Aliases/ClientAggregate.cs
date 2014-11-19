using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum ClientAggregate
    {
        Client = EntityName.Client,
        Firm = EntityName.Firm, //
        LegalPerson = EntityName.LegalPerson, //
        LegalPersonProfile = EntityName.LegalPersonProfile, //
        Account = EntityName.Account, //
        Limit = EntityName.Limit, //
        Bargain = EntityName.Bargain, //
        Deal = EntityName.Deal, //
        Order = EntityName.Order, //
        OrderPosition = EntityName.OrderPosition, //
        Contact = EntityName.Contact, 
        ClientLink = EntityName.ClientLink, //
        DenormalizedClientLink = EntityName.DenormalizedClientLink,

        // FIXME {s.pomadin, 24.09.2014}: remove after merge with AM branch
        Appointment = EntityName.Appointment,
        Phonecall = EntityName.Phonecall,
        Task = EntityName.Task,
    }
}
