using HtmlAgilityPack;
using SafeInputs.Enums;
using SafeInputs.Interfaces;
using SafeInputs.Policies;

namespace SafeInputs.Contexts
{
    public class HtmlSanitizer : ISanitizer, ISanitizer<HtmlSanitizerPolicy>, IContextSanitizer
    {
        public SanitizationContext Context => SanitizationContext.Html;

        public string Sanitize(string input)
        {
            return Sanitize(input, HtmlSanitizerPolicy.Default());
        }

        public string Sanitize(string input, HtmlSanitizerPolicy? policy)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(input);

            policy ??= HtmlSanitizerPolicy.Default();

            CleanNodes(doc.DocumentNode, policy);

            return doc.DocumentNode.InnerHtml;
        }

        // Explicit interface implementation for ISanitizer<HtmlSanitizerPolicy>
        string ISanitizer<HtmlSanitizerPolicy>.Sanitize(string input, HtmlSanitizerPolicy options)
            => Sanitize(input, options);

        string IContextSanitizer.Sanitize(string input, object? options)
            => Sanitize(input, options as HtmlSanitizerPolicy);

        private void CleanNodes(HtmlNode node, HtmlSanitizerPolicy policy)
        {
            foreach (var child in node.ChildNodes.ToList())
            {
                if (!policy.AllowedTags.Contains(child.Name))
                {
                    child.Remove();
                }
                else
                {
                    var attrsToRemove = child.Attributes
                        .Where(attr => !policy.IsAttributeAllowed(child.Name, attr.Name))
                        .ToList();

                    foreach (var attr in attrsToRemove)
                        child.Attributes.Remove(attr);

                    CleanNodes(child, policy);
                }
            }
        }
    }
}
