using SafeInputs.Contexts;
using SafeInputs.Enums;
using SafeInputs.Policies;

public static class Sanitizer
{
    public static string Sanitize(string input, SanitizationContext context, object? options = null)
    {
        return context switch
        {
            SanitizationContext.Html => new HtmlSanitizer().Sanitize(input, options as HtmlSanitizerPolicy),
            SanitizationContext.PlainText => new PlainTextSanitizer().Sanitize(input),
            SanitizationContext.Sql => new SqlSanitizer().Sanitize(input),
            SanitizationContext.Url => new UrlSanitizer().Sanitize(input),
            SanitizationContext.Attribute => new AttributeSanitizer().Sanitize(input),
            _ => input
        };
    }
}
