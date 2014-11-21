using System;
using System.Threading;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.DisposableExtension
{
    /// <summary>
    /// The <see cref="LockWriter"/>
    ///   class is used to provide thread safe write access to a resource using a provided
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
    public sealed class LockWriter : IDisposable
    {
        /// <overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LockWriter"/> class.
        ///   </summary>
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="LockWriter"/> class using the provided lock reference.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='InfiniteTimeout']/*"/>
        /// </remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='NullLockReferenceException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='LockRecursionException']/*"/>
        public LockWriter(ReaderWriterLockSlim lockReference)
            : this(lockReference, Timeout.Infinite)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockWriter"/> class using the provided lock reference and timeout value.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <param name="timeout">
        /// The timeout value within which the lock must be entered, measured in milliseconds.
        /// </param>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='TimeoutOutOfRangeException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='LockRecursionException']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Exceptions/Exception[@name='TimeoutException']/*"/>
        public LockWriter(ReaderWriterLockSlim lockReference, Int32 timeout)
        {
            LockReferenceSlim = lockReference;

            // Acquire a write lock
            if (LockReferenceSlim.TryEnterWriteLock(timeout) == false)
            {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockWriter"/> class using the provided lock reference.
        /// </summary>
        /// <param name="lockReference">
        /// The lock reference.
        /// </param>
        /// <remarks>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='PreferReaderWriterLockSlimUsage']/*"/>
        /// <include file="Documentation\CommonDocumentation.xml" path="CommonDocumentation/Remarks/Remark[@name='InfiniteTimeout']/*"/>
        /// </remarks>
        public LockWriter(ReaderWriterLock lockReference)
            : this(lockReference, Timeout.Infinite)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockWriter"/> class using the provided lock reference and timeout value.
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
        public LockWriter(ReaderWriterLock lockReference, Int32 timeout)
        {
            Cookie = default(LockCookie);

            LockReference = lockReference;

            // Check if there is already read lock on this thread
            // IMPORTANT: This is to avoid deadlocks on the current thread
            // See Remarks (caution) on ms-help://MS.VSCC.v90/MS.MSDNQTR.v90.en/fxref_mscorlib/html/9e17065a-0b8b-f611-1166-63231b0c6987.htm
            if (LockReference.IsReaderLockHeld)
            {
                try
                {
                    // There is already a read lock on the thread
                    // Upgrade the reader lock to a writer lock
                    Cookie = LockReference.UpgradeToWriterLock(timeout);
                }
                catch (ApplicationException ex)
                {
                    // Wrap the ReaderWriterLocks native timeout exception (ApplicationException) in an actual TimeoutException
                    throw new TimeoutException(ex.Message, ex);
                }
            }
            else
            {
                try
                {
                    // Acquire a write lock
                    LockReference.AcquireWriterLock(timeout);
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
        ///   If an upgraded lock was obtained, the write lock is downgraded to a read lock.
        /// </remarks>
        public void Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;

            if (LockReferenceSlim != null)
            {
                if (LockReferenceSlim.IsWriteLockHeld)
                {
                    LockReferenceSlim.ExitWriteLock();
                }
            }
            else
            {
                if (LockReference.IsWriterLockHeld)
                {
                    // Check if there was previously a reader lock
                    if (Cookie == default(LockCookie))
                    {
                        // Release the write lock
                        LockReference.ReleaseWriterLock();
                    }
                    else
                    {
                        LockCookie internalCookie = Cookie;

                        // Downgrade the writer lock back to a reader lock
                        LockReference.DowngradeFromWriterLock(ref internalCookie);
                    }
                }
            }
        }

        /// <summary>
        ///   Gets or sets the cookie.
        /// </summary>
        /// <value>
        ///   The cookie.
        /// </value>
        private LockCookie Cookie
        {
            get;
            set;
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this <see cref = "LockWriter" /> is disposed.
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
    }
}