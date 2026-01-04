using System.Text.Json.Serialization;

public class BuildCreateRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Species { get; set; }
    public int Subspecies { get; set; }
    public List<AbilityRequest>? Abilities { get; set; }
}

public class AbilityRequest
{
    public int Id { get; set; }
}

public class UpdateBuildRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<AbilityRequest>? Abilities { get; set; }
}

public class UpdateBuildFullRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
    
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }
    
    [JsonPropertyName("dinosaur")]
    public object? Dinosaur { get; set; }
    
    [JsonPropertyName("votes")]
    public object? Votes { get; set; }
    
    public List<UpdateAbilityRequest>? Abilities { get; set; }
}

public class UpdateAbilityRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public int? Price { get; set; }
    public object? Slots { get; set; }
}