using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum ActivityAggregate
    {
        Activity = EntityName.Activity,
        Appointment = EntityName.Appointment,
        PhoneCall = EntityName.Phonecall,
        Task = EntityName.Task,
        Letter = EntityName.Letter,
    }

	public enum AppointmentAggregate
	{
		Appointment = EntityName.Appointment,
        AppointmentRegardingObject = EntityName.AppointmentRegardingObject,
        AppointmentAttendee = EntityName.AppointmentAttendee,
        AppointmentOrganizer = EntityName.AppointmentOrganizer,
	}
	
	public enum PhonecallAggregate
	{
		Phonecall = EntityName.Phonecall,
        PhonecallRegardingObject = EntityName.PhonecallRegardingObject,
        PhonecallRecipient = EntityName.PhonecallRecipient,
	}
	
	public enum TaskAggregate
	{
		Task = EntityName.Task,
        TaskRegardingObject = EntityName.TaskRegardingObject,
	}

	public enum LetterAggregate
	{
		Letter = EntityName.Letter,
        LetterRegardingObject = EntityName.LetterRegardingObject,
        LetterSender = EntityName.LetterSender,
        LetterRecipient = EntityName.LetterRecipient,
    }
}