public class EmailDto
{
    public uint Uid { get; set; }
    public string? From { get; set; }
    public string? Subject { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool IsRead { get; set; }
}
