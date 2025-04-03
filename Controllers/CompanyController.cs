using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ITB2203Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly DataContext _context;

        public CompanyController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/replyPercentage")]
        public ActionResult<double> GetCompanyReplyPercentage(int id)
        {
            var company = _context.Companies.Include(c => c.Emails).FirstOrDefault(c => c.Id == id);
            if (company == null)
                return NotFound();

            var totalEmails = company.Emails.Count;
            if (totalEmails == 0)
                return NotFound();

            var repliedEmails = company.Emails.Count(e => e.IsReply);
            var replyPercentage = (double)repliedEmails / totalEmails * 100;

            return Ok(replyPercentage);
        }

        [HttpPost("{id}/emails")]
        public ActionResult<Email> PostEmail(int id, Email email)
        {
            var company = _context.Companies.Find(id);
            if (company == null)
                return NotFound();

            var campaign = _context.Campaigns.Find(email.CampaignId);
            if (campaign == null)
                return NotFound();

            if (_context.Emails.Any(e => e.CampaignId == email.CampaignId && e.CompanyId == email.CompanyId && !e.IsReply))
                return BadRequest("Cannot send duplicate email for the same campaign to the company without a reply.");

            if (string.IsNullOrEmpty(email.Subject))
                return BadRequest("Subject cannot be empty.");

            if (string.IsNullOrEmpty(email.Body))
            {
                email.Body = $"Hello {company.Name}, {campaign.BodyTemplate}";
            }

            email.CompanyId = company.Id;
            _context.Emails.Add(email);
            _context.SaveChanges();

            return CreatedAtAction(nameof(EmailController.GetEmail), new { id = email.Id }, email);
        }
    }
}
