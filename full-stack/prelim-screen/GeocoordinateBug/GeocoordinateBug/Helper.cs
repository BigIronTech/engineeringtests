using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocoordinateBug
{
    public class Helper
    {
        public enum FormatLatLongOptions 
        {
            Default,
            FixedPoint,
            Dms
        }
        public static string LatLong(string text, FormatLatLongOptions options = FormatLatLongOptions.Default)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var sign = ' ';

            // NOTE: We are pretty flexible here. We just try to eat everything and make some kind'a sense out of it.
            // Typically, we are looking for one of the following:
            //   [+/-]nnnnnnnnn
            //   [+/-]nnn.nnnnnn
            var foundDecimal = false;
            var integerText = new StringBuilder();
            var decimalText = new StringBuilder();
            foreach (var ch in text)
            {
                if (char.IsDigit(ch))
                {
                    if (foundDecimal)
                    {
                        decimalText.Append(ch);
                    }
                    else
                    {
                        if (integerText.Length < 3)
                        {
                            integerText.Append(ch);
                        }
                        else
                        {
                            if (decimalText.Length < 6)
                                decimalText.Append(ch);
                        }
                    }
                }
                else
                {
                    switch (ch)
                    {
                        case '+':
                        case '-':
                            sign = ch;
                            break;
                        case '.':
                            foundDecimal = true;
                            break;
                    }
                }

                if (integerText.Length >= 3 && decimalText.Length >= 6)
                    break;
                if (integerText.Length >= 10 || decimalText.Length >= 10)
                    break;
            }

            long integerValue = 0;
            if (integerText.Length > 0)
                integerValue = long.Parse(integerText.ToString());
            integerValue = Math.Min(integerValue, 180);
            if (sign == ' ')
                // this is almost certainly a longitude in the US but without a sign.
                // We will assume negative.
                if (integerValue >= 90)
                    sign = '-';
            var formatted = new StringBuilder();
            if (sign != ' ')
                formatted.Append(sign);
            if (options == FormatLatLongOptions.Dms)
            {
                formatted.Append(integerValue);
                formatted.Append('°');
                var decimalFraction = double.Parse("." + decimalText);
                var minutes = (int)(decimalFraction * 60.0);
                var seconds = decimalFraction * 60.0 * 60.0 - minutes * 60.0;
                formatted.Append($" {minutes}\' {seconds:f4}\"");
            }
            else
            {
                long decimalValue = 0;
                if (decimalText.Length > 0)
                    decimalValue = long.Parse(decimalText.ToString());
                if (options == FormatLatLongOptions.FixedPoint)
                {
                    formatted.Append($"{integerValue:d03}");
                    formatted.Append($"{decimalValue:d06}");
                }
                else
                {
                    formatted.Append(integerValue);
                    formatted.Append('.');
                    formatted.Append($"{decimalValue}");
                }
            }

            text = formatted.ToString();
            if (text.Length > 10)
                text = text.Substring(0, 10);
            return text;
        }
    }
}
