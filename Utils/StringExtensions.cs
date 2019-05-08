using System;
using System.Collections.Generic;

namespace me.krivda.utils
{
    public static class StringExtensions
    {
        //// str - the source string
        //// index- the start location to replace at (0-based)
        //// length - the number of characters to be removed before inserting
        //// replace - the string that is replacing characters
        public static string ReplaceAt(this string str, int index, int length, string replace)
        {
            str = str.PadRight(index);
            string result = str.Remove(index, Math.Min(length, str.Length - index)).Insert(index, replace);
            return result;
        }

        //// str - the source string
        //// index- the start location to replace at (0-based)
        //// length - the number of characters to be removed before inserting
        //// replace - the string that is replacing characters
        public static string ReplaceBetween(this string str, int start, int end, string replace)
        {
            return ReplaceAt(str, start, end-start, replace);
        }


        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        public static string Slice(this string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }

        /// <summary>
        /// takes a substring between two anchor strings (or the end of the string if that anchor is null)
        /// </summary>
        /// <param name="this">a string</param>
        /// <param name="from">an optional string to search after</param>
        /// <param name="until">an optional string to search before</param>
        /// <param name="start">an optional start search position</param>
        /// <param name="comparison">an optional comparison for the search</param>
        /// <returns>a substring based on the search</returns>
        public static string Substring(this string @this, string from = null, string until = null, int start=0, StringComparison comparison = StringComparison.InvariantCulture)
        {
            if (@this == null){throw new ArgumentNullException(nameof(@this));}
            
            var fromLength = (from ?? string.Empty).Length;
            var startIndex = !string.IsNullOrEmpty(from)
                ? @this.IndexOf(from, start, comparison) + fromLength
                : 0;

            if (startIndex < fromLength) { throw new ArgumentException("from: Failed to find an instance of the first anchor"); }

            var endIndex = !string.IsNullOrEmpty(until)
                ? @this.IndexOf(until, startIndex, comparison)
                : @this.Length;

            if (endIndex < 0) { throw new ArgumentException("until: Failed to find an instance of the last anchor"); }

            var subString = @this.Substring(startIndex, endIndex - startIndex);
            return subString;
        }

        public static Token Tokenize(this string @this, string from = null, string until = null, int start = 0,
            StringComparison comparison = StringComparison.InvariantCulture)
        {
            Token result = null;

            if (@this == null) { throw new ArgumentNullException(nameof(@this)); }

            var fromLength = (from ?? string.Empty).Length;
            var untilLength = (until ?? string.Empty).Length;
            var startIndex = !string.IsNullOrEmpty(from)
                ? @this.IndexOf(from, start, comparison) + fromLength
                : 0;

            if (startIndex < fromLength){return null;}

            var endIndex = !string.IsNullOrEmpty(until)
                ? @this.IndexOf(until, startIndex, comparison)
                : @this.Length;

            if (endIndex < 0) { return null; }

            var subString = @this.Substring(startIndex, endIndex - startIndex);

            int startToken = startIndex = !string.IsNullOrEmpty(from)
                ? startIndex - fromLength
                : 0;

            result =new Token(startToken, endIndex + untilLength, subString);

            return result;
        }

        public static List<Token> TokenizeAll(this string @this, string from = null, string until = null, int start = 0,
            StringComparison comparison = StringComparison.InvariantCulture)
        {

            var tokens = new List<Token>();

            int pos = start;
            Token token = @this.Tokenize(from, until, pos);
            while (token!=null)
            {
                pos = token.End;
                tokens.Add(token);
                token = @this.Tokenize(from, until, pos);
            }

            return tokens;
        }


    }

    public class Token
    {
        public int Start { get; }
        public int End { get; }
        public string Content { get; }

        public Token(int start, int end, string content)
        {
            Start = start;
            End = end;
            Content = content;
        }
    }
}