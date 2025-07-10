using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Contexts
{
    public static class UrlSanitizer
    {
        public static string Sanitize(string? input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            try
            {
                return Uri.EscapeDataString(input);
            }
            catch
            {
                return string.Empty; // fallback
            }
        }
    }
}
