public class UserPlaceData
{
    public string Id { get; set; }
    public string Env { get; set; }
    public FieldData[] field { get; set; }
}

public class FieldData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public float Reward { get; set; }
    public float CleanFee { get; set; }
    public float RepairFee { get; set; }
    public int EnvPosition { get; set; }
    public int Env { get; set; }
    public string StartTime { get; set; }
    public int Time { get; set; }
    public int Limit { get; set; }
    public string Condition { get; set; }
    public float CurrentTime { get; set; }
    public int Count { get; set; }
}