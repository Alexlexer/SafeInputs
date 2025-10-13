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

        // Optional regexes for speed / pre-cleaning (kept compiled)
        private static readonly Regex CommentRegex = new Regex(@"<!--.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex DangerousTagsRegex = new Regex(
            @"<(script|iframe|style|object|embed|form)\b[^>]*>.*?</\1\s*>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public string Sanitize(string input) => Sanitize(input, HtmlSanitizerPolicy.Default());

        public string Sanitize(string input, HtmlSanitizerPolicy? policy)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            policy ??= HtmlSanitizerPolicy.Default();

            // 1) Remove comments (safe to do first)
            input = CommentRegex.Replace(input, string.Empty);

            // 2) Remove known dangerous containers with their content
            input = DangerousTagsRegex.Replace(input, string.Empty);

            var sb = new StringBuilder(input.Length);
            int pos = 0;
            int len = input.Length;

            while (pos < len)
            {
                if (input[pos] == '<')
                {
                    int endTag = input.IndexOf('>', pos);
                    if (endTag == -1)
                    {
                        // malformed tag — append char and advance
                        sb.Append(input[pos]);
                        pos++;
                        continue;
                    }

                    // extract inner of < ... >
                    int innerStart = pos + 1;
                    int innerLen = endTag - innerStart;
                    if (innerLen <= 0)
                    {
                        // e.g. "<>" or similar — skip tag markers
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
                    string rawTagName = isEndTag ? (tagContent.Length > 1 ? tagContent.Substring(1).TrimStart() : string.Empty) : tagContent;
                    if (string.IsNullOrEmpty(rawTagName))
                    {
                        pos = endTag + 1;
                        continue;
                    }

                    // find tag name up to first whitespace (if any)
                    string tagName;
                    int firstSpace = rawTagName.IndexOfAny(new char[] { ' ', '\t', '\r', '\n' });
                    if (firstSpace >= 0)
                        tagName = rawTagName.Substring(0, firstSpace);
                    else
                        tagName = rawTagName;

                    tagName = tagName.ToLowerInvariant();

                    // ONLY allow tags from allowlist
                    if (policy.AllowedTags.Contains(tagName))
                    {
                        // reconstruct tag
                        sb.Append('<');
                        if (isEndTag) sb.Append('/');
                        sb.Append(tagName);

                        // attributes only for opening tags
                        if (!isEndTag)
                        {
                            string attrsPart = string.Empty;
                            if (firstSpace >= 0 && firstSpace + 1 < rawTagName.Length)
                                attrsPart = rawTagName.Substring(firstSpace + 1).Trim();

                            if (!string.IsNullOrEmpty(attrsPart))
                            {
                                string filtered = FilterAttributesSpan(attrsPart, tagName, policy);
                                if (!string.IsNullOrEmpty(filtered))
                                {
                                    sb.Append(' ').Append(filtered);
                                }
                            }
                        }

                        sb.Append('>');
                    }
                    // else: tag not allowed => drop the tag markers (but keep whatever text outside tags)
                    pos = endTag + 1;
                }
                else
                {
                    sb.Append(input[pos]);
                    pos++;
                }
            }

            return sb.ToString();
        }

        // Robust span-based attribute parser — returns "name='value' name2='v2'" or empty
        private string FilterAttributesSpan(string attrText, string tagName, HtmlSanitizerPolicy policy)
        {
            var sb = new StringBuilder(attrText.Length);
            ReadOnlySpan<char> span = attrText.AsSpan();
            int i = 0;
            int n = span.Length;

            while (i < n)
            {
                // skip whitespace
                while (i < n && char.IsWhiteSpace(span[i])) i++;
                if (i >= n) break;

                // attr name
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
                        int valStart = i;
                        while (i < n && span[i] != quote) i++;
                        int valLen = Math.Max(0, i - valStart);
                        attrValue = span.Slice(valStart, valLen).ToString();
                        if (i < n && span[i] == quote) i++; // skip closing quote
                    }
                    else
                    {
                        int valStart = i;
                        while (i < n && !char.IsWhiteSpace(span[i])) i++;
                        int valLen = Math.Max(0, i - valStart);
                        attrValue = span.Slice(valStart, valLen).ToString();
                    }
                }

                // decide whether to keep attribute (whitelist-only)
                if (policy.IsAttributeAllowed(tagName, attrName))
                {
                    if (sb.Length > 0) sb.Append(' ');
                    sb.Append(attrName);
                    if (!string.IsNullOrEmpty(attrValue))
                    {
                        sb.Append("='").Append(attrValue).Append('\'');
                    }
                }
                // else skip attribute
            }

            return sb.ToString().TrimEnd();
        }

        // explicit interface implementations
        string ISanitizer<HtmlSanitizerPolicy>.Sanitize(string input, HtmlSanitizerPolicy options)
            => Sanitize(input, options);

        string IContextSanitizer.Sanitize(string input, object? options)
            => Sanitize(input, options as HtmlSanitizerPolicy);
    }
}
