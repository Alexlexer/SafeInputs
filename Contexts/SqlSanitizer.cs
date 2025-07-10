using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Contexts
{
    public static class SqlSanitizer
    {
        public static string Sanitize(string? input)
        {
            if (input == null) return string.Empty;

            var builder = new StringBuilder();

            foreach (char c in input)
            {
                // Basic SQL escaping: duplicate single quotes
                if (c == '\'') builder.Append("''");
                else builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
