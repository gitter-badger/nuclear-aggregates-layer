using System;
using System.Diagnostics;
using System.Linq;

using Microsoft.Practices.ObjectBuilder2;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="BuildTreeDisposer"/>
    ///   class is used to dispose build trees.
    /// </summary>
    internal static class BuildTreeDisposer
    {
        /// <summary>
        /// Disposes the tree.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="buildTree">
        /// The build tree.
        /// </param>
        public static void DisposeTree(IBuilderContext context, BuildTreeItemNode buildTree)
        {
            if (buildTree == null)
            {
                return;
            }

            TeardownTreeNode(context, buildTree);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        /// <param name="instanceReference">
        /// The instance reference.
        /// </param>
        private static void DisposeInstance(WeakReference instanceReference)
        {
            if (instanceReference == null)
            {
                return;
            }

            if (instanceReference.IsAlive == false)
            {
                return;
            }

            DisposeInstance(instanceReference.Target);
        }

        /// <summary>
        /// Disposes the instance.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private static void DisposeInstance(Object instance)
        {
            if (instance == null)
            {
                return;
            }

            IDisposable disposable = instance as IDisposable;

            if (disposable != null)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    Debug.WriteLine("Object was already disposed");
                }
            }
        }

        /// <summary>
        /// Instances the exists in lifetime manager.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="instanceReference">
        /// The instance reference.
        /// </param>
        /// <returns>
        /// A <see cref="Boolean"/> instance.
        /// </returns>
        private static Boolean InstanceExistsInLifetimeManager(IBuilderContext context, WeakReference instanceReference)
        {
            if (context == null)
            {
                return false;
            }

            if (instanceReference == null)
            {
                return false;
            }

            Object instance = instanceReference.Target;

            ILifetimeContainer lifetimeContainer = context.Lifetime;

            if (lifetimeContainer == null)
            {
                return false;
            }

            return lifetimeContainer.OfType<ILifetimePolicy>().Any(lifetimeManager => ReferenceEquals(lifetimeManager.GetValue(), instance));
        }

        /// <summary>
        /// Teardowns the tree node.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="treeNode">
        /// The tree node.
        /// </param>
        private static void TeardownTreeNode(IBuilderContext context, BuildTreeItemNode treeNode)
        {
            // If the parent node can't be torn down then neither can any of the children
            if (InstanceExistsInLifetimeManager(context, treeNode.ItemReference))
            {
                return;
            }

            // Only nodes created by the unit container will be disposed
            if (treeNode.NodeCreatedByContainer)
            {
                DisposeInstance(treeNode.ItemReference);
            }

            // Recursively call through the child nodes
            for (Int32 index = 0; index < treeNode.Children.Count; index++)
            {
                BuildTreeItemNode child = treeNode.Children[index];

                TeardownTreeNode(context, child);
            }

            return;
        }
    }
}