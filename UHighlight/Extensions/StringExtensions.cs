using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.Extensions
{
    public static class StringExtensions
    {
        public static bool TryParseVolumeShape(this string value, out EVolumeShape shape)
        {
            value = value.ToTitleCase() switch
            {
                _ => value
            };

            return Enum.TryParse(value, out shape);
        }

        public static bool TryParseVolumeMaterial(this string value, out EVolumeMaterial material)
        {
            value = value.ToTitleCase() switch
            {
                "T" => "Transparent",
                "S" => "Solid",
                _ => value
            };

            return Enum.TryParse(value, out material);
        }

        public static bool TryParseVolumeColor(this string value, out EVolumeColor color)
        {
            value = value.ToTitleCase() switch
            {
                "R" => "Red",
                "Re" => "Red",
                "G" => "Green",
                "Gr" => "Green",
                "B" => "Blue",
                "Bl" => "Blue",
                "C" => "Cyan",
                "Cy" => "Cyan",
                "M" => "Magenta",
                "Ma" => "Magenta",
                "L" => "Lime",
                "Li" => "Lime",
                "Go" => "Gold",
                "S" => "Silver",
                "Si" => "Silver",
                "Co" => "Copper",
                "P" => "Pink",
                "Pi" => "Pink",
                _ => value
            };

            return Enum.TryParse(value, out color);
        }

        private static string ToTitleCase(this string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
        }
    }
}
