using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace ProductApplication.Controllers;
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IConfiguration _configuration;

    public ProductController(ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
        _tokenAcquisition = tokenAcquisition;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IEnumerable<ProductResult>> Get()
    {
        var mozzAPIToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
            new[] { $"api://{_configuration.GetValue<string>("EntraId:MozzAPIClientId")}/access_as_user" });
        var mozzAPIClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7049")
        };
        mozzAPIClient.DefaultRequestHeaders.Authorization = new("Bearer", mozzAPIToken);
        var response = await mozzAPIClient.GetAsync("GraphCall");
        var result = await response.Content.ReadFromJsonAsync<MozzAPIResult>();
        return new[] { new ProductResult { Source = "MozzAPI", OrganizationName = result!.OrganizationName } };
    }
}
