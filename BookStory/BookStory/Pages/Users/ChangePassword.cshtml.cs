using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BookStory.Pages.Users
{
    public class ChangePasswordModel : PageModel
    {
        private readonly StoryDBContext _context;
        public ChangePasswordModel(StoryDBContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string opassword, string npassword, string cpassword)
        {
            User u = null;
            string json = HttpContext.Session.GetString("user");
            if (json != null) u = JsonConvert.DeserializeObject<User>(json);
            string message = "";
            var entity = _context.Users.FirstOrDefault(s => s.Uid == u.Uid);
            if (!entity.Password.Equals(opassword))
            {
                message = "Mật khẩu cũ không đúng. Vui lòng nhập lại";
            }
            else if (entity.Password == npassword)
            {
                message = "Mật khẩu mới giống với mật khẩu cũ. Vui lòng nhập lại";
            }
            else if (!npassword.Equals(cpassword))
            {
                message = "Mật khẩu nhập lại không khớp với mật khẩu cũ";
            }
            else
            {

                entity.Password = npassword;
                _context.Entry(entity).CurrentValues.SetValues(entity);
                _context.SaveChanges();
                message = "Đổi mật khẩu thành công";
            }
            Message = message;
            return Page();
        }
    }
}
