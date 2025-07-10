using SafeInputs.Interfaces;
using SafeInputs.Enums;
namespace SafeInputs.Contexts
{
    public class UrlSanitizer : ISanitizer, IContextSanitizer
    {
        public SanitizationContext Context => SanitizationContext.Url;
        public string Sanitize(string? input)
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

        string IContextSanitizer.Sanitize(string input, object? options)
            => Sanitize(input);
    }
}
