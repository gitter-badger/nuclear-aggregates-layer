﻿using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Appointment : ActivityBase
    {
        public ActivityPurpose Purpose { get; set; }
        public byte AfterSaleServiceType { get; set; }
    }
}
