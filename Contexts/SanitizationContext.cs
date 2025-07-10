using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Contexts
{
    public enum SanitizationContext
    {
        /// <summary>
        /// Plain text — removes control characters, trims, and normalizes spacing.
        /// </summary>
        PlainText,

        /// <summary>
        /// HTML — strips or allows safe tags/attributes via a policy (e.g., <b>, <p>).
        /// </summary>
        Html,

        /// <summary>
        /// SQL — escapes SQL injection patterns (basic: ', ;, --, etc.).
        /// </summary>
        Sql,

        /// <summary>
        /// URL — applies percent-encoding for query strings or paths.
        /// </summary>
        Url,

        /// <summary>
        /// Attribute — escapes double/single quotes and angle brackets for HTML attributes.
        /// </summary>
        Attribute
    }
}
