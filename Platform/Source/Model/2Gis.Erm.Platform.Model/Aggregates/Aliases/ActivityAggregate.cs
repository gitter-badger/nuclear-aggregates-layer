using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates.Aliases
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class ActivityAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Activity(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.Appointment(),
                                    EntityType.Instance.Phonecall(),
                                    EntityType.Instance.Task(),
                                    EntityType.Instance.Letter()
                                })
                    .ToArray();
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class AppointmentAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Appointment(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                                {
                                    EntityType.Instance.AppointmentRegardingObject(),
                                    EntityType.Instance.AppointmentAttendee(),
                                    EntityType.Instance.AppointmentOrganizer()
                                })
                    .ToArray();
            }
        }
    }

    public static class PhonecallAggregate
    {
        public static IEntityType Root
        {
            get { return EntityType.Instance.Phonecall(); }
        }

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
    {
                                    EntityType.Instance.PhonecallRegardingObject(),
                                    EntityType.Instance.PhonecallRecipient()
                                })
                    .ToArray();
    }
        }
    }

    public static class TaskAggregate
	{
        public static IEntityType Root
        {
            get { return EntityType.Instance.Task(); }
	}
	
        public static IEntityType[] Entities
	{
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
                        {
                            EntityType.Instance.TaskRegardingObject()
                        })
                    .ToArray();
            }
        }
	}
	
    public static class LetterAggregate
    {
        public static IEntityType Root
	{
            get { return EntityType.Instance.Letter(); }
	}

        public static IEntityType[] Entities
        {
            get
            {
                return new[] { Root }
                    .Concat(new IEntityType[]
	{
                            EntityType.Instance.LetterRegardingObject(),
                            EntityType.Instance.LetterSender(),
                            EntityType.Instance.LetterRecipient()
                        })
                    .ToArray();
            }
        }
    }
}