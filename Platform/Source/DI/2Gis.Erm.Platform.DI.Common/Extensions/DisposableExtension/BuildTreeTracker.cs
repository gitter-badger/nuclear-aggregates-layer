using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.ObjectBuilder2;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="BuildTreeTracker"/>
    ///   class is used to track build trees created by the container.
    /// </summary>
    internal class BuildTreeTracker : BuilderStrategy, IBuildTreeTracker
    {
        /// <summary>
        ///   Stores the build trees.
        /// </summary>
        private readonly IBuildTreeStore _store;

        /// <summary>
        ///   The _current build node.
        /// </summary>
        [ThreadStatic]
        private static BuildTreeItemNode _currentBuildNode;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BuildTreeTracker" /> class.
        /// </summary>
        public BuildTreeTracker()
            : this(new BuildTreeStore())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildTreeTracker"/> class.
        /// </summary>
        /// <param name="store">
        /// The store.
        /// </param>
        public BuildTreeTracker(IBuildTreeStore store)
        {
            _store = store;
        }

        /// <summary>
        /// Creates the tracked deferred resolution.
        /// </summary>
        /// <param name="originalDeferredFunction">
        /// The original deferred function.
        /// </param>
        /// <returns>
        /// A <see cref="MulticastDelegate"/> instance.
        /// </returns>
        public Delegate CreateTrackedDeferredResolution(Delegate originalDeferredFunction)
        {
            Type delegateType = originalDeferredFunction.GetType();

            if (delegateType.IsGenericType == false)
            {
                return originalDeferredFunction;
            }

            Type genericDelegateType = delegateType.GetGenericTypeDefinition();

            Debug.Assert(genericDelegateType != null, "The type is not a generic type.");

            if (genericDelegateType.Equals(typeof(Func<>)) == false)
            {
                return originalDeferredFunction;
            }

            // This looks like a lazy loaded delegate for dependency injection
            // We need to redirect this through another method to manage the original build tree 
            // when the delegate invocation creates more instances
            Type[] genericArguments = delegateType.GetGenericArguments();

            if (genericArguments.Length > 1)
            {
                return originalDeferredFunction;
            }

            Type genericArgument = genericArguments[0];
            Type[] typeArguments = new[]
                                   {
                                       genericArgument
                                   };
            Type deferredTrackerType = typeof(DeferredResolutionTracker<>);

            Contract.Assume(deferredTrackerType.IsGenericTypeDefinition);
            Contract.Assume(deferredTrackerType.GetGenericArguments().Length == typeArguments.Length);

            Type genericDeferredTrackerType = deferredTrackerType.MakeGenericType(typeArguments);
            MethodInfo resolveMethod = genericDeferredTrackerType.GetMethod("Resolve");
            Object[] trackerArguments = new Object[]
                                        {
                                            originalDeferredFunction, Store, CurrentBuildNode
                                        };
            Object resolvedTracker = Activator.CreateInstance(genericDeferredTrackerType, trackerArguments);

            return Delegate.CreateDelegate(delegateType, resolvedTracker, resolveMethod);
        }

        /// <summary>
        /// Disposes all trees.
        /// </summary>
        public void DisposeAllTrees()
        {
            IEnumerable<BuildTreeItemNode> buildTrees = Store.GetBuildTrees();

            foreach (BuildTreeItemNode buildTree in buildTrees)
            {
                DisposeTree(null, buildTree);
            }
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        ///   PostBuildUp method is called when the chain has finished the PreBuildUp
        ///   phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">
        /// Context of the build operation.
        /// </param>
        public override void PostBuildUp(IBuilderContext context)
        {
            if (context != null)
            {
                // Check if the item created is Func<T> for lazy loaded dependency injection
                Delegate lazyDelegate = context.Existing as Delegate;

                if (lazyDelegate != null)
                {
                    context.Existing = CreateTrackedDeferredResolution(lazyDelegate);
                }

                AssignInstanceToCurrentTreeNode(context.BuildKey, context.Existing);

                if (IsValidBuildTreeNode(CurrentBuildNode) == false)
                {
                    throw new InvalidOperationException("The dependency has not been successfully created.");
                }

                Contract.Assume(CurrentBuildNode != null);
                Contract.Assume(CurrentBuildNode.ItemReference != null);
                Contract.Assume(CurrentBuildNode.ItemReference.IsAlive);

                BuildTreeItemNode parentNode = CurrentBuildNode.Parent;

                if (parentNode == null)
                {
                    // This is the end of the creation of the root node
                    Store.Add(CurrentBuildNode);
                }

                // Move the current node back up to the parent
                // If this is the top level node, this will set the current node back to null
                CurrentBuildNode = parentNode;
            }

            base.PostBuildUp(context);
        }

        /// <summary>
        /// Called during the chain of responsibility for a teardown operation. The
        ///   PostTearDown method is called when the chain has finished the PreTearDown
        ///   phase and executes in reverse order from the PreTearDown calls.
        /// </summary>
        /// <param name="context">
        /// Context of the teardown operation.
        /// </param>
        public override void PostTearDown(IBuilderContext context)
        {
            base.PostTearDown(context);

            // Get the build tree for this item
            if (context != null)
            {
                BuildTreeItemNode buildTree = Store.GetBuildTreeForInstance(context.Existing);

                if (buildTree != null)
                {
                    DisposeTree(context, buildTree);
                }

                DisposeDeadTrees(context);
            }
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        ///   PreBuildUp method is called when the chain is being executed in the
        ///   forward direction.
        /// </summary>
        /// <param name="context">
        /// Context of the build operation.
        /// </param>
        public override void PreBuildUp(IBuilderContext context)
        {
            base.PreBuildUp(context);

            if (context == null)
            {
                return;
            }

            Boolean nodeCreatedByContainer = context.Existing == null;

            if (context.BuildKey == null)
            {
                return;
            }

            BuildTreeItemNode newTreeNode = new BuildTreeItemNode(context.BuildKey, nodeCreatedByContainer, CurrentBuildNode);

            if (CurrentBuildNode != null)
            {
                // This is a child node
                CurrentBuildNode.Children.Add(newTreeNode);
            }

            CurrentBuildNode = newTreeNode;

            BuildTreeRecovery recovery = new BuildTreeRecovery(context, newTreeNode, failedNode => CurrentBuildNode = failedNode.Parent);

            context.RecoveryStack.Add(recovery);
        }

        /// <summary>
        /// Assigns the instance to current tree node.
        /// </summary>
        /// <param name="buildKey">
        /// The build key.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        private static void AssignInstanceToCurrentTreeNode(NamedTypeBuildKey buildKey, Object instance)
        {
            if (CurrentBuildNode.BuildKey != buildKey)
            {
                String message = String.Format(
                    CultureInfo.CurrentCulture, "BuildTreeTracker_BuildTreeConstructedOutOfOrder", CurrentBuildNode.BuildKey, buildKey);

                throw new InvalidOperationException(message);
            }

            CurrentBuildNode.AssignInstance(instance);
        }

        /// <summary>
        /// Finds the nodes in lifetime manager.
        /// </summary>
        /// <param name="context">
        /// The builder context.
        /// </param>
        /// <param name="node">
        /// The node to check.
        /// </param>
        /// <param name="nodesInLifetimeManager">
        /// The nodes in lifetime manager.
        /// </param>
        private static void FindNodesInLifetimeManager(
            IBuilderContext context, BuildTreeItemNode node, IDictionary<Object, BuildTreeItemNode> nodesInLifetimeManager)
        {
            if (node == null)
            {
                return;
            }

            if (IsTreeNodeReferenceValid(node))
            {
                Object nodeTarget = node.ItemReference.Target;

                ILifetimeContainer lifetimeContainer = context.Lifetime;

                if (lifetimeContainer != null)
                {
                    if (lifetimeContainer.OfType<ILifetimePolicy>().Any(lifetimeManager => ReferenceEquals(lifetimeManager.GetValue(), nodeTarget)))
                    {
                        if (node.Parent != null)
                        {
                            node.Parent.Children.Remove(node);
                        }

                        // This instance is in a lifetime manager
                        // Check if the instance has already been found in this tree
                        if (nodesInLifetimeManager.ContainsKey(nodeTarget) == false)
                        {
                            // Add this node to the list
                            nodesInLifetimeManager.Add(nodeTarget, node);
                        }
                    }
                }
            }

            for (Int32 index = node.Children.Count - 1; index >= 0; index--)
            {
                BuildTreeItemNode child = node.Children[index];
                FindNodesInLifetimeManager(context, child, nodesInLifetimeManager);
            }
        }

        /// <summary>
        /// Determines whether the specified node has a valid reference.
        /// </summary>
        /// <param name="node">
        /// The node to check.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified node has a valid reference; otherwise, <c>false</c>.
        /// </returns>
        private static Boolean IsTreeNodeReferenceValid(BuildTreeItemNode node)
        {
            if (node == null)
            {
                return false;
            }

            if (node.ItemReference == null)
            {
                return false;
            }

            if (node.ItemReference.IsAlive == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified node is valid to be part of a build tree.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified node is valid to be part of a build tree; otherwise, <c>false</c>.
        /// </returns>
        private static Boolean IsValidBuildTreeNode(BuildTreeItemNode node)
        {
            if (node == null)
            {
                return false;
            }

            if (node.ItemReference == null)
            {
                return false;
            }

            if (node.ItemReference.IsAlive == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Disposes the dead trees.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        private void DisposeDeadTrees(IBuilderContext context)
        {
            IEnumerable<BuildTreeItemNode> buildTrees = Store.GetBuildTrees();

            foreach (BuildTreeItemNode buildTree in buildTrees)
            {
                if (IsTreeNodeReferenceValid(buildTree))
                {
                    continue;
                }

                DisposeTree(context, buildTree);
            }
        }

        /// <summary>
        /// Disposes the tree.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="buildTree">
        /// The build tree.
        /// </param>
        private void DisposeTree(IBuilderContext context, BuildTreeItemNode buildTree)
        {
            if (context != null)
            {
                // Lifetime manager nodes can only be determined if there is a context
                // If this is called because the extension is being disposed then there is no context
                PromoteLifetimeManagerNodesToBuildTrees(context, buildTree);
            }

            BuildTreeDisposer.DisposeTree(context, buildTree);

            Store.Remove(buildTree);
        }

        /// <summary>
        /// Promotes the lifetime manager nodes to build trees.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="buildTree">
        /// The build tree.
        /// </param>
        private void PromoteLifetimeManagerNodesToBuildTrees(IBuilderContext context, BuildTreeItemNode buildTree)
        {
            IDictionary<Object, BuildTreeItemNode> lifetimeNodes = new Dictionary<Object, BuildTreeItemNode>();

            FindNodesInLifetimeManager(context, buildTree, lifetimeNodes);

            foreach (KeyValuePair<Object, BuildTreeItemNode> lifetimeNode in lifetimeNodes)
            {
                if (lifetimeNode.Value == null)
                {
                    continue;
                }

                if (lifetimeNode.Value.ItemReference == null)
                {
                    continue;
                }

                if (lifetimeNode.Value.ItemReference.IsAlive == false)
                {
                    continue;
                }

                Store.Add(lifetimeNode.Value);
            }
        }

        /// <summary>
        ///   Gets or sets the current build node.
        /// </summary>
        /// <value>
        ///   The current build node.
        /// </value>
        private static BuildTreeItemNode CurrentBuildNode
        {
            get
            {
                return _currentBuildNode;
            }

            set
            {
                _currentBuildNode = value;
            }
        }

        /// <summary>
        ///   Gets the build trees.
        /// </summary>
        /// <value>
        ///   The build trees.
        /// </value>
        public virtual IEnumerable<BuildTreeItemNode> BuildTrees
        {
            get
            {
                return Store.GetBuildTrees();
            }
        }

        /// <summary>
        ///   Gets the store.
        /// </summary>
        /// <value>
        ///   The store.
        /// </value>
        private IBuildTreeStore Store
        {
            get
            {
                return _store;
            }
        }
    }
}