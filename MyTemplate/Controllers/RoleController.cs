using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyTemplate.Contracts.Requests;

namespace MyTemplate.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly IRoleService roleService;
    public RoleController(IRoleService roleService)
    {
        this.roleService = roleService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken) => this.ToActionResult(await roleService.GetAllAsync(cancellationToken));
    
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id) => this.ToActionResult(await roleService.GetAsync(id));
    
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] RoleRequest request) => this.ToActionResult(await roleService.AddAsync(request));

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] RoleRequest request) => this.ToActionResult(await roleService.UpdateAsync(id, request));

    [HttpPut("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatusAsync([FromRoute] Guid id) => this.ToActionResult(await roleService.ToggleStatusAsync(id));

}
