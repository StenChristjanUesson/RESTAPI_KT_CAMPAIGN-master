using ITB2203Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ITB2203Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignController : ControllerBase
    {
        private readonly DataContext _context;

        public CampaignController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Campaign>> GetCampaign(string? name = null)
        {
            var query = _context.Campaigns.AsQueryable();

            if (name != null)
                query = query.Where(x => x.BodyTemplate != null && x.BodyTemplate.Contains(name, StringComparison.OrdinalIgnoreCase));

            return query.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Campaign> GetCampaign(int id)
        {
            var campaign = _context.Campaigns.Include(c => c.Emails).FirstOrDefault(c => c.Id == id);

            if (campaign == null)
                return NotFound();

            return Ok(campaign);
        }

        [HttpPut("{id}")]
        public IActionResult PutCampaign(int id, Campaign campaign)
        {
            if (id != campaign.Id)
                return BadRequest();

            var dbCampaign = _context.Campaigns.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (dbCampaign == null)
                return NotFound();

            _context.Update(campaign);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost]
        public ActionResult<Campaign> PostCampaign(Campaign campaign)
        {
            if (string.IsNullOrEmpty(campaign.CountryCode) || campaign.CountryCode.Length != 2)
                return BadRequest("CountryCode must be exactly 2 characters.");

            if (_context.Campaigns.Any(c => c.Id == campaign.Id))
                return Conflict();

            _context.Add(campaign);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCampaign), new { id = campaign.Id }, campaign);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCampaign(int id)
        {
            var campaign = _context.Campaigns.Find(id);
            if (campaign == null)
                return NotFound();

            _context.Campaigns.Remove(campaign);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("{id}/employees")]
        public ActionResult<int> GetCampaignEmployees(int id)
        {
            var campaign = _context.Campaigns
                .Include(c => c.Emails)  // Laadime kampaania ja tema seotud e-kirjad
                .FirstOrDefault(c => c.Id == id);

            if (campaign == null)
                return NotFound();

            var companyIds = campaign.Emails.Select(e => e.CompanyId).Distinct().ToList();

            var totalEmployees = _context.Companies
                .Where(c => companyIds.Contains(c.Id))
                .Sum(c => c.NumberOfEmployees);

            return Ok(totalEmployees);
        }


    }
}
