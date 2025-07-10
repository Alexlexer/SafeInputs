using HtmlAgilityPack;
using SafeInputs.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Contexts
{
    public static class HtmlSanitizer
    {
        public static string Sanitize(string html, HtmlSanitizerPolicy? policy = null)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            policy ??= HtmlSanitizerPolicy.Default();

            CleanNodes(doc.DocumentNode, policy);

            return doc.DocumentNode.InnerHtml;
        }

        private static void CleanNodes(HtmlNode node, HtmlSanitizerPolicy policy)
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
