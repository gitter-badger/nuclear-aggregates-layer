using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Properties;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities
{
    public sealed class TaskTypeIdentity : EntityPropertyIdentityBase<TaskTypeIdentity>
    {
        public override int Id
        {
            get { return 9; }
        }

        public override string Description
        {
            get { return "TaskType"; }
        }

        public override string PropertyName
        {
            get { return "TaskType"; }
        }

        public override Type PropertyType
        {
            get { return typeof(ActivityTaskType); }
        }
    }
}