using Entities.Domain;
using RestSharp;
using WebApi.Data;

namespace WebApi
{
    public static class ApiClient
    {
        private static readonly string? aspNetCoreVariable = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        static ApiClient()
        {
            string? baseAddress = aspNetCoreVariable?.Split(";")[0];
            RestClient? restClient = baseAddress == null ? null : new(baseAddress);
            HttpClient? api = baseAddress == null ? null : new() { BaseAddress = new Uri(baseAddress) };
        }
    }
}
