using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Policies
{
    public class HtmlSanitizerPolicy
    {
        public HashSet<string> AllowedTags { get; set; } = new() { "b", "i", "u", "p", "strong", "em", "ul", "ol", "li" };
        public Dictionary<string, HashSet<string>> AllowedAttributes { get; set; } = new();

        public bool IsAttributeAllowed(string tag, string attr)
        {
            return AllowedAttributes.TryGetValue(tag, out var attrs) && attrs.Contains(attr);
        }

        public static HtmlSanitizerPolicy Default() => new();
    }

}
