using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="BuildTreeStore"/>
    ///   class is used to store build trees.
    /// </summary>
    internal class BuildTreeStore : IBuildTreeStore
    {
        /// <summary>
        ///   Defines the lock used to protect the list of build trees.
        /// </summary>
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        /// <summary>
        ///   Stores the build trees created by the current container.
        /// </summary>
        private readonly List<BuildTreeItemNode> _buildTrees = new List<BuildTreeItemNode>();

        /// <summary>
        /// Adds the specified build tree.
        /// </summary>
        /// <param name="buildTree">
        /// The build tree.
        /// </param>
        public void Add(BuildTreeItemNode buildTree)
        {
            using (new LockWriter(_lock))
            {
                BuildTreeItemNode existingTree = GetExistingBuildTreeInternal(buildTree.ItemReference.Target);

                if (existingTree == null)
                {
                    _buildTrees.Add(buildTree);
                }
            }
        }

        /// <summary>
        /// Gets the build tree for instance.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// A <see cref="BuildTreeItemNode"/> instance.
        /// </returns>
        public BuildTreeItemNode GetBuildTreeForInstance(Object instance)
        {
            using (new LockReader(_lock))
            {
                return GetExistingBuildTreeInternal(instance);
            }
        }

        /// <summary>
        /// Gets the build trees.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> instance.
        /// </returns>
        public IEnumerable<BuildTreeItemNode> GetBuildTrees()
        {
            using (new LockReader(_lock))
            {
                List<BuildTreeItemNode> treeCopy = new List<BuildTreeItemNode>(_buildTrees);

                return new ReadOnlyCollection<BuildTreeItemNode>(treeCopy);
            }
        }

        /// <summary>
        /// Removes the specified build tree.
        /// </summary>
        /// <param name="buildTree">
        /// The build tree.
        /// </param>
        public void Remove(BuildTreeItemNode buildTree)
        {
            using (new LockWriter(_lock))
            {
                _buildTrees.Remove(buildTree);
            }
        }

        /// <summary>
        /// Gets the existing build tree internal.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// A <see cref="BuildTreeItemNode"/> instance.
        /// </returns>
        private BuildTreeItemNode GetExistingBuildTreeInternal(Object instance)
        {
             return _buildTrees.Where(x => x.ItemReference.IsAlive && ReferenceEquals(x.ItemReference.Target, instance)).SingleOrDefault();
        }
    }
}