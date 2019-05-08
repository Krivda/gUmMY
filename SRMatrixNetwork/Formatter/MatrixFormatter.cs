using System;
using System.Collections.Generic;
using System.Drawing;
using me.krivda.utils;

namespace SRMatrixNetwork.Formatter
{
    public class MatrixFormatter
    {
        public const string MATRIX_FORMATTING_MARKER_END = ": ";
        public const string MATRIX_FORMATTING_START = "[";
        public const string MATRIX_FORMATTING_END = "]";

        private static readonly List<Tuple<string, Color>> colors = new List<Tuple<string, Color>>()
        {
            new Tuple<string, Color> ( "icon,persona", Color.BlueViolet),
            new Tuple<string, Color> ( "icon,file", Color.Green),
            new Tuple<string, Color> ( "icon,API", Color.Khaki),
            new Tuple<string, Color> ( "icon,livingPersona", Color.Blue),
            new Tuple<string, Color> ( "icon", Color.Yellow),
            new Tuple<string, Color> ( "persona", Color.Aqua),
            new Tuple<string, Color> ( "IC", Color.Red),
        };

        public static string ProcessMarker(string marker)
        {
            if (string.IsNullOrEmpty(marker)) return marker;

            string compare = marker.Trim().ToLowerInvariant();
            foreach (var color in colors)
            {
                if (color.Item1.ToLowerInvariant().Equals(compare))
                {
                    return $"color,{color.Item2.ToArgb()}";
                }   
            }

            return marker;
        }

        public static string ApplyMatrixFormatting(string message)
        {
            string result = message;

            int pos = 0;
            Token formatInjection = result.Tokenize(MATRIX_FORMATTING_START,
                MatrixFormatter.MATRIX_FORMATTING_END, pos);

            while (formatInjection != null)
            {
                Token tagName = formatInjection.Content.Tokenize("", MATRIX_FORMATTING_MARKER_END);
                if (tagName != null)
                {
                    string newTagName = MatrixFormatter.ProcessMarker(tagName.Content);

                    if (!string.Equals(newTagName, tagName.Content))
                    {
                        string newFormattingContent = formatInjection.Content.ReplaceBetween(tagName.Start, tagName.End - MATRIX_FORMATTING_MARKER_END.Length, newTagName);
                        result = result.ReplaceBetween(formatInjection.Start + MATRIX_FORMATTING_START.Length,
                            formatInjection.End - MATRIX_FORMATTING_END.Length, newFormattingContent);
                    }
                    pos = formatInjection.End + (newTagName.Length - tagName.Content.Length);
                }
                else
                {
                    pos = formatInjection.End;
                }

                formatInjection = result.Tokenize(MatrixFormatter.MATRIX_FORMATTING_START,
                    MatrixFormatter.MATRIX_FORMATTING_END, pos);
            }

            return result;
        }   
    }
}