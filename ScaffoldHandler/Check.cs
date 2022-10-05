using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScaffoldHandler
{

    [DebuggerStepThrough]
    internal class Check
    {
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
            {
                NotEmpty(parameterName, "parameterName");
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);
            if (value.Count == 0)
            {
                NotEmpty(parameterName, "parameterName");
                throw new ArgumentException(AbstractionsStrings.CollectionArgumentIsEmpty(parameterName));
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName)
        {
            Exception ex = null;
            if (value == null)
            {
                ex = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                ex = new ArgumentException(AbstractionsStrings.ArgumentIsEmpty(parameterName));
            }

            if (ex != null)
            {
                NotEmpty(parameterName, "parameterName");
                throw ex;
            }

            return value;
        }

        public static string NullButNotEmpty(string value, string parameterName)
        {
            if (value != null && value.Length == 0)
            {
                NotEmpty(parameterName, "parameterName");
                throw new ArgumentException(AbstractionsStrings.ArgumentIsEmpty(parameterName));
            }

            return value;
        }

        public static IReadOnlyList<T> HasNoNulls<T>(IReadOnlyList<T> value, string parameterName) where T : class
        {
            NotNull(value, parameterName);
            if (value.Any((T e) => e == null))
            {
                NotEmpty(parameterName, "parameterName");
                throw new ArgumentException(parameterName);
            }

            return value;
        }

        [Conditional("DEBUG")]
        public static void DebugAssert([DoesNotReturnIf(false)] bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception("Check.DebugAssert failed: " + message);
            }
        }
    }
}
