using RestSharp;

namespace WebApi
{
    public static class ApiClient
    {
        static readonly string baseAddress = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(";")[0];
        public static readonly RestClient restClient = new(baseAddress);
        public static readonly HttpClient api = new() { BaseAddress = new Uri(baseAddress) };
    }
}
