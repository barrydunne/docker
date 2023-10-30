using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Text;
using IdentityModel.Client;

const string ids = "http://localhost:10004";
const string api = "http://localhost:11080";

// Get access token
using var client = new HttpClient();
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = $"{ids}/connect/token",
    ClientId = "client",
    ClientSecret = "secret",
    Scope = "publicapi"
});
if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    return;
}

// Create request
var jobRequest = new
{
    StartingAddress = "Silver Creek Court, Orlando, FL 34714, United States",
    DestinationAddress = "Chocolate Emporium, 6000 Universal Blvd, Orlando, FL 32819, United States",
    Email = "string@example.com"
};

// Send request
var content = new StringContent(JsonSerializer.Serialize(jobRequest), Encoding.UTF8, MediaTypeNames.Application.Json);
var request = new HttpRequestMessage(HttpMethod.Post, $"{api}/job") { Content = content };
request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());
client.SetBearerToken(tokenResponse.AccessToken!);
var apiResponse = await client.SendAsync(request);

// Check response
if (apiResponse.IsSuccessStatusCode)
{
    var apiResponseContent = await apiResponse.Content.ReadAsStringAsync();
    Console.WriteLine(apiResponseContent);
}
else
    Console.WriteLine(apiResponse.StatusCode);