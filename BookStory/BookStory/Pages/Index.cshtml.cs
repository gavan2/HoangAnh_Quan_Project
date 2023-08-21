using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace BookStory.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly StoryDBContext _context;
        public IndexModel(ILogger<IndexModel> logger, StoryDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public List<Category> listCategory { get; set; }
        [BindProperty]
        public List<Story> listStory { get; set; }
        [BindProperty]
        public Dictionary<Chapter, List<Category>> listNewChapter { get; set; }
        [BindProperty]
        public List<Chapter> saveChapter { get; set; }
        [BindProperty]
        public List<Story> listFullStory { get; set; }
        [BindProperty]
        public int categoryId { get; set; }

        public void OnGet(int Cid)
        {
            ViewData["Cid"] = new SelectList(_context.Categories, "Cid", "Title");
            listCategory = _context.Categories.ToList();
            categoryId = Cid;
            List<StoriesCategory> storiesCategories = new();
            storiesCategories = _context.StoriesCategories.Where(x => x.Cid == Cid).ToList();
            if (storiesCategories.Count != 0)
            {
                List<Story> stories = new();
                foreach (StoriesCategory sc in storiesCategories)
                {
                    foreach (Story story in _context.Stories.ToList())
                    {
                        if (story.Sid == sc.Sid)
                        {
                            stories.Add(story);
                        }
                    }
                }
                listStory = stories;
            }
            else
            {
                listStory = _context.Stories.OrderByDescending(s => s.Sid).ToList();
            }

            Dictionary<Chapter, List<Category>> listNewChapters = new();
            foreach (Chapter c in _context.Chapters.OrderByDescending(x => x.CreatedAt).Take(24).ToList())
            {
                listNewChapters.Add(c, GetCategoriesBySid(c.Sid));
            }
            _ = _context.Chapters.ToList();
            listNewChapter = listNewChapters;

            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null)
            {
                u = JsonConvert.DeserializeObject<User>(json);
                List<Chapter> SaveChapters = _context.Chapters.OrderByDescending(s => s.Sid).Where(x => x.Readings.Where(r => r.Ctid == x.Ctid && r.Uid == u.Uid).Any()).Take(16).ToList();
                saveChapter = SaveChapters;
            }
            listFullStory = _context.Stories.Where(x => x.Status == 1).OrderByDescending(s => s.Sid).Take(12).ToList();
        }

        public List<Category> GetCategoriesBySid(int sid)
        {
            List<Category> categories = new();
            categories = _context.Categories.Where(x => x.StoriesCategories.Where(s => s.Sid == sid).Any()).ToList();
            return categories;
        }


    }
}