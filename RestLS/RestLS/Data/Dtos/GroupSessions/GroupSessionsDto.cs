namespace RestLS.Data.Dtos.GroupSessions;


public record GroupSessionDto(int Id, string Name, int DocId);
public record CreateGroupSessionDto(string Name);
public record UpdateGroupSessionDto(string Name);