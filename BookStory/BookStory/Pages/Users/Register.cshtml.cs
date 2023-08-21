using BookStory.Common;
using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BookStory.Pages.Users
{
    public class RegisterModel : PageModel
    {
        private readonly StoryDBContext _context;
        public RegisterModel(StoryDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }
        [BindProperty]
        public string repass { get; set; }

        [BindProperty]
        public string message { get; set; }

        public void OnGet()
        {
            message = "Đăng ký tài khoản";
        }

        public IActionResult OnPost()
        {
            bool isRegister = true;
            foreach (User u in _context.Users.ToList())
            {
                if (u.Email.Equals(User.Email))
                {
                    isRegister = false;
                }
            }
            if (!User.Password.Equals(repass))
            {
                message = "Mật khẩu nhập lại không giống với mật khẩu cũ";
            }
            else if (isRegister == false)
            {
                message = "Đăng ký thất bại, email đã tồn tại trong hệ thống";
            }
            else
            {
                Random generator = new Random();
                int r = generator.Next(100000, 1000000);
                new MailHelper().SendMail(User.Email, "Xác minh từ hệ thống BookStory", $"Mã xác minh của bạn là {r}");
                HttpContext.Session.SetInt32("code", r);
                User.Role = 2;
                HttpContext.Session.SetString("register-user", JsonConvert.SerializeObject(User));
                return Redirect("/Users/Verify");
            }
            return Page();
        }
    }
}
