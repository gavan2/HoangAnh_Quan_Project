using BookStory.Hubs;
using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookStory.Pages.Book
{
    public class ContentModel : PageModel
    {
        private readonly StoryDBContext _context;
        private readonly IHubContext<SignalRServer> _signalRHub;

        public ContentModel(StoryDBContext context, IHubContext<SignalRServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }

        [BindProperty]
        public List<Category> listCategory { get; set; }

        [BindProperty]
        public List<Chapter> AllChapters { get; set; }

        [BindProperty]
        public Chapter Chapter { get; set; }

        [BindProperty]
        public bool IsSave { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, int id1)
        {
            listCategory = _context.Categories.ToList();
            Chapter c = _context.Chapters.FirstOrDefault(x => x.Sid == id && x.Chapnumber == id1.ToString());
            if (c == null)
            {
                return NotFound();
            }
            Story s = _context.Stories.FirstOrDefault(x => x.Sid == id);
            AllChapters = _context.Chapters.Where(x => x.Sid == id).ToList();
            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null)
            {
                u = JsonConvert.DeserializeObject<User>(json);
                bool Issave = false;
                foreach (Reading r in _context.Readings.ToList())
                {
                    if (r.Uid == u.Uid && r.Ctid == c.Ctid)
                    {
                        Issave = true;
                    }
                }
                IsSave = Issave;
            }
            s.View += 1;
            _context.SaveChanges();
            Chapter = c;
            await _signalRHub.Clients.All.SendAsync("LoadBook", id, id1);
            return Page();
        }
    }
}
