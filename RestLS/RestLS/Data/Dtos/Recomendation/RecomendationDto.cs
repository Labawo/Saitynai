namespace RestLS.Data.Dtos.Recomendation;

public record RecomendationDto(int Id, string Name,string Description);
public record CreateRecomendationDto(string Name, string Description);
public record UpdateRecomendationDto(string Name, string Description);