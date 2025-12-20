namespace MyTemplate.Contracts.Requests;

public record RoleRequest(
    string Name,
    IList<Permission> Permissions
);