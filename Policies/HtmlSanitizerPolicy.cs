using System.Collections.Generic;

namespace SafeInputs.Policies
{
    public class HtmlSanitizerPolicy
    {
        /// <summary>
        /// List of allowed HTML tags (e.g. b, p, a).
        /// </summary>
        public HashSet<string> AllowedTags { get; set; } = new()
        {
            "b", "i", "u", "p", "strong", "em", "ul", "ol", "li", "br", "span", "div", "a"
        };

        /// <summary>
        /// Denied tags that should be removed regardless of context.
        /// </summary>
        public HashSet<string> BlockedTags { get; set; } = new()
        {
            "script", "iframe", "object", "embed", "form", "style"
        };

        /// <summary>
        /// Global allowed attributes for any tag.
        /// </summary>
        public HashSet<string> GlobalAllowedAttributes { get; set; } = new()
        {
            "class", "style", "id", "title", "href", "src", "alt", "target"
        };

        /// <summary>
        /// Per-tag attribute allowlist.
        /// </summary>
        public Dictionary<string, HashSet<string>> AllowedAttributes { get; set; } = new();

        /// <summary>
        /// Checks whether an attribute is allowed on a tag.
        /// </summary>
        public bool IsAttributeAllowed(string tag, string attr)
        {
            if (AllowedAttributes.TryGetValue(tag, out var allowedForTag))
            {
                return allowedForTag.Contains(attr);
            }

            return GlobalAllowedAttributes.Contains(attr);
        }

        /// <summary>
        /// Creates a reusable default policy.
        /// </summary>
        public static HtmlSanitizerPolicy Default()
        {
            var policy = new HtmlSanitizerPolicy();

            // Example: restrict <a> tag to href/target only
            policy.AllowedAttributes["a"] = new HashSet<string> { "href", "target", "rel" };

            // Example: restrict <img> tag
            policy.AllowedAttributes["img"] = new HashSet<string> { "src", "alt", "width", "height" };

            return policy;
        }

        /// <summary>
        /// Clones the current policy into a new one.
        /// </summary>
        public HtmlSanitizerPolicy Clone()
        {
            return new HtmlSanitizerPolicy
            {
                AllowedTags = new HashSet<string>(AllowedTags),
                BlockedTags = new HashSet<string>(BlockedTags),
                GlobalAllowedAttributes = new HashSet<string>(GlobalAllowedAttributes),
                AllowedAttributes = AllowedAttributes.ToDictionary(
                    entry => entry.Key,
                    entry => new HashSet<string>(entry.Value)
                )
            };
        }
    }
}
