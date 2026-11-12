using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RecebimentoMercadorias.Services;

public sealed class SupabaseAdminClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public SupabaseAdminClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<Guid> InviteUserAsync(string email, string nome, CancellationToken cancellationToken = default)
    {
        var supabaseUrl = _configuration["SUPABASE_URL"]?.TrimEnd('/')
            ?? throw new InvalidOperationException("Configure SUPABASE_URL.");
        var serviceRoleKey = _configuration["SUPABASE_SERVICE_ROLE_KEY"]
            ?? throw new InvalidOperationException("Configure SUPABASE_SERVICE_ROLE_KEY para convidar usuários.");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{supabaseUrl}/auth/v1/invite");
        request.Headers.Add("apikey", serviceRoleKey);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", serviceRoleKey);
        request.Content = JsonContent.Create(new
        {
            email,
            data = new { nome }
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode is not HttpStatusCode.OK and not HttpStatusCode.Created)
        {
            throw new SupabaseAdminException(
                response.StatusCode,
                $"O Supabase não aceitou o convite de acesso (HTTP {(int)response.StatusCode}).");
        }

        var invitedUser = await response.Content.ReadFromJsonAsync<InviteUserResponse>(
            cancellationToken: cancellationToken);

        if (invitedUser is null || invitedUser.Id == Guid.Empty)
        {
            throw new SupabaseAdminException(
                response.StatusCode,
                "O Supabase respondeu sem o identificador do usuário convidado.");
        }

        return invitedUser.Id;
    }

    private sealed class InviteUserResponse
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
    }
}

public sealed class SupabaseAdminException : Exception
{
    public SupabaseAdminException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}
