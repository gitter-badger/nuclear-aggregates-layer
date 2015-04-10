namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IDoesLegalPersonHaveAnyProfilesAspect : IAspect
    {
        bool HasProfiles { get; }
    }
}