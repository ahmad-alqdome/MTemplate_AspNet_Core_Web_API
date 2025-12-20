using Mapster;
using MyTemplate.Contracts.Requests;
using MyTemplate.Persistense;

namespace MyTemplate.Services.Implementations;

public class RoleService(RoleManager<ApplicationRole> _roleManager, ApplicationDbContext _context) : IRoleService
{

    public async Task<Response<RoleDetailResponse>> AddAsync(RoleRequest request)
    {
        var roleIsExists = await _roleManager.RoleExistsAsync(request.Name);

        if (roleIsExists)
            return Response.Failure<RoleDetailResponse>(RoleErrors.DuplicatedRole);

        var allowedPermissions = _context.Permissions.ToList();

      
        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.CreateVersion7().ToString()
        };

        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {


            //await _context.Permissions.AddRangeAsync(request.Permissions);
            

            foreach(var permission in request.Permissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    Permission = permission,
                    Role = role

                });
            }   

            
            await _context.SaveChangesAsync();

            var response = new RoleDetailResponse(role.Id, role.Name, request.Permissions);

            return Response.Success(response);
        }

        var error = result.Errors.First();

        return Response.Failure<RoleDetailResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Response<List<RoleResponse>>> GetAllAsync( CancellationToken cancellationToken = default) 
        => Response.Success(
            await _roleManager.Roles
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken));

    public async Task<Response<RoleDetailResponse>> GetAsync(Guid id)
    {
        if (await _roleManager.FindByIdAsync(id.ToString()) is not { } role)
            return Response.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

        var permissions = await _context.RolePermissions.Include(x => x.Permission).Where(x => x.RoleId == id).Select(x => x.Permission).ToListAsync();

        var response = new RoleDetailResponse(role.Id, role.Name!, permissions);

        return Response.Success(response);
    }

    public async Task<Response<string>> ToggleStatusAsync(Guid id)
    {
        if (await _roleManager.FindByIdAsync(id.ToString()) is not { } role)
            return Response.Failure<string>(RoleErrors.RoleNotFound);

        role.IsDisabled = !role.IsDisabled;

        await _roleManager.UpdateAsync(role);

        return Response.Success("Done");
    }

    public async Task<Response<string>> UpdateAsync(Guid id, RoleRequest request)
    {
        var roleIsExists = await _roleManager.Roles
            .AnyAsync(x => x.Name == request.Name && x.Id != id);

        if (roleIsExists)
            return Response.Failure<string>(RoleErrors.DuplicatedRole);

        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role is null)
            return Response.Failure<string>(RoleErrors.RoleNotFound);

        // كل الصلاحيات المسموحة
        var allowedPermissionIds = await _context.Permissions
            .Select(p => p.Id)
            .ToListAsync();

        // تحقق أن الصلاحيات المرسلة صحيحة
        var requestPermissionIds = request.Permissions.Select(p => p.Id).ToList();

        if (requestPermissionIds.Except(allowedPermissionIds).Any())
            return Response.Failure<string>(RoleErrors.InvalidPermissions);

        role.Name = request.Name;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Response.Failure<string>(
                new Error(error.Code, error.Description, StatusCodes.Status400BadRequest)
            );
        }

        // صلاحيات الدور الحالية
        var existingRolePermissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == id)
            .ToListAsync();

        var existingPermissionIds = existingRolePermissions
            .Select(rp => rp.PermissionId)
            .ToList();

        // الصلاحيات الجديدة
        var permissionsToAdd = requestPermissionIds
            .Except(existingPermissionIds)
            .Select(permissionId => new RolePermission
            {
                RoleId = id,
                PermissionId = permissionId
            })
            .ToList();

        // الصلاحيات المحذوفة
        var permissionsToRemove = existingRolePermissions
            .Where(rp => !requestPermissionIds.Contains(rp.PermissionId))
            .ToList();

        if (permissionsToRemove.Any())
            _context.RolePermissions.RemoveRange(permissionsToRemove);

        if (permissionsToAdd.Any())
            await _context.RolePermissions.AddRangeAsync(permissionsToAdd);

        await _context.SaveChangesAsync();

        return Response.Success("Updated");
    }


}