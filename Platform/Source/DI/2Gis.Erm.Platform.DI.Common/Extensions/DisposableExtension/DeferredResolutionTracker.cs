using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="DeferredResolutionTracker{T}"/>
    ///   class is used to manage deferred resolution functions to ensure that build trees
    ///   contain references to lazy loaded instances.
    /// </summary>
    /// <typeparam name="T">
    /// The type of instance being resolved.
    /// </typeparam>
    internal class DeferredResolutionTracker<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeferredResolutionTracker{T}"/> class.
        /// </summary>
        /// <param name="resolutionFunction">
        /// The resolution function.
        /// </param>
        /// <param name="buildTreeStore">
        /// The build tree store.
        /// </param>
        /// <param name="parentNode">
        /// The parent node.
        /// </param>
        public DeferredResolutionTracker(Func<T> resolutionFunction, IBuildTreeStore buildTreeStore, BuildTreeItemNode parentNode)
        {
            ResolutionFunction = resolutionFunction;
            TreeStore = buildTreeStore;
            ParentNode = parentNode;
        }

        /// <summary>
        /// Resolves this instance.
        /// </summary>
        /// <returns>
        /// The resolved instance.
        /// </returns>
        public T Resolve()
        {
            T resolvedInstance = ResolutionFunction();
            IEnumerable<BuildTreeItemNode> buildTrees = TreeStore.GetBuildTrees();

            BuildTreeItemNode deferredBuildTree = buildTrees.Where(x => IsDeferredBuildTreeReference(x, resolvedInstance)).FirstOrDefault();

            if (deferredBuildTree == null)
            {
                return resolvedInstance;
            }

            // We have found the deferred build tree for the instance created by the deferred resolution function
            TreeStore.Remove(deferredBuildTree);

            // Add the build tree as a child to the parent node
            ParentNode.Children.Add(deferredBuildTree);

            return resolvedInstance;
        }

        /// <summary>
        /// Determines whether the specified build tree is related to the resolved instance.
        /// </summary>
        /// <param name="buildTreeNode">
        /// The build tree node.
        /// </param>
        /// <param name="resolvedInstance">
        /// The resolved instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified build tree is related to the resolved instance; otherwise, <c>false</c>.
        /// </returns>
        private static Boolean IsDeferredBuildTreeReference(BuildTreeItemNode buildTreeNode, T resolvedInstance)
        {
            if (buildTreeNode == null)
            {
                return false;
            }

            if (buildTreeNode.ItemReference.IsAlive == false)
            {
                return false;
            }

            if (ReferenceEquals(buildTreeNode.ItemReference.Target, resolvedInstance))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///   Gets or sets the parent node.
        /// </summary>
        /// <value>
        ///   The parent node.
        /// </value>
        private BuildTreeItemNode ParentNode
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the resolution function.
        /// </summary>
        /// <value>
        ///   The resolution function.
        /// </value>
        private Func<T> ResolutionFunction
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the tree store.
        /// </summary>
        /// <value>
        ///   The tree store.
        /// </value>
        private IBuildTreeStore TreeStore
        {
            get;
            set;
        }
    }
}