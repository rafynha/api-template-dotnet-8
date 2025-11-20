namespace component.template.domain.Models.External.User;

public class DeleteUserResponse
{
    public long Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime DeletedAt { get; set; }
}