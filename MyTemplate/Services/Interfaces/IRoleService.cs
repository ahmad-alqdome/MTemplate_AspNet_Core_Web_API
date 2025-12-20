using MyTemplate.Contracts.Requests;

namespace MyTemplate.Services.Interfaces;

public interface IRoleService
{
    Task<Response<List<RoleResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Response<RoleDetailResponse>> GetAsync(Guid id);
    Task<Response<RoleDetailResponse>> AddAsync(RoleRequest request);
    Task<Response<string>> UpdateAsync(Guid id, RoleRequest request);
    Task<Response<string>> ToggleStatusAsync(Guid id);
}
