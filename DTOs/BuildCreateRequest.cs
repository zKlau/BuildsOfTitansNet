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
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public int Price { get; set; }
    public AbilitySlotRequest? Slots { get; set; }
}

public class AbilitySlotRequest
{
    public int Id { get; set; }
    public string? Name { get; set; }
}