using System.Collections.Generic;
using System.Linq;

namespace SafeInputs.Policies
{
    public class HtmlSanitizerPolicy
    {
        /// <summary>
        /// List of allowed HTML tags (e.g. b, p, a).
        /// </summary>
        public HashSet<string> AllowedTags { get; set; } = new()
        {
            "b", "i", "u", "p", "strong", "em", "ul", "ol", "li", "br", "span", "div", "a", "img"
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
        /// NOTE: do NOT include "style" or JS event handlers here for safety.
        /// </summary>
        public HashSet<string> GlobalAllowedAttributes { get; set; } = new()
        {
            "class", "id", "title", "href", "src", "alt", "target"
        };

        /// <summary>
        /// Global blocked attributes for any tag (JS handlers, dangerous attrs).
        /// These are always blocked regardless of AllowedAttributes entries.
        /// </summary>
        public HashSet<string> GlobalBlockedAttributes { get; set; } = new()
        {
            "onerror", "onclick", "onload", "onmouseover", "onfocus", "onblur", "style"
        };

        /// <summary>
        /// Per-tag attribute allowlist.
        /// Keys and values are expected in lower-case.
        /// </summary>
        public Dictionary<string, HashSet<string>> AllowedAttributes { get; set; } = new();

        /// <summary>
        /// Per-tag attribute blocklist (optional, overrides allowed lists).
        /// </summary>
        public Dictionary<string, HashSet<string>> BlockedAttributes { get; set; } = new();

        /// <summary>
        /// Checks whether an attribute is allowed on a tag.
        /// First checks explicit blocklist, then per-tag allowlist, then global allowlist.
        /// </summary>
        public bool IsAttributeAllowed(string tag, string attr)
        {
            if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(attr)) return false;

            tag = tag.ToLowerInvariant();
            attr = attr.ToLowerInvariant();

            // If explicitly blocked (global or per-tag) => not allowed
            if (IsAttributeBlocked(tag, attr)) return false;

            // If per-tag allowlist exists => require it
            if (AllowedAttributes.TryGetValue(tag, out var allowedForTag))
                return allowedForTag.Contains(attr);

            // Otherwise, fall back to global allowlist
            return GlobalAllowedAttributes.Contains(attr);
        }

        /// <summary>
        /// Checks whether attribute is explicitly blocked (per-tag or global).
        /// </summary>
        public bool IsAttributeBlocked(string tag, string attr)
        {
            if (string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(attr)) return false;

            tag = tag.ToLowerInvariant();
            attr = attr.ToLowerInvariant();

            if (BlockedAttributes.TryGetValue(tag, out var blockedForTag))
                if (blockedForTag.Contains(attr)) return true;

            return GlobalBlockedAttributes.Contains(attr);
        }

        /// <summary>
        /// Creates a reusable default policy.
        /// </summary>
        public static HtmlSanitizerPolicy Default()
        {
            var policy = new HtmlSanitizerPolicy();

            // Restrict <a> tag to href/target/rel only
            policy.AllowedAttributes["a"] = new HashSet<string> { "href", "target", "rel" };

            // Restrict <img> tag
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
                GlobalBlockedAttributes = new HashSet<string>(GlobalBlockedAttributes),
                AllowedAttributes = AllowedAttributes.ToDictionary(
                    entry => entry.Key,
                    entry => new HashSet<string>(entry.Value)
                ),
                BlockedAttributes = BlockedAttributes.ToDictionary(
                    entry => entry.Key,
                    entry => new HashSet<string>(entry.Value)
                )
            };
        }
    }
}
