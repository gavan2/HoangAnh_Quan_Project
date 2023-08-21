using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookStory.Pages.Admin
{
    public class ListUserModel : PageModel
    {
        private readonly StoryDBContext _context;
        public ListUserModel(StoryDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int TotalStory { get; set; }
        [BindProperty]
        public int TotalView { get; set; }
        [BindProperty]
        public int TotalUser { get; set; }
        [BindProperty]
        public List<User> listlUser { get; set; }

        public IActionResult OnGet()
        {
            if (VerifyAdmin() == 1)
            {
                Update();
                return Page();
            }
            else if (VerifyAdmin() == 0)
            {
                return Redirect("/Users/Login");
            }
            else
            {
                return Redirect("/Index");
            }
        }

        public int VerifyAdmin()
        {
            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null) u = JsonConvert.DeserializeObject<User>(json);
            if (u != null)
            {
                if (u.Role == 1)
                {
                    return 1;
                }
                else if (u.Role == 2)
                {
                    return 2;
                }
            }
            return 0;
        }
        void Update()
        {
            List<Story> stories = _context.Stories.ToList();
            TotalStory = stories.Count();
            TotalUser = _context.Users.Count();
            int totalView = 0;
            foreach (Story story in stories)
            {
                totalView += (int)story.View;
            }
            TotalView = totalView;
            listlUser = _context.Users.ToList();
        }
    }
}
