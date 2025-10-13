using SafeInputs.Enums;

namespace SafeInputs.Interfaces
{
    public interface IContextSanitizer
    {
        SanitizationContext Context { get; }
        string Sanitize(string input, object? options = null);
    }
}
