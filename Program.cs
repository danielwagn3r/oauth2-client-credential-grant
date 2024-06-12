// See https://aka.ms/new-console-template for more information
using IdentityModel.Client;
using System.Text.Json;

Console.WriteLine("Hello, World!");

// discover endpoints from well-known openid-configuration
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://apisummit.eu.auth0.com");
if (disco.IsError)
{
    Console.WriteLine(disco.Error);
    return;
}

// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = disco.TokenEndpoint,

    ClientId = "wqB2hYtfm2BVQ7GP07XPjO664uEKEthr",
    ClientSecret = "Lpj5AsJChxquoVitWJJLL_rpawjIZpHbSn8K4zP-vHUmDPNHE6jhIFo4WrZk5UbG",

    Resource = { "http://calculator-api" },

    Parameters =
    {
        { "audience", "http://calculator-api" }
    },

    Scope = "calc:double calc:square"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

Console.WriteLine(tokenResponse.AccessToken);

// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("https://localhost:6001/double/2");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
}
else
{
    var content = await response.Content.ReadAsStringAsync();
    var doc = JsonDocument.Parse(content).RootElement;
    Console.WriteLine(JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true }));
}
