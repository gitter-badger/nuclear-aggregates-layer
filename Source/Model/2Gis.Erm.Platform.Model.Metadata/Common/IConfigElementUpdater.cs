using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public interface IConfigElementUpdater
    {
        void SetParent(IConfigElement parentElement);
        void AddChilds(IEnumerable<IConfigElement> childs);
        void AddFeature(IConfigFeature feature);
    }
}