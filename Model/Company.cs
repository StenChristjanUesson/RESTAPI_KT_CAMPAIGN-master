namespace ITB2203Application.Model
{
    public class Company
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Domain { get; set; }
        public int NumberOfEmployees { get; set; }
        public List<Email> Emails { get; set; } = new List<Email>();

    }
}
