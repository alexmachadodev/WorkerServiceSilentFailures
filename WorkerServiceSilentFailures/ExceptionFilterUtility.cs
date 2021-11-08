using System;

namespace WorkerServiceSilentFailures
{
    /// <summary>
    /// Essentially returning false from the exception filter prevents the stack from being unwound.
    /// Catching and rethrowing (even with throw; and not throw exception;) will unwind the stack to this point and then throw the exception again.
    /// Ref.: https://blog.stephencleary.com/2020/06/a-new-pattern-for-exception-logging.html
    /// </summary>
    public static class ExceptionFilterUtility
    {
        /// <summary>
        /// Another scenario is if the catch block actually handles the exception.
        /// Say, if we know there is an exception that is safe to ignore.
        /// In that case, use the True helper method so that the exception matches the catch block and the stack is unwound and the exception is handled there.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool True(Action action)
        {
            action();
            return true;
        }

        /// <summary>
        /// If the body of your catch is nothing more than throw.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool False(Action action)
        {
            action();
            return false;
        }
    }
}