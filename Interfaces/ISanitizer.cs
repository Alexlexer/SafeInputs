using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeInputs.Interfaces
{
    public interface ISanitizer
    {
        string Sanitize(string input);
    }
}
