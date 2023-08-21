using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace BookStory.Pages.Admin
{
    public class ListChapterModel : PageModel
    {
        private readonly StoryDBContext _context;
        public ListChapterModel(StoryDBContext context)
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
        public List<Chapter> listChapter { get; set; }
        [BindProperty]
        public string message { get; set; }
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
            listChapter = _context.Chapters.ToList();
        }

        public IActionResult OnPostEditChapter(int ctid, string chaptername, string chapternumber, string chaptersubname, string chaptercontent)
        {
            try
            {
                var entity = _context.Chapters.FirstOrDefault(x => x.Ctid == ctid);
                entity.Name = chaptername;
                entity.Subname = chaptersubname;
                entity.Chapnumber = chapternumber;
                entity.Content = chaptercontent;
                entity.UpdatedAt = DateTime.Now;
                _context.Entry(entity).CurrentValues.SetValues(entity);
                _context.SaveChanges();
                message = " | Cập nhật thành công chương " + entity.Chapnumber + " của truyện " + entity.Name;
                Update();
                return Page();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }

        }

        public IActionResult OnGetDeleteChapter(int chapterid)
        {
            try
            {
                List<Reading> readings = _context.Readings.Where(x => x.Ctid == chapterid).ToList();
                Chapter chapter = _context.Chapters.FirstOrDefault(x => x.Ctid == chapterid);
                _context.RemoveRange(readings);
                _context.Remove(chapter);
                _context.SaveChanges();
                Update();
                return Page();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }
        }

        public JsonResult OnGetChapterByCtid(int ctid)
        {
            try
            {
                Chapter s = _context.Chapters.FirstOrDefault(x => x.Ctid == ctid);
                return new JsonResult(new
                {
                    chapnumber = s.Chapnumber,
                    name = s.Name,
                    subname = s.Subname,
                    content = s.Content,
                });
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
