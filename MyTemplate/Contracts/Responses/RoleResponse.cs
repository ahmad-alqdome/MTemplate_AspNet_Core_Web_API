namespace MyTemplate.Contracts.Responses;

public record RoleResponse(
    string Id,
    string Name,
    bool IsDeleted
);