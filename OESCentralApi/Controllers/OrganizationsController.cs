using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OESCentralApi.Domain.Organizations;
using OESCentralApi.Domain.Services;
using OESCentralApi.Persistence;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OESCentralApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrganizationsController : ControllerBase
{
    private readonly OESCentralApiDbContext _context;
    private readonly HttpClient _httpClient;
    public OrganizationsController(OESCentralApiDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    // GET: api/<OrganizationsController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationResponse>>> Get([FromQuery] string keyword)
    {
        List<OrganizationResponse> response = await _context.Organization
            .Where(o => o.Name.ToLower().Contains(keyword.ToLower()))
            .Select(o => new OrganizationResponse(o.Name, o.Uri))
            .ToListAsync();

        return Ok(response);
    }

    // GET api/<OrganizationsController>/5
    [HttpGet("{name}")]
    public async Task<ActionResult<OrganizationResponse>> GetOne(string name)
    {
        OrganizationResponse? response = await _context.Organization
            .Where(o => o.Name == name)
            .Select(o => new OrganizationResponse(o.Name, o.Uri))
            .SingleOrDefaultAsync();

        if (response is null)
            return NotFound();

        return Ok(response);
    }

    // POST api/<OrganizationsController>
    [HttpPost]
    public async Task<ActionResult<OrganizationPasswordResponse>> Post([FromBody] OrganizationRequest request)
    {
        Uri pingUri = new(request.Uri, "api");
        HttpRequestMessage toPing = new(HttpMethod.Get, pingUri);
		//try
		//{
		//	var appResponse = await _httpClient.SendAsync(toPing);
		//	if ((int)appResponse.StatusCode != 418)
		//		return BadRequest("Received an incorrect status code (Unable to verify)");
		//}
		//catch (Exception)
		//{
		//	return BadRequest("Received an incorrect status code (Unable to verify)");
		//}

		string pass = PasswordService.GenerateRandomPassword();
        Organization newOrganization = new()
        {
            Name = request.Name,
            Uri = request.Uri,
            Added = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            Password = PasswordService.GetHash(pass),
        };

        _context.Add(newOrganization);
        await _context.SaveChangesAsync();

        return Created($"api/organizations/{request.Name}", new OrganizationPasswordResponse(request.Name, request.Uri, pass));
    }

    [HttpPut("{name}")]
    public async Task<ActionResult> Put(string name, [FromBody] OrganizationPasswordRequest request)
    {
		Uri pingUri = new(request.Uri, "api");
		HttpRequestMessage toPing = new(HttpMethod.Get, pingUri);
        try
        {
			var appResponse = await _httpClient.SendAsync(toPing);
			if ((int)appResponse.StatusCode != 418)
				return BadRequest("Received an incorrect status code (Unable to verify)");
		}
        catch (Exception)
        {
			return BadRequest("Received an incorrect status code (Unable to verify)");
		}

        Organization? org = await _context.Organization.SingleOrDefaultAsync(o => o.Name == name);
        if (org is null)
			return NotFound();

        if (!PasswordService.CompareHash(request.Password, org.Password))
            return Forbid("Invalid password");

        org.Updated = DateTime.UtcNow;
        org.Uri = request.Uri;

        await _context.SaveChangesAsync();
        return NoContent();
	}

    [HttpDelete("{name}")]
	public async Task<ActionResult> Delete(string name, [FromBody] string password)
    {
		Organization? org = await _context.Organization.SingleOrDefaultAsync(o => o.Name == name);
		if (org is null)
			return NotFound();

		if (!PasswordService.CompareHash(password, org.Password))
			return Forbid("Invalid password");

        _context.Remove(org);
        await _context.SaveChangesAsync();
        return NoContent();
	}
}
