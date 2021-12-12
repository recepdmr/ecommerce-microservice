using CatalogService.Api.Infrastructure.EntityFrameworkCore.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CatalogController : ControllerBase
{

    public CatalogController(CatalogDbContext catalogDbContext)
    {
        CatalogDbContext = catalogDbContext;
    }

    public CatalogDbContext CatalogDbContext { get; }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var catalogs = await CatalogDbContext.CatalogItems.ToListAsync();

        return Ok(catalogs);
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
    {
        var catalog = await CatalogDbContext.CatalogItems.FindAsync(id);

        return Ok(catalog);
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrandsAsync()
    {

        var brands = await CatalogDbContext.CatalogBrands.ToListAsync();

        return Ok(brands);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTypesAsync()
    {
        var types = await CatalogDbContext.CatalogTypes.ToListAsync();

        return Ok(types);
    }
}