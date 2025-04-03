using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITB2203Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly DataContext _context;

        public EmailController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult<Email> PostEmail(Email email)
        {
            var company = _context.Companies.Find(email.CompanyId);
            if (company == null)
                return NotFound();

            var campaign = _context.Campaigns.Find(email.CampaignId);
            if (campaign == null)
                return NotFound();

            if (string.IsNullOrEmpty(email.Subject))
                return BadRequest("Subject cannot be empty.");

            if (_context.Emails.Any(e => e.CampaignId == email.CampaignId && e.CompanyId == email.CompanyId && !e.IsReply))
                return BadRequest("Cannot send duplicate email for the same campaign to the company without a reply.");

            if (string.IsNullOrEmpty(email.Body))
                email.Body = $"Hello {company.Name}, {campaign.BodyTemplate ?? "No template available"}";

            _context.Emails.Add(email);
            _context.SaveChanges();

            return CreatedAtAction(nameof(EmailController.GetEmail), new { id = email.Id }, email);
        }

        [HttpGet("{id}")]
        public ActionResult<Email> GetEmail(int id)
        {
            var email = _context.Emails
                .Include(e => e.Company)
                .Include(e => e.Campaign)
                .FirstOrDefault(e => e.Id == id);

            if (email == null)
                return NotFound();

            return Ok(email);
        }

    }
}
