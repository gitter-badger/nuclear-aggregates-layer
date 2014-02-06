namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    /// <summary>
    /// Synchronized with phonecall.dg_purpose picklist.
    /// </summary>
    public enum ActivityPurpose
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
