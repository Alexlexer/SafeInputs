using SafeInputs.Enums;
using SafeInputs.Interfaces;
using SafeInputs.Policies;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SafeInputs.Contexts
{
    public class HtmlSanitizer : ISanitizer, ISanitizer<HtmlSanitizerPolicy>, IContextSanitizer
    {
        public SanitizationContext Context => SanitizationContext.Html;

        // Precompiled regexes for performance
        private static readonly Regex CommentRegex = new Regex(@"<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex DangerousTagsRegex = new Regex(
            @"<(script|iframe|style|object|embed|form)\b[^>]*>.*?</\1\s*>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public string Sanitize(string input) => Sanitize(input, HtmlSanitizerPolicy.Default());

        public string Sanitize(string input, HtmlSanitizerPolicy? policy)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            policy ??= HtmlSanitizerPolicy.Default();

            // 1) Remove comments quickly
            input = CommentRegex.Replace(input, string.Empty);

            // 2) Remove dangerous container tags + content (script, iframe, style, ...)
            input = DangerousTagsRegex.Replace(input, string.Empty);

            // 3) Linear parse: keep allowed tags and filter attributes (whitelist-only)
            var sb = new StringBuilder(input.Length);
            int pos = 0, len = input.Length;

            while (pos < len)
            {
                char c = input[pos];
                if (c != '<')
                {
                    sb.Append(c);
                    pos++;
                    continue;
                }

                // c == '<'
                int endTag = input.IndexOf('>', pos);
                if (endTag == -1)
                {
                    // malformed tag: append '<' as text and continue
                    sb.Append('<');
                    pos++;
                    continue;
                }

                // inner content between '<' and '>'
                int innerStart = pos + 1;
                int innerLen = endTag - innerStart;
                if (innerLen <= 0)
                {
                    // cases like "<>"
                    pos = endTag + 1;
                    continue;
                }

                string tagContent = input.Substring(innerStart, innerLen).Trim();
                if (string.IsNullOrEmpty(tagContent))
                {
                    pos = endTag + 1;
                    continue;
                }

                bool isEndTag = tagContent.StartsWith("/");
                string rawNameAndAttrs = isEndTag ? (tagContent.Length > 1 ? tagContent.Substring(1).TrimStart() : string.Empty) : tagContent;
                if (string.IsNullOrEmpty(rawNameAndAttrs))
                {
                    pos = endTag + 1;
                    continue;
                }

                // find tag name up to whitespace (safe without Split[0])
                int wsIndex = rawNameAndAttrs.IndexOfAny(new[] { ' ', '\t', '\r', '\n' });
                string tagName = wsIndex >= 0 ? rawNameAndAttrs.Substring(0, wsIndex) : rawNameAndAttrs;
                if (string.IsNullOrEmpty(tagName))
                {
                    pos = endTag + 1;
                    continue;
                }
                tagName = tagName.ToLowerInvariant();

                // Only allow tags explicitly in policy
                if (policy.AllowedTags.Contains(tagName))
                {
                    sb.Append('<');
                    if (isEndTag) sb.Append('/');
                    sb.Append(tagName);

                    // attributes only for opening tags
                    if (!isEndTag && wsIndex >= 0 && wsIndex + 1 < rawNameAndAttrs.Length)
                    {
                        string attrsPart = rawNameAndAttrs.Substring(wsIndex + 1).Trim();
                        if (!string.IsNullOrEmpty(attrsPart))
                        {
                            string filtered = FilterAttributesSpan(attrsPart, tagName, policy);
                            if (!string.IsNullOrEmpty(filtered))
                            {
                                sb.Append(' ').Append(filtered);
                            }
                        }
                    }

                    // self-closing detection: if original tag had trailing '/' before '>', keep it
                    bool selfClosing = (rawNameAndAttrs.EndsWith("/") || (endTag - 1 >= 0 && input[endTag - 1] == '/'));
                    if (selfClosing && !isEndTag)
                    {
                        // normalize to "<tag ... />"
                        if (sb[sb.Length - 1] != '/') sb.Append(" /");
                    }

                    sb.Append('>');
                }
                // else: tag not in allowlist => drop tag markers, keep content outside tags

                pos = endTag + 1;
            }

            return sb.ToString();
        }

        // Robust attribute parser using Span to avoid Regex over-attribution and support quoted values with spaces.
        // Returns attributes formatted like: name='value' name2='v2'  (single-quoted normalized)
        private string FilterAttributesSpan(string attrText, string tagName, HtmlSanitizerPolicy policy)
        {
            var sb = new StringBuilder(attrText.Length);
            ReadOnlySpan<char> span = attrText.AsSpan();
            int i = 0, n = span.Length;

            while (i < n)
            {
                // skip whitespace
                while (i < n && char.IsWhiteSpace(span[i])) i++;
                if (i >= n) break;

                // read attribute name
                int nameStart = i;
                while (i < n && !char.IsWhiteSpace(span[i]) && span[i] != '=') i++;
                if (i <= nameStart) break;
                var attrName = span.Slice(nameStart, i - nameStart).ToString().ToLowerInvariant();

                // skip whitespace
                while (i < n && char.IsWhiteSpace(span[i])) i++;

                // read optional value
                string attrValue = string.Empty;
                if (i < n && span[i] == '=')
                {
                    i++; // skip '='
                    while (i < n && char.IsWhiteSpace(span[i])) i++;

                    if (i < n && (span[i] == '"' || span[i] == '\''))
                    {
                        char quote = span[i++];
                        int vs = i;
                        while (i < n && span[i] != quote) i++;
                        int lenVal = Math.Max(0, i - vs);
                        attrValue = span.Slice(vs, lenVal).ToString();
                        if (i < n && span[i] == quote) i++; // skip closing quote
                    }
                    else
                    {
                        int vs = i;
                        while (i < n && !char.IsWhiteSpace(span[i])) i++;
                        int lenVal = Math.Max(0, i - vs);
                        attrValue = span.Slice(vs, lenVal).ToString();
                    }
                }

                // decide to keep: whitelist-only via policy
                if (policy.IsAttributeAllowed(tagName, attrName))
                {
                    if (sb.Length > 0) sb.Append(' ');
                    sb.Append(attrName);
                    if (!string.IsNullOrEmpty(attrValue))
                    {
                        sb.Append("='").Append(attrValue).Append('\'');
                    }
                }
                // otherwise skip attribute
            }

            return sb.ToString().TrimEnd();
        }

        // Explicit interface implementations
        string ISanitizer<HtmlSanitizerPolicy>.Sanitize(string input, HtmlSanitizerPolicy options)
            => Sanitize(input, options);

        string IContextSanitizer.Sanitize(string input, object? options)
            => Sanitize(input, options as HtmlSanitizerPolicy);
    }
}
