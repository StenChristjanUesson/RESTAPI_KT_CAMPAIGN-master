namespace ITB2203Application.Model
{
    public class Email
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int CompanyId { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public bool IsReply { get; set; }

        public Company Company { get; set; }
        public Campaign Campaign { get; set; }
    }
}
