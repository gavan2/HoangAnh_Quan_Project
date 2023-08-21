using BookStory.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace BookStory.Pages.Users
{
    public class LoginModel : PageModel
    {

        private readonly StoryDBContext _context;
        public LoginModel(StoryDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User user { get; set; }

        [BindProperty]
        public string message { get; set; }

        public void OnGet()
        {
            message = "Nhập thông tin đăng nhập hệ thống";
        }

        public IActionResult OnPost()
        {
            User u = _context.Users.SingleOrDefault(x => x.Email.Equals(this.user.Email) && x.Password.Equals(this.user.Password));
            if (u != null)
            {
                HttpContext.Session.SetString("user", JsonConvert.SerializeObject(u));
                return RedirectToPage("/Index");
            }
            else
            {
                message = "Sai thông tin đăng nhập";
                return Page();
            }
        }

    }
}
