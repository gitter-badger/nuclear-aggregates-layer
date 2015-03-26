namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
    public enum PhonecallPurpose
    {
        NotSet = 0,
        FirstCall = 1,
        ProductPresentation = 3,
        OpportunitiesPresentation = 4,
        OfferApproval = 5,
        DecisionApproval = 6,
        Sale = 10,
        Service = 8,

        /// <summary>
        /// Допродажа
        /// </summary>
        Upsale = 9,
        Prolongation = 7
    }
}