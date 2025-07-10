using HtmlAgilityPack;
using SafeInputs.Contexts;
using SafeInputs.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Extensions
{
    public class FluentSanitizer
    {
        private string _input;
        private SanitizationContext _context;
        private HtmlSanitizerPolicy? _htmlPolicy;

        public static FluentSanitizer Sanitize(string input)
            => new() { _input = input };

        public FluentSanitizer ForHtml(HtmlSanitizerPolicy? policy = null)
        {
            _context = SanitizationContext.Html;
            _htmlPolicy = policy;
            return this;
        }

        public string Apply()
            => Sanitizer.Sanitize(_input, _context, _htmlPolicy);
    }
}
