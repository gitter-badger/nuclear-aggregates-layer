using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Practices.ObjectBuilder2;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="BuildTreeRecovery"/>
    ///   class is used to repair the build tree and a build failure occurs.
    /// </summary>
    internal class BuildTreeRecovery : IRequiresRecovery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildTreeRecovery"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="failedNode">
        /// The current node.
        /// </param>
        /// <param name="failureAction">
        /// The failure action.
        /// </param>
        public BuildTreeRecovery(IBuilderContext context, BuildTreeItemNode failedNode, Action<BuildTreeItemNode> failureAction)
        {
            Context = context;
            FailedNode = failedNode;
            FailureAction = failureAction;
        }

        /// <summary>
        /// A method that does whatever is needed to clean up
        ///   as part of cleaning up after an exception.
        /// </summary>
        /// <remarks>
        /// Don't do anything that could throw in this method,
        ///   it will cause later recover operations to get skipped
        ///   and play real havoc with the stack trace.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", 
            Justification = "Generic exception catch caters for the Unity advice to not allow exceptions in this method to be thrown.")]
        public void Recover()
        {
            try
            {
                if (FailureAction != null)
                {
                    FailureAction(FailedNode);
                }

                BuildTreeItemNode parentNode = FailedNode.Parent;

                BuildTreeDisposer.DisposeTree(Context, FailedNode);

                if (parentNode != null)
                {
                    parentNode.Children.Remove(FailedNode);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Recovery failed: " + ex);
            }
        }

        /// <summary>
        ///   Gets the context.
        /// </summary>
        /// <value>
        ///   The context.
        /// </value>
        protected IBuilderContext Context
        {
            get;
            private set;
        }

        /// <summary>
        ///   Gets the failed node.
        /// </summary>
        /// <value>
        ///   The failed node.
        /// </value>
        protected BuildTreeItemNode FailedNode
        {
            get;
            private set;
        }

        /// <summary>
        ///   Gets the failure action.
        /// </summary>
        /// <value>
        ///   The failure action.
        /// </value>
        protected Action<BuildTreeItemNode> FailureAction
        {
            get;
            private set;
        }
    }
}