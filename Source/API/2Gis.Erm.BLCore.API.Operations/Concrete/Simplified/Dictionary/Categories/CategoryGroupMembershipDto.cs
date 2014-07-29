namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories
{
    public sealed class CategoryGroupMembershipDto
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public long? CategoryGroupId { get; set; }
        public int CategoryLevel { get; set; }
    }
}
