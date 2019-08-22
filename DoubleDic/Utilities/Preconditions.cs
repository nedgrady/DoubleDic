using System;
using JetBrains.Annotations;

namespace DoubleDic.Utilities
{
    /// <summary>
    /// Helper static methods for argument/state validation.
    /// </summary>
    internal static class Preconditions
    {
        /// <summary>
        /// Returns the given argument after checking whether it's null. This is useful for putting
        /// nullity checks in parameters which are passed to base class constructors.
        /// </summary>
        [ContractAnnotation("argument:null => halt")]
        internal static T CheckNotNull<T>(T argument, [InvokerParameterName] string paramName) where T : class
        {
            if (argument is null)
                throw new ArgumentNullException(paramName);

            return argument;
        }
    }
}
