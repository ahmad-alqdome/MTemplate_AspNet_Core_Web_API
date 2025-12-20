namespace MyTemplate.Contracts.Responses;

public record RoleDetailResponse(
    Guid Id,
    string Name,
    IEnumerable<Permission> Permissions
);