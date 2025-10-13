using SafeInputs.Enums;
using SafeInputs.Policies;

namespace SafeInputs.Extensions
{
    public class FluentSanitizer
    {
        private readonly string _input;
        private SanitizationContext _context;
        private HtmlSanitizerPolicy? _htmlPolicy;

        private FluentSanitizer(string input)
        {
            _input = input;
        }
        public static FluentSanitizer Sanitize(string input)
        => new FluentSanitizer(input);

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
