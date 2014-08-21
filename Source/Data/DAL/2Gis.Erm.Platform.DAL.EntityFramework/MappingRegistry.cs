using System;

using AutoMapper;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    internal sealed class MappingRegistry
    {
        static MappingRegistry()
        {
            RegisterMappingFromDal();
            RegisterMappingToDal();
        }

        public static TOutput Map<TInput, TOutput>(TInput input)
        {
            if (Mapper.FindTypeMapFor<TInput, TOutput>() == null)
            {
                throw new InvalidOperationException("The requested mapping is not supported.");
            }

            return Mapper.Map<TOutput>(input);
        }

        public static bool CheckRegistration(Type sourceType, Type targetType)
        {
            return Mapper.FindTypeMapFor(sourceType, targetType) != null;
        }

        private static void RegisterMappingFromDal()
        {
            #region Appointment

            Mapper.CreateMap<AppointmentBase, Appointment>()
                  .ForMember(dto => dto.Header, x => x.MapFrom(t => t.Subject))
                  .ForMember(dto => dto.Status, x => x.MapFrom(t => (ActivityStatus)t.Status))
                  .ForMember(dto => dto.Priority, x => x.MapFrom(t => (ActivityPriority)t.Priority))
                  .ForMember(dto => dto.Purpose, x => x.MapFrom(t => (ActivityPurpose)t.Purpose))
                ;
            Mapper.CreateMap<AppointmentReference, RegardingObject<Appointment>>()
                  .ForMember(dto => dto.SourceEntityId, x => x.MapFrom(t => t.AppointmentId))
                  .ForMember(dto => dto.TargetEntityName, x => x.MapFrom(t => (EntityName)t.ReferencedType))
                  .ForMember(dto => dto.TargetEntityId, x => x.MapFrom(t => t.ReferencedObjectId))
                ;

            #endregion

            #region Phonecall

            Mapper.CreateMap<PhonecallBase, Phonecall>()
                  .ForMember(dto => dto.Header, x => x.MapFrom(t => t.Subject))
                  .ForMember(dto => dto.Status, x => x.MapFrom(t => (ActivityStatus)t.Status))
                  .ForMember(dto => dto.Priority, x => x.MapFrom(t => (ActivityPriority)t.Priority))
                  .ForMember(dto => dto.Purpose, x => x.MapFrom(t => (ActivityPurpose)t.Purpose))
                ;
            Mapper.CreateMap<PhonecallReference, RegardingObject<Phonecall>>()
                  .ForMember(dto => dto.SourceEntityId, x => x.MapFrom(t => t.PhonecallId))
                  .ForMember(dto => dto.TargetEntityName, x => x.MapFrom(t => (EntityName)t.ReferencedType))
                  .ForMember(dto => dto.TargetEntityId, x => x.MapFrom(t => t.ReferencedObjectId))
                ;

            #endregion

            #region Task

            Mapper.CreateMap<TaskBase, Task>()
                  .ForMember(dto => dto.Header, x => x.MapFrom(t => t.Subject))
                  .ForMember(dto => dto.Status, x => x.MapFrom(t => (ActivityStatus)t.Status))
                  .ForMember(dto => dto.Priority, x => x.MapFrom(t => (ActivityPriority)t.Priority))
                  .ForMember(dto => dto.TaskType, x => x.MapFrom(t => (TaskType)t.TaskType))
                ;
            Mapper.CreateMap<TaskReference, RegardingObject<Task>>()
                  .ForMember(dto => dto.SourceEntityId, x => x.MapFrom(t => t.TaskId))
                  .ForMember(dto => dto.TargetEntityName, x => x.MapFrom(t => (EntityName)t.ReferencedType))
                  .ForMember(dto => dto.TargetEntityId, x => x.MapFrom(t => t.ReferencedObjectId))
                ;

            #endregion
        }

        private static void RegisterMappingToDal()
        {
            #region Appointment

            Mapper.CreateMap<Appointment, AppointmentBase>()
                  .ForMember(dto => dto.Subject, x => x.MapFrom(t => t.Header))
                  .ForMember(dto => dto.Status, x => x.MapFrom(t => (int)t.Status))
                  .ForMember(dto => dto.Priority, x => x.MapFrom(t => (int)t.Priority))
                ;
            Mapper.CreateMap<RegardingObject<Appointment>, AppointmentReference>()
                  .ForMember(dest => dest.AppointmentId, cfg => cfg.MapFrom(src => src.SourceEntityId))
                  .ForMember(dest => dest.Reference, cfg => cfg.MapFrom(src => ReferenceType.RegardingObject))
                  .ForMember(dest => dest.ReferencedType, cfg => cfg.MapFrom(src => (int)src.TargetEntityName))
                  .ForMember(dest => dest.ReferencedObjectId, cfg => cfg.MapFrom(src => src.TargetEntityId))
                ;

            #endregion

            #region Phonecall

            Mapper.CreateMap<Phonecall, PhonecallBase>()
                  .ForMember(dto => dto.Subject, x => x.MapFrom(t => t.Header))
                  .ForMember(dto => dto.Status, x => x.MapFrom(t => (int)t.Status))
                  .ForMember(dto => dto.Priority, x => x.MapFrom(t => (int)t.Priority))
                ;
            Mapper.CreateMap<RegardingObject<Phonecall>, PhonecallReference>()
                  .ForMember(dest => dest.PhonecallId, cfg => cfg.MapFrom(src => src.SourceEntityId))
                  .ForMember(dest => dest.Reference, cfg => cfg.MapFrom(src => ReferenceType.RegardingObject))
                  .ForMember(dest => dest.ReferencedType, cfg => cfg.MapFrom(src => (int)src.TargetEntityName))
                  .ForMember(dest => dest.ReferencedObjectId, cfg => cfg.MapFrom(src => src.TargetEntityId))
                ;

            #endregion

            #region Task

            Mapper.CreateMap<Task, TaskBase>()
                  .ForMember(dto => dto.Subject, x => x.MapFrom(t => t.Header))
                  .ForMember(dto => dto.Status, x => x.MapFrom(t => (int)t.Status))
                  .ForMember(dto => dto.Priority, x => x.MapFrom(t => (int)t.Priority))
                  .ForMember(dto => dto.TaskType, x => x.MapFrom(t => (int)t.TaskType))
                ;
            Mapper.CreateMap<RegardingObject<Task>, TaskReference>()
                  .ForMember(dest => dest.TaskId, cfg => cfg.MapFrom(src => src.SourceEntityId))
                  .ForMember(dest => dest.Reference, cfg => cfg.MapFrom(src => ReferenceType.RegardingObject))
                  .ForMember(dest => dest.ReferencedType, cfg => cfg.MapFrom(src => (int)src.TargetEntityName))
                  .ForMember(dest => dest.ReferencedObjectId, cfg => cfg.MapFrom(src => src.TargetEntityId))
                ;

            #endregion
        }
    }
}