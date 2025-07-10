using SafeInputs.Contexts;
using SafeInputs.Policies;

public static class Sanitizer
{
    public static string Sanitize(string input, SanitizationContext context, object? options = null)
    {
        return context switch
        {
            SanitizationContext.Html => HtmlSanitizer.Sanitize(input, options as HtmlSanitizerPolicy),
            SanitizationContext.PlainText => PlainTextSanitizer.Sanitize(input),
            SanitizationContext.Sql => SqlSanitizer.Sanitize(input),
            SanitizationContext.Url => UrlSanitizer.Sanitize(input),
            SanitizationContext.Attribute => AttributeSanitizer.Sanitize(input),
            _ => input
        };
    }
}
