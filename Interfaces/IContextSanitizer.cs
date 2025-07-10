using SafeInputs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Interfaces
{
    public interface IContextSanitizer
    {
        SanitizationContext Context { get; }
        string Sanitize(string input, object? options = null);
    }
}
