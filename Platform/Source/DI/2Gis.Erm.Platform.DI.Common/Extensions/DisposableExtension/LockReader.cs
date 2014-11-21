using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="LockReader"/>
    ///   class is used to provide thread safe read access to a resource using a provided 
    ///   <see cref="ReaderWriterLock"/> or <see cref="ReaderWriterLockSlim"/> instance.
    /// </summary>
    /// <remarks>
    /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='RecommendReaderWriterLockSlim']/*"/>
    /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='AvoidExternalRelease']/*"/>
    /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='RecommendUsingStatement']/*"/>
    /// </remarks>
    /// <threadsafety instance="true"/>
    /// <example>
    /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Examples/Example[@name='ReaderWriterLockSlim']/*"/>
    /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Examples/Example[@name='ReaderWriterLock']/*"/>
    /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Examples/Example[@name='IncorrectUsage']/*"/>
    /// </example>
    public sealed class LockReader : IDisposable
    {
        /// <overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LockReader"/> class.
        ///   </summary>
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LockReader"/> class using the provided lock reference.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='NotUpgradeableReadLock']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='DescribeUpgradeableLock']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='InfiniteTimeout']/*"/>
        /// </remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='NullLockReferenceException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='LockRecursionException']/*"/>
        public LockReader(ReaderWriterLockSlim lockReference)
            : this(lockReference, Timeout.Infinite, false)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockReader"/> class using the provided lock reference and timeout value.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <param name="timeout">
        /// The timeout value within which the lock must be entered, measured in milliseconds.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='NotUpgradeableReadLock']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='DescribeUpgradeableLock']/*"/>
        /// </remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='NullLockReferenceException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='TimeoutOutOfRangeException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='LockRecursionException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='TimeoutException']/*"/>
        public LockReader(ReaderWriterLockSlim lockReference, Int32 timeout)
            : this(lockReference, timeout, false)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockReader"/> class using the provided lock reference and lock upgrade flag.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <param name="obtainUpgradeableLock">
        /// <c>true</c> to obtain an upgradeable lock; otherwise <c>false</c>.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='DescribeUpgradeableLock']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='InfiniteTimeout']/*"/>
        /// </remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='NullLockReferenceException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='LockRecursionException']/*"/>
        public LockReader(ReaderWriterLockSlim lockReference, Boolean obtainUpgradeableLock)
            : this(lockReference, Timeout.Infinite, obtainUpgradeableLock)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockReader"/> class using the provided lock reference, timeout value and lock upgrade flag.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <param name="timeout">
        /// The timeout value within which the lock must be entered, measured in milliseconds.
        /// </param>
        /// <param name="obtainUpgradeableLock">
        /// <c>true</c> to obtain an upgradeable lock; otherwise <c>false</c>.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='DescribeUpgradeableLock']/*"/>
        /// </remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='TimeoutOutOfRangeException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='LockRecursionException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='TimeoutException']/*"/>
        public LockReader(ReaderWriterLockSlim lockReference, Int32 timeout, Boolean obtainUpgradeableLock)
        {
            UpgradableLockRequested = obtainUpgradeableLock;
            LockReferenceSlim = lockReference;

            // NOTE: This will fail if there is an existing reader lock for this thread and the recursion policy doesn't allow nested read locks
            if (UpgradableLockRequested)
            {
                if (LockReferenceSlim.TryEnterUpgradeableReadLock(timeout) == false)
                {
                    throw new TimeoutException();
                }
            }
            else
            {
                if (LockReferenceSlim.TryEnterReadLock(timeout) == false)
                {
                    throw new TimeoutException();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockReader"/> class using the provided lock reference.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='PreferReaderWriterLockSlimUsage']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='InfiniteTimeout']/*"/>
        /// </remarks>
        public LockReader(ReaderWriterLock lockReference)
            : this(lockReference, Timeout.Infinite)
        {
           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockReader"/> class using the provided lock reference and timeout value.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <param name="timeout">
        /// The timeout value within which the lock must be entered, measured in milliseconds.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='PreferReaderWriterLockSlimUsage']/*"/>
        /// </remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='TimeoutException']/*"/>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", 
            Justification = "Code contracts validate the parameter.")]
        public LockReader(ReaderWriterLock lockReference, Int32 timeout)
        {
            LockReference = lockReference;

            // Check if there is a writer lock already for this thread
            if (lockReference.IsWriterLockHeld == false)
            {
                try
                {
                    // Only cause a reader lock if the current thread doesn't already have a writer lock
                    lockReference.AcquireReaderLock(timeout);
                }
                catch (ApplicationException ex)
                {
                    // Wrap the ReaderWriterLocks native timeout exception (ApplicationException) in an actual TimeoutException
                    throw new TimeoutException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// The <see cref="Dispose"/> method releases the lock that was acquired by this instance.
        /// </remarks>
        public void Dispose()
        {
            // Check if this instance is already disposed
            if (Disposed)
            {
                return;
            }

            Disposed = true;

            // Check if there is a slim lock
            if (LockReferenceSlim != null)
            {
                // There is a ReaderWriterLockSlim
                if (UpgradableLockRequested)
                {
                    if (LockReferenceSlim.IsUpgradeableReadLockHeld)
                    {
                        LockReferenceSlim.ExitUpgradeableReadLock();
                    }
                }
                else
                {
                    // Check if there is a read lock
                    if (LockReferenceSlim.IsReadLockHeld)
                    {
                        // Exit the read lock
                        LockReferenceSlim.ExitReadLock();
                    }
                }
            }
            else
            {
                // There is a ReaderWriterLock
                // If there is a writer lock, we have not called for a reader lock in this instance
                if (LockReference.IsWriterLockHeld)
                {
                    return;
                }

                // Check if there is still a read lock
                if (LockReference.IsReaderLockHeld)
                {
                    // Release the read lock
                    LockReference.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref = "LockReader" /> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        private Boolean Disposed
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the lock reference.
        /// </summary>
        /// <value>
        ///   The lock reference.
        /// </value>
        private ReaderWriterLock LockReference
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets the lock reference slim.
        /// </summary>
        /// <value>
        ///   The lock reference slim.
        /// </value>
        private ReaderWriterLockSlim LockReferenceSlim
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether an upgradable lock was requested.
        /// </summary>
        /// <value>
        ///   <c>true</c> if an upgradable lock was requested; otherwise, <c>false</c>.
        /// </value>
        private Boolean UpgradableLockRequested
        {
            get;
            set;
        }
    }
}