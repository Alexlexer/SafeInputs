using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Contexts
{
    public static class PlainTextSanitizer
    {
        public static string Sanitize(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var sanitized = new StringBuilder();
            foreach (char c in input)
            {
                // Remove control characters except line breaks
                if (!char.IsControl(c) || c == '\n' || c == '\r')
                {
                    sanitized.Append(c);
                }
            }

            return sanitized.ToString().Trim();
        }
    }
}