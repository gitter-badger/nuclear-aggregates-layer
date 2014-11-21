﻿using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic
{
    [DataContract]
    public sealed class UpdateOrganizationStructureDenormalizationIdentity : OperationIdentityBase<UpdateOrganizationStructureDenormalizationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.UpdateOrganizationStructureDenormalization;
            }
        }
        public override string Description
        {
            get
            {
                return "Обновление денормализованного представления структуры организации";
            }
        }
    }
}