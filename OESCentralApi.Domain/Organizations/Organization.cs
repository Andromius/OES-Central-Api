namespace OESCentralApi.Domain.Organizations;

public class Organization
{
    public string Name { get; set; }
    public Uri Uri { get; set; }
    public DateTime Added { get; set; }
    public DateTime Updated { get; set; }
    public string Password { get; set; }
}
