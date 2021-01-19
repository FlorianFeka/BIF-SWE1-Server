using SocketTry.Http;
using System;
using System.Text.RegularExpressions;

namespace SocketTry.Utils
{
    internal static class Util
    {
        internal static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }
        
        internal static HttpMethod ProcessMethod(string methodString)
        {
            bool success = Enum.TryParse(methodString, out HttpMethod method);
            if (success)
            {
                return method;
            }
            throw new ArgumentException("Invalid or no HTTP method!");
        }

        internal static void FillEmptySpaceWith<T>(T[] arr, T fill, int from)
        {
            if (from >= arr.Length) throw new Exception($"{nameof(from)} is greater then the length of {nameof(arr)}");

            for(int i = from; i < arr.Length; i++)
            {
                arr[i] = fill;
            }
        }

        /// <summary>
        /// Returns path without parameters and other additions.
        /// </summary>
        /// <example> Example:
        /// <code>/test/123 -> /test/*</code>
        /// </example>
        /// <param name="path"></param>
        /// <returns>Pure path</returns>
        internal static string GetPurePath(string path)
        {
            string pathWithoutQueryParameters = GetPathWithoutQueryParameters(path);
            var pathParts = pathWithoutQueryParameters.Split("/");

            var pathParameterRegex = new Regex(@"({[_\w]+})");

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (pathParameterRegex.IsMatch(pathParts[i]))
                {
                    pathParts[i] = "*";
                }
            }

            return string.Join('/', pathParts);
        }

        internal static string GetPathWithoutQueryParameters(string path)
        {
            var startIndex = path.IndexOf("/");
            var parameterIndex = path.IndexOf("?");
            var lastIndex = parameterIndex;
            if (lastIndex < 0)
            {
                lastIndex = path.Length;
            }
            var pathWithoutQueryParameters = startIndex < 0 ? "" : path.Substring(startIndex, lastIndex - startIndex);
            return pathWithoutQueryParameters;
        }
    }
}
