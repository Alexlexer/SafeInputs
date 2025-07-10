using SafeInputs.Contexts;
using SafeInputs.Interfaces;
using SafeInputs.Policies;
using SafeInputs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SafeInputs.Extensions
{
    public static class SanitizerServiceExtensions
    {
        public static IServiceCollection AddInputSanitizers(this IServiceCollection services)
        {
            services.AddSingleton<ISanitizer, PlainTextSanitizer>();
            services.AddSingleton<ISanitizer, SqlSanitizer>();
            services.AddSingleton<ISanitizer, UrlSanitizer>();
            services.AddSingleton<ISanitizer, AttributeSanitizer>();
            services.AddSingleton<ISanitizer<HtmlSanitizerPolicy>, HtmlSanitizer>();

            services.AddSingleton<IContextSanitizer, PlainTextSanitizer>();
            services.AddSingleton<IContextSanitizer, SqlSanitizer>();
            services.AddSingleton<IContextSanitizer, UrlSanitizer>();
            services.AddSingleton<IContextSanitizer, AttributeSanitizer>();
            services.AddSingleton<IContextSanitizer, HtmlSanitizer>();

            services.AddSingleton<SanitizerDispatcher>();

            return services;
        }
    }
}
