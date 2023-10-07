namespace RestLS.Data.Dtos.GroupSessions;


public record GroupSessionDto(int Id, string Name, int DocId);
public record CreateGroupSessionDto(string Name, string Description,double Price,int Spaces, string Time);
public record UpdateGroupSessionDto(string Name, string Description,double Price,int Spaces, string Time);