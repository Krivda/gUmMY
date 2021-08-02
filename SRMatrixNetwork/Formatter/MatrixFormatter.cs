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
            new Tuple<string, Color> ( "icоn,persona", Color.BlueViolet),
            new Tuple<string, Color> ( "iсоn,persona", Color.BlueViolet),
            new Tuple<string, Color> ( "icоn,pеrsona", Color.BlueViolet),
            new Tuple<string, Color> ( "icоn,personа", Color.BlueViolet),
            new Tuple<string, Color> ( "icon,file", Color.Yellow),
            new Tuple<string, Color> ( "icon,API", Color.Orange),
            new Tuple<string, Color> ( "icon,livingPersona", Color.Blue),
            new Tuple<string, Color> ( "icon,portal", Color.CornflowerBlue),
            new Tuple<string, Color> ( "icon", Color.Lime),
            new Tuple<string, Color> ( "icon,deployment", Color.Lime),
            new Tuple<string, Color> ( "icon,mine", Color.Salmon),
            new Tuple<string, Color> ( "icon,fountain", Color.CadetBlue),
            new Tuple<string, Color> ( "icon,pool", Color.CadetBlue),
            new Tuple<string, Color> ( "persona", Color.Magenta),
            new Tuple<string, Color> ( "IC", Color.Red),
            new Tuple<string, Color> ( "node", Color.Cyan),
            new Tuple<string, Color> ( "datatrail,closed", Color.Red),
            new Tuple<string, Color> ( "datatrail,open", Color.DarkGreen),
            new Tuple<string, Color> ( "exception", Color.Red),

        };

        private static readonly Color SHAPE_PERFECT = Color.DarkGreen;
        private static readonly Color SHAPE_GOOD = Color.Lime;
        private static readonly Color SHAPE_WOUNDED = Color.DarkRed;
        private static readonly Color SHAPE_DEAD = Color.Red;

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
                MATRIX_FORMATTING_END, pos);

            while (formatInjection != null)
            {
                Token tagName = formatInjection.Content.Tokenize("", MATRIX_FORMATTING_MARKER_END);
                if (tagName != null)
                {
                    string newTagName = ProcessMarker(tagName.Content);

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

                formatInjection = result.Tokenize(MATRIX_FORMATTING_START,
                    MatrixFormatter.MATRIX_FORMATTING_END, pos);
            }

            return result;
        }

        public static string ApplyMatrixConditionFormat(int current, int max)
        {
            Color healthColor;

            if (current < 1)
            {
                healthColor = SHAPE_DEAD;
            }
            else if (current == max)
            {
                healthColor = SHAPE_PERFECT;
            }
            else if (current * 2 > max)
            {
                healthColor = SHAPE_GOOD;
            }
            else
            {
                healthColor = SHAPE_WOUNDED;
            }

            return $"[color,{healthColor.ToArgb()}: {current}mc]";
        }
    }
}