namespace RestLS.Data.Dtos.Admins;

public record AdminDto(int Id, string Name, string Lastname);
public record CreateAdminDto(string Name, string Lastname);
public record UpdateAdminDto(string Name, string Lastname);