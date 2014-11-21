using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.MsCrm
{
    [StableContract]
    [DataContract(Namespace = ServiceNamespaces.BasicOperations.MsCrm201309)]
    public enum MsCrmDealStage
    {
        [EnumMember]
        None = 0,

        // -- stages syncronized with dynamics crm

        // Сбор информации
        [EnumMember]
        CollectInformation = DealStage.CollectInformation,

        // Проведение презентации
        [EnumMember]
        HoldingProductPresentation = DealStage.HoldingProductPresentation,

        // Согласование КП
        [EnumMember]
        MatchAndSendProposition = DealStage.MatchAndSendProposition
    }
}