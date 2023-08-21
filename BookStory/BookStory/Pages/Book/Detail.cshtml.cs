using BookStory.Hubs;
using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using static System.Formats.Asn1.AsnWriter;

namespace BookStory.Pages.Book
{
    public class DetailModel : PageModel
    {
        private readonly StoryDBContext _context;
        private readonly IHubContext<SignalRServer> _signalRHub;

        public DetailModel(StoryDBContext context, IHubContext<SignalRServer> signalRHub)
        {
            _context = context;
            _signalRHub = signalRHub;
        }
        [BindProperty]
        public Story Story { get; set; }

        [BindProperty]
        public Author Author { get; set; }

        [BindProperty]
        public List<Category> CategoriesOfStory { get; set; }

        [BindProperty]
        public List<Chapter> NewChapters { get; set; }

        [BindProperty]
        public List<Chapter> ListChapter { get; set; }

        [BindProperty]
        public List<Story> ListStoryAuthor { get; set; }

        [BindProperty]
        public List<Story> ListStoryHighestView { get; set; }

        [BindProperty]
        public List<Category> listCategory { get; set; }

        public void OnGet(int id)
        {
            listCategory = _context.Categories.ToList();
            Story = _context.Stories.FirstOrDefault(x => x.Sid == id);
            Author = _context.Authors.FirstOrDefault(x => x.Aid == _context.StoriesAuthors.FirstOrDefault(s => s.Sid == id).Aid);

            ListStoryAuthor = _context.Stories.Where(s => s.StoriesAuthors.Where(x => x.Aid == Author.Aid && x.Sid != id).Any()).Take(5).ToList();

            ListStoryHighestView = _context.Stories.OrderByDescending(x => x.View).Take(10).ToList();

            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null)
            {
                u = JsonConvert.DeserializeObject<User>(json);
            }

            List<StoriesCategory> storiesCategories = _context.StoriesCategories
                .Where(x => x.Sid == id)
                .ToList();

            List<int> categoryIds = storiesCategories.Select(sc => sc.Cid).ToList();

            CategoriesOfStory = _context.Categories
                .Where(c => categoryIds.Contains(c.Cid))
                .ToList();

            NewChapters = _context.Chapters.OrderByDescending(x => x.Chapnumber).Where(x => x.Sid == id).Take(5).ToList();

            ListChapter = _context.Chapters.Where(x => x.Sid == id).ToList();
        }
    }
}
