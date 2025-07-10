using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Contexts
{
    public static class AttributeSanitizer
    {
        public static string Sanitize(string? input)
        {
            if (input == null) return string.Empty;

            var builder = new StringBuilder();

            foreach (char c in input)
            {
                switch (c)
                {
                    case '"': builder.Append("&quot;"); break;
                    case '\'': builder.Append("&#39;"); break;
                    case '<': builder.Append("&lt;"); break;
                    case '>': builder.Append("&gt;"); break;
                    case '&': builder.Append("&amp;"); break;
                    default: builder.Append(c); break;
                }
            }

            return builder.ToString();
        }
    }
}
