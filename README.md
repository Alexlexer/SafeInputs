# SafeInputs Library

SafeInputs is a .NET library for sanitizing user input across multiple contexts to prevent injection attacks, remove unsafe content, and ensure safe handling of text, HTML, SQL, URLs, and attributes.

## Features

- **PlainText Sanitization**: Removes control characters, unwanted whitespace, and ensures safe plain text input.
- **HTML Sanitization**: Cleans HTML by allowing only safe tags and attributes. Blocks scripts, iframes, and other dangerous content.
- **SQL Sanitization**: Escapes single quotes to prevent SQL injection attacks.
- **URL Sanitization**: Encodes unsafe characters in URLs.
- **Attribute Sanitization**: Escapes quotes and dangerous characters in HTML attributes.

## Installation

You can include the library in your project via your preferred method (NuGet package or project reference).

```bash
# If published as a NuGet package
dotnet add package SafeInputs
```

Or add the project directly to your solution and reference it.

## Usage

```csharp
using SafeInputs;
using SafeInputs.Enums;
using SafeInputs.Policies;

// Sanitize plain text
string inputText = "Hello\tWorld\n";
string safeText = Sanitizer.Sanitize(inputText, SanitizationContext.PlainText);

// Sanitize HTML
string htmlInput = "<b>Bold</b><script>alert(1)</script>";
string safeHtml = Sanitizer.Sanitize(htmlInput, SanitizationContext.Html);

// Sanitize SQL
string sqlInput = "O'Reilly; DROP TABLE Users;";
string safeSql = Sanitizer.Sanitize(sqlInput, SanitizationContext.Sql);

// Sanitize URL
string urlInput = "https://example.com/?q=hello world";
string safeUrl = Sanitizer.Sanitize(urlInput, SanitizationContext.Url);

// Sanitize attribute
string attrInput = "\"onmouseover='alert(1)'\"";
string safeAttr = Sanitizer.Sanitize(attrInput, SanitizationContext.Attribute);
```

## Extensibility

- You can define custom policies for HTML sanitization by creating a `HtmlSanitizerPolicy` instance and configuring allowed tags and attributes.
- Implement `ISanitizer` or `IContextSanitizer` interfaces to add your own sanitization logic.

## Testing

The library includes comprehensive unit tests using [xUnit](https://xunit.net/). Tests cover:

- PlainText, HTML, SQL, URL, and attribute sanitization
- Handling of malformed and complex input
- Ensuring dangerous content is removed while preserving safe content

```bash
dotnet test
```

## License

This project is licensed under the MIT License. See the LICENSE file for details.
