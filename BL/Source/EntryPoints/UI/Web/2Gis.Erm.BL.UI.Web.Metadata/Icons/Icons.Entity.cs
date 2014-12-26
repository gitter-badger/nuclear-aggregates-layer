using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Icons
{
    public static partial class Icons
    {
        public static class Entity
        {
            public const string Default = "en_ico_lrg_Default.gif";
            public const string DefaultSmall = "en_ico_16_Default.gif";

            public static string Large(EntityName entity)
            {
                return string.Format("en_ico_lrg_{0}.gif", entity);
            }

            public static string Small(EntityName entity)
            {
                return string.Format("en_ico_16_{0}.gif", entity);
            }
        }
    }
}