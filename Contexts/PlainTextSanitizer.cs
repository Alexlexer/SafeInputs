using SafeInputs.Enums;
using SafeInputs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Contexts
{
    public class PlainTextSanitizer : ISanitizer, IContextSanitizer
    {
        public SanitizationContext Context => SanitizationContext.PlainText;

        public string Sanitize(string? input)
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

        string IContextSanitizer.Sanitize(string input, object? options)
            => Sanitize(input);
    }
}