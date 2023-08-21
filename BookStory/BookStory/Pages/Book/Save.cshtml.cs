using BookStory.Hubs;
using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace BookStory.Pages.Book
{
    public class SaveModel : PageModel
    {
        private readonly StoryDBContext _context;
        private readonly IHubContext<SignalRServer> _signalRHub;

        public SaveModel(StoryDBContext context, IHubContext<SignalRServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }
        [BindProperty]
        public List<Category> listCategory { get; set; }

        [BindProperty]
        public List<Chapter> listChapter { get; set; }

        public void OnGet(int id, int? id1)
        {
            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null) u = JsonConvert.DeserializeObject<User>(json);
            if (u != null)
            {
                if (u.Uid != id)
                {
                    id = u.Uid;
                }
                listCategory = _context.Categories.ToList();
                listChapter = _context.Chapters.OrderByDescending(s => s.Sid).Where(x => x.Readings.Where(r => r.Ctid == x.Ctid && r.Uid == id).Any()).ToList();
            }
            else
            {
                RedirectToPage("/Users/Login");
            }
        }

        public async Task<IActionResult> OnGetUnSave(int id, int id1)
        {
            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null) u = JsonConvert.DeserializeObject<User>(json);
            if (u != null)
            {
                int ctid = _context.Chapters.FirstOrDefault(x => x.Chapnumber.Equals(id1.ToString()) && x.Sid == id).Ctid;
                Reading r = _context.Readings.FirstOrDefault(x => x.Ctid == ctid);
                _context.Readings.Remove(r);
                _context.SaveChanges();
                await _signalRHub.Clients.All.SendAsync("LoadSave", id);
                return Redirect("/Book/Save");
            }
            else
            {
                return Redirect("/Users/Login");
            }
        }

        public async Task<IActionResult> OnGetSave(int id, int id1)
        {
            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null) u = JsonConvert.DeserializeObject<User>(json);
            if (u != null)
            {
                List<Reading> readings = _context.Readings.ToList();

                int rid = _context.Readings.Any() ? _context.Readings.Max(r => r.Rid) : 1;
                int ctid = _context.Chapters.FirstOrDefault(x => x.Chapnumber.Equals(id1.ToString()) && x.Sid == id).Ctid;
                int count = _context.Readings.Where(x => x.Ctid == ctid && x.Uid == u.Uid).ToList().Count;
                if (count == 0)
                {
                    Reading r = new();
                    r.Rid = rid + 1;
                    r.Uid = u.Uid;
                    r.Ctid = ctid;
                    _context.Add<Reading>(r);
                    _context.SaveChanges();
                }
                await _signalRHub.Clients.All.SendAsync("LoadSave", id);
                return Redirect("/Book/Save");
            }
            else
            {
                return Redirect("/Users/Login");
            }

        }
    }
}
