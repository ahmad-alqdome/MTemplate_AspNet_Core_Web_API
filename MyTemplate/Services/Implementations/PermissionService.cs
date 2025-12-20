using MyTemplate.Enums;
using MyTemplate.Persistense;

namespace MyTemplate.Services.Implementations;
public class PermissionService : IPermissionService { 
    private readonly ApplicationDbContext _context; 
    public PermissionService(ApplicationDbContext context) 
    { _context = context; } 
    public async Task AddModulePermissionsAsync() { 
        var Modules = SystemModules.Modules; 
        
        var actions = new[] { 
            PermissionAction.Create, 
            PermissionAction.Read, 
            PermissionAction.Update, 
            PermissionAction.Delete }; 
        
        foreach (var module in Modules) { 
            foreach (var action in actions) { 
                if (!await _context.Permissions.AnyAsync(p => p.Module == module && p.Actions == action)) {
                    _context.Permissions.Add(new Permission { Module = module, Actions = action }); } } 
        
        }
        await _context.SaveChangesAsync(); 
    
    }
}