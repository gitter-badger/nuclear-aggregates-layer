using System;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{   
    /// <summary>
    /// The <see cref="DisposableStrategyExtension"/>
    ///   class is used to define the build strategy for disposing objects on tear down by a <see cref="IUnityContainer"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="DisposableStrategyExtension"/> tracks build trees as instances are created by a container due to either a <see cref="IUnityContainer.Resolve"/> 
    ///     or <see cref="IUnityContainer.BuildUp"/> invocations. The extension then disposes instances in a build tree when <see cref="IUnityContainer.Teardown"/>
    ///     is invoked on an instance resolved by the container, built up by the container or when the container is disposed.
    ///   </para>
    /// <note>
    /// This extension should be configured on containers that are used by Neovolve.Toolkit.Server.Unity.UnityServiceHostFactory, 
    ///     <see cref="UnityServiceElement"/> or 
    ///     Neovolve.Toolkit.Server.Unity.UnityHttpModule.
    ///   </note>
    /// <para>
    /// A build tree identifies the hierarchy of instances as they are injected into other instances. 
    ///     The root of a build tree is unique to a combination of a <see cref="NamedTypeBuildKey"/> and an object instance. 
    ///     Child nodes in a build tree may not be unique to a build tree due to a <see cref="ILifetimePolicy"/> 
    ///     in the container returning an existing instance rather than creating a new instance. 
    ///     For example, the <see cref="ContainerControlledLifetimeManager"/> will ensure that the same instance for a build key is returned rather than creating
    ///     a new instance. In this case the same instance (for the build key) may exist multiple times within a build tree and/or across multiple build trees.
    ///   </para>
    /// <para>
    /// When <see cref="IUnityContainer.Teardown"/> is invoked on a container, the <see cref="DisposableStrategyExtension"/> will attempt to find the instance
    ///     as a root node in the set of build trees created by the container. When a build tree is found, a top-down recursive disposal pattern is used to tear down each
    ///     instance in the build tree according to a set of disposal rules.
    ///   </para>
    /// <para>
    /// The tear down process has the following behaviors:
    ///     <list type="bullet">
    /// <item>
    /// <description>Any <see cref="ObjectDisposedException"/> thrown on <see cref="IDisposable.Dispose"/> is caught and ignored.</description>
    /// </item>
    /// <item>
    /// <description>Build trees are removed when they are torn down.</description>
    /// </item>
    /// <item>
    /// <description>Build trees are removed if their root instances have been garbage collected.</description>
    /// </item>
    /// <item>
    /// <description>All build trees are disposed when the <see cref="IUnityContainer"/> is disposed.</description>
    /// </item>
    /// </list>
    /// .
    ///   </para>
    /// <para>
    /// The following disposal rules are used to tear down a build tree:
    ///     <list type="number">
    /// <item>
    /// <description>The instance being torn down must exist as the root instance of a build tree.</description>
    /// </item>
    /// <item>
    /// <description>The root node is skipped if it was not created by the container (meaning it was built up by the container).</description>
    /// </item>
    /// <item>
    /// <description>Tree nodes are skipped if they are found in a <see cref="ILifetimePolicy"/>.
    ///         </description>
    /// </item>
    /// <item>
    /// <description>Child nodes of tree nodes that are in a <see cref="ILifetimePolicy"/> are skipped.</description>
    /// </item>
    /// <item>
    /// <description>Tree nodes already claimed by the garbage collector are skipped.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// The <see cref="DisposableStrategyExtension"/> can be configured via the application configuration or at runtime.
    ///   </para>
    /// <code lang="xml" title="Extension added via configuration">
    /// <![CDATA[<?xml version="1.0" ?>
    /// <configuration>
    ///     <configSections>
    ///         <section name="unity"
    ///                  type="Microsoft.Practices.Unity.Configuration.UnityConfigurationSection, Microsoft.Practices.Unity.Configuration"/>
    ///     </configSections>
    ///     <unity>
    ///         <containers>
    ///             <container>
    ///                 <register type="System.Security.Cryptography.HashAlgorithm, mscorlib"
    ///                           mapTo="System.Security.Cryptography.SHA256CryptoServiceProvider, System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/>
    ///                 <extensions>
    ///                     <add type="Neovolve.Toolkit.Unity.DisposableStrategyExtension, Neovolve.Toolkit.Unity"/>
    ///                 </extensions>
    ///             </container>
    ///         </containers>
    ///     </unity>
    /// </configuration>
    /// ]]>
    ///   </code>
    /// <para>
    /// The <see cref="DisposableStrategyExtension"/> can be configured for a <see cref="IUnityContainer"/> at runtime.
    ///   </para>
    /// <code lang="c#" title="Extension added at runtime">
    /// <![CDATA[public void ExampleExtensionUsage()
    /// {
    ///     IDisposableType actual;
    /// 
    ///     using (UnityContainer container = new UnityContainer())
    ///     {
    ///         using (DisposableStrategyExtension disposableStrategyExtension = new DisposableStrategyExtension())
    ///         {
    ///             container.AddExtension(disposableStrategyExtension);
    ///             container.RegisterType(typeof(IDisposableType), typeof(DisposableType), new TransientLifetimeManager());
    /// 
    ///             actual = container.Resolve<IDisposableType>();
    /// 
    ///             container.Teardown(actual);
    ///         }
    ///     }
    /// }]]>
    ///   </code>
    /// </example>
    public class DisposableStrategyExtension : UnityContainerExtension, IDisposable
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "DisposableStrategyExtension" /> class.
        /// </summary>
        public DisposableStrategyExtension()
            : this(new BuildTreeTracker())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableStrategyExtension"/> class.
        /// </summary>
        /// <param name="buildTreeTracker">
        /// The build tree tracker.
        /// </param>
        internal DisposableStrategyExtension(IBuildTreeTracker buildTreeTracker)
        {
            TreeTracker = buildTreeTracker;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                // Free managed resources
                TreeTracker.DisposeAllTrees();
            }

            // Free native resources if there are any.
        }

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        ///   <see cref="T:Microsoft.Practices.Unity.ExtensionContext"/> by adding strategies, policies, etc. to
        ///   install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            Context.Strategies.Add(TreeTracker, UnityBuildStage.PreCreation);
        }

        /// <summary>
        ///   Gets or sets the tree tracker.
        /// </summary>
        /// <value>
        ///   The tree tracker.
        /// </value>
        private IBuildTreeTracker TreeTracker
        {
            get;
            set;
        }
    }
}