namespace RestLS.Data.Dtos.Doctors;

public record UserDto(int Id, string Nickname, string Password);
public record CreateUserDto(string Nickname, string Password);
public record UpdateUserDto(string Nickname);
