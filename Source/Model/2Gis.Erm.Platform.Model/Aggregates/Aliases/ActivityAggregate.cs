using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    public enum ActivityAggregate
    {
        Activity = EntityName.Activity,
        Appointment = EntityName.Appointment,
        PhoneCall = EntityName.Phonecall,
        Task = EntityName.Task,
    }

	public enum AppointmentAggregate
	{
		Appointment = EntityName.Appointment,
		RegardingObjectReference = EntityName.RegardingObjectReference,
	}
	
	public enum PhonecallAggregate
	{
		Phonecall = EntityName.Phonecall,
		RegardingObjectReference = EntityName.RegardingObjectReference,
	}
	
	public enum TaskAggregate
	{
		Task = EntityName.Task,
		RegardingObjectReference = EntityName.RegardingObjectReference,
	}
}