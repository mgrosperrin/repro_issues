using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace MozzAPI.Controllers;
[ApiController]
[Route("[controller]")]
public class GraphCallController : ControllerBase
{
    private readonly ITokenAcquisition _tokenAcquisition;

    public GraphCallController(ITokenAcquisition tokenAcquisition)
    {
        _tokenAcquisition = tokenAcquisition;
    }

    [HttpGet]
    public async Task<GraphResult> Get()
    {
        var graphToken = await _tokenAcquisition.GetAuthenticationResultForUserAsync(
        scopes: new[] { "user.read" });
        var graphClient = new GraphServiceClient(new PreAuthenticatedTokenCredential(graphToken));
        var currentOrganizations = await graphClient.Organization.Request().GetAsync();
        return new GraphResult
        {
            OrganizationName = currentOrganizations.First().DisplayName
        };
    }
}
internal class PreAuthenticatedTokenCredential : TokenCredential
{
    private readonly AccessToken _accessToken;

    public PreAuthenticatedTokenCredential(AuthenticationResult tokenvalue)
    {
        _accessToken = new AccessToken(tokenvalue.AccessToken, tokenvalue.ExpiresOn);
    }
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return _accessToken;
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new ValueTask<AccessToken>(_accessToken);
    }
}
