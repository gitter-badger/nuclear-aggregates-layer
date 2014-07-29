namespace DoubleGis.Erm.BLCore.ExtractUseCases.UseCases
{
    public class UseCase
    {
        public string Description { get; set; }
        public UseCaseNode Root { get; set; }
        public int MaxUseCaseDepth { get; set; }

        public override string ToString()
        {
            return string.Format("Depth:{0}. {1}", MaxUseCaseDepth, Description);
        }
    }
}