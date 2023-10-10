namespace RestLS.Data.Dtos.Recomendation;

public record RecomendationDto(int Id, string Description);
public record CreateRecomendationDto(string Description);
public record UpdateRecomendationDto(string Description);