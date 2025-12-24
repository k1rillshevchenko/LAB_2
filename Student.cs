namespace LAB_2.Models;

public class Student
{
    public string FullName { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class SearchParams
{
    public string? FullName { get; set; }
    public string? Faculty { get; set; }
    public string? Department { get; set; }
}
