using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace API.UtilitiesAndExtensions
{
    public static class StringHelper
    {
        public static string JoinAdvanced(this String theString, string prefixSeparator, string postfixOperator, string[] strings)
        {
            var sb = new StringBuilder();

            foreach (string s in strings)
            {
                sb.Append(prefixSeparator + s + postfixOperator);
            }

            return sb.ToString();
        }

        public static bool HasParenParity(this String theString)
        {
            var openParenCount = theString.Where(n => n == '(').Count();
            var closedParenCount = theString.Where(n => n == ')').Count();

            return openParenCount == closedParenCount;
        }

        public static int IndexOfNth(this string input,
                                     string value, int startIndex, int nth)
        {
            if (nth < 1)
                throw new NotSupportedException("Param 'nth' must be greater than 0!");
            if (nth == 1)
                return input.IndexOf(value, startIndex);
            var idx = input.IndexOf(value, startIndex);
            if (idx == -1)
                return -1;
            return input.IndexOfNth(value, idx + 1, --nth);
        }
    }

    static public class Extensions
    {
        public static void SetNewSize<T>(this Stack<T> theStack, int newSize, bool isStackObjectIDisposable = false)
        {
            int startCount = theStack.Count;
            if (startCount == 0)
            {
                return;
            }

            if (isStackObjectIDisposable)
            {
                for (int i = 0; i < startCount - newSize; i++)
                {
                    var disp = theStack.Peek() as IDisposable;
                    theStack.Pop();
                    if (disp != null)
                    {
                        disp.Dispose();
                    }
                }
            }
            else
            {
                for (int i = 0; i < startCount - newSize; i++)
                {
                    theStack.Pop();
                }
            }
        }
    }
}
