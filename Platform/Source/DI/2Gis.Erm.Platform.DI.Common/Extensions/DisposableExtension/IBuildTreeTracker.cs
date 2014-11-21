using System.Collections.Generic;

using Microsoft.Practices.ObjectBuilder2;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="IBuildTreeTracker"/>
    ///   interface is used to track build trees as they are created and destroyed.
    /// </summary>
    internal interface IBuildTreeTracker : IBuilderStrategy
    {
        /// <summary>
        /// Disposes all build trees.
        /// </summary>
        void DisposeAllTrees();

        /// <summary>
        ///   Gets the build trees.
        /// </summary>
        /// <value>
        ///   The build trees.
        /// </value>
        IEnumerable<BuildTreeItemNode> BuildTrees
        {
            get;
        }
    }
}