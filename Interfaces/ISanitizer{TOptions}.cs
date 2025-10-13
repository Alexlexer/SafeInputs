namespace SafeInputs.Interfaces
{
    public interface ISanitizer<in TOptions>
    {
        string Sanitize(string input, TOptions options);
    }
}
