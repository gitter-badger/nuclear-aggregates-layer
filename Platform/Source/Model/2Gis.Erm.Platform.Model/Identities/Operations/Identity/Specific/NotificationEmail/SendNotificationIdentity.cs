namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.NotificationEmail
{
    public class SendNotificationIdentity : OperationIdentityBase<SendNotificationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.SendNotificationIdentity; }
        }

        public override string Description
        {
            get { return "Отправка сообщения"; }
        }
    }
}