using SafeInputs.Enums;
using SafeInputs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Services
{
    public class SanitizerDispatcher
    {
        private readonly IEnumerable<IContextSanitizer> _sanitizers;

        public SanitizerDispatcher(IEnumerable<IContextSanitizer> sanitizers)
        {
            _sanitizers = sanitizers;
        }

        public string Sanitize(string input, SanitizationContext context, object? options = null)
        {
            var sanitizer = _sanitizers.FirstOrDefault(s => s.Context == context);
            return sanitizer?.Sanitize(input, options) ?? input;
        }
    }
}
