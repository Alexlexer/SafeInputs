using Xunit;
using SafeInputs;
using SafeInputs.Enums;
using SafeInputs.Policies;

public class SanitizerTests
{
    [Fact]
    public void Sanitize_PlainText_RemovesControlCharacters()
    {
        var input = "Hello\tWorld\n";
        var result = Sanitizer.Sanitize(input, SanitizationContext.PlainText);
        Assert.DoesNotContain('\t', result);
        Assert.DoesNotContain('\n', result);
    }

    [Fact]
    public void Sanitize_Html_RemovesDisallowedTags()
    {
        var input = "<b>Bold</b><script>alert(1)</script>";
        var result = Sanitizer.Sanitize(input, SanitizationContext.Html);
        Assert.Contains("Bold", result);
        Assert.DoesNotContain("script", result);
    }

    [Fact]
    public void Sanitize_Sql_EscapesSqlInjection()
    {
        var input = "O'Reilly; DROP TABLE Users;";
        var result = Sanitizer.Sanitize(input, SanitizationContext.Sql);
        Assert.DoesNotContain("DROP TABLE", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Sanitize_Url_EncodesSpecialCharacters()
    {
        var input = "https://example.com/?q=hello world";
        var result = Sanitizer.Sanitize(input, SanitizationContext.Url);
        Assert.Contains("%20", result);
    }

    [Fact]
    public void Sanitize_Attribute_EscapesQuotes()
    {
        var input = "\"onmouseover='alert(1)'\"";
        var result = Sanitizer.Sanitize(input, SanitizationContext.Attribute);
        Assert.DoesNotContain("\"", result);
        Assert.DoesNotContain("'", result);
    }
}
