namespace IcmChat
{
    public static class OllamaResilienceHandlerExtension
    {
        public static IServiceCollection AddOllamaResilienceHandler(this IServiceCollection services)
        {
            services.ConfigureHttpClientDefaults(httpClientBuilder =>
            {
#pragma warning disable EXTEXP0001
                httpClientBuilder.RemoveAllResilienceHandlers();
#pragma warning restore EXTEXP0001

                httpClientBuilder.AddStandardResilienceHandler(Options =>
                {
                    Options.AttemptTimeout.Timeout = TimeSpan.FromMinutes(5);
                    Options.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(10);
                    Options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(10);
                });
            });
            return services;
        }
    }
}
