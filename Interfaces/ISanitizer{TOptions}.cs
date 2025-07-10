using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Interfaces
{
    public interface ISanitizer<in TOptions>
    {
        string Sanitize(string input, TOptions options);
    }
}
