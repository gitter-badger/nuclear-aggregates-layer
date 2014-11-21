using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="IBuildTreeStore"/>
    ///   interface is used to manage the storage of build trees.
    /// </summary>
    internal interface IBuildTreeStore
    {
        /// <summary>
        /// Adds the specified build tree.
        /// </summary>
        /// <param name="buildTree">
        /// The build tree.
        /// </param>
        void Add(BuildTreeItemNode buildTree);

        /// <summary>
        /// Gets the build tree for instance.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// A <see cref="BuildTreeItemNode"/> instance.
        /// </returns>
        BuildTreeItemNode GetBuildTreeForInstance(Object instance);

        /// <summary>
        /// Gets the build trees.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> instance.
        /// </returns>
        IEnumerable<BuildTreeItemNode> GetBuildTrees();

        /// <summary>
        /// Removes the specified build tree.
        /// </summary>
        /// <param name="buildTree">
        /// The build tree.
        /// </param>
        void Remove(BuildTreeItemNode buildTree);
    }
}