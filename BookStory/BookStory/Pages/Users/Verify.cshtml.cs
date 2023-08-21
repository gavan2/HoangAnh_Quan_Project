using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BookStory.Pages.Users
{
    public class VerifyModel : PageModel
    {
        private readonly StoryDBContext _context;
        public VerifyModel(StoryDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string code)
        {
            int? sessioncode = HttpContext.Session.GetInt32("code");
            if (sessioncode != null)
            {
                if (code.Equals(sessioncode.ToString()))
                {
                    string json = HttpContext.Session.GetString("register-user");
                    var user = JsonConvert.DeserializeObject<User>(json);
                    _context.Add<User>(user);
                    _context.SaveChanges();
                    HttpContext.Session.Remove("register-user");
                    HttpContext.Session.Remove("code");
                }
                else
                {
                    message = "Nhập sai mã xác minh vui lòng nhập lại";
                }
            }
            return Page();
        }
    }
}
