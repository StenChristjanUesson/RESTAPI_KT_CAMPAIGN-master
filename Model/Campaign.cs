namespace ITB2203Application.Model;

public class Campaign
{
    public int Id { get; set; }
    public string? CountryCode { get; set; }
    public string? BodyTemplate { get; set; }
    public List<Email> Emails { get; set; } = new List<Email>();

}
