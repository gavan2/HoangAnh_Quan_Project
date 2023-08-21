using BookStory.Common;
using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStory.Pages.Users
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly StoryDBContext _context;
        public ForgotPasswordModel(StoryDBContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string email)
        {
            User u = _context.Users.FirstOrDefault(x => x.Email == email);
            if (u != null)
            {
                Random generator = new Random();
                int r = generator.Next(100000, 1000000);
                new MailHelper().SendMail(email, $"Mật khẩu mới của tài khoản {email}", $"Mật khẩu mới của bạn là {r}");
                u.Password = r.ToString();
                _context.SaveChanges();
                message = "Đã gửi mật khẩu mới vào email, vui lòng kiểm tra";
                return Page();
            }
            else
            {
                message = "Email không tồn tại trong hệ thống!";
            }
            return Page();
        }
    }
}
