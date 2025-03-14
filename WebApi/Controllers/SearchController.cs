using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController(ISearchService _service): ControllerBase
{
    [HttpGet(Name = "searchByTerm")]
    public async Task<IActionResult> SearchByTerm([Required] string term)
    {
        if (String.IsNullOrEmpty(term) || String.IsNullOrWhiteSpace(term))
        {
            return BadRequest("Term cannot be empty");
        }

        try
        {
            var result = await _service.SearchByTermAsync(term);
            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error searching " + e);
            throw;
        }
    }
}