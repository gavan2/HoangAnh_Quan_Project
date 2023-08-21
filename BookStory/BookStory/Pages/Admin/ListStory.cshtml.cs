using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace BookStory.Pages.Admin
{
    public class ListStoryModel : PageModel
    {
        private readonly StoryDBContext _context;
        public ListStoryModel(StoryDBContext context)
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
        public List<Story> listStory { get; set; }
        [BindProperty]
        public List<Category> listCategory { get; set; }
        [BindProperty]
        public List<Author> listAuthor { get; set; }

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
            listStory = _context.Stories.ToList();
            listCategory = _context.Categories.ToList();
            listAuthor = _context.Authors.ToList();
        }

        public IActionResult OnPostAddCategory(string category)
        {
            try
            {
                Category c = new()
                {
                    Title = category
                };
                _context.Add<Category>(c);
                _context.SaveChanges();
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }
        }

        public IActionResult OnGetEditCategory(int cid, string title)
        {
            try
            {
                Category c = _context.Categories.FirstOrDefault(x => x.Cid == cid);
                c.Title = title;
                _context.SaveChanges();
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }
        }

        public IActionResult OnPostAddChapter(int sid, string chaptername, string chapternumber, string chaptersubname, string chaptercontent)
        {
            try
            {
                Chapter c = new()
                {
                    Name = chaptername,
                    Subname = chaptersubname,
                    Chapnumber = chapternumber,
                    Content = chaptercontent,
                    Sid = sid,
                    CreatedAt = System.DateTime.Now,
                    UpdatedAt = System.DateTime.Now
                };
                _context.Add<Chapter>(c);
                _context.SaveChanges();
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }
        }

        public IActionResult OnGetChapter(int sid)
        {
            try
            {
                Chapter s = _context.Chapters.OrderByDescending(x => x.Ctid).FirstOrDefault(x => x.Sid == sid);
                if (s == null)
                {
                    s = new()
                    {
                        Sid = sid,
                        Name = _context.Stories.FirstOrDefault(x => x.Sid == sid).Name,
                        Chapnumber = "0",
                    };
                }
                int cnumber = int.Parse(s.Chapnumber) + 1;
                return new JsonResult(new
                {
                    chapnumber = cnumber,
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

        public IActionResult OnGetInformationStory(int storyid)
        {
            try
            {
                Story s = _context.Stories.FirstOrDefault(x => x.Sid == storyid);
                Author a = _context.Authors.FirstOrDefault(x => x.Aid == x.StoriesAuthors.FirstOrDefault(sa => sa.Sid == storyid).Aid);
                List<int> sc = _context.StoriesCategories.Where(x => x.Sid == storyid).Select(x => x.Cid).ToList();
                int[] sa = sc.ToArray();
                Update();
                return new JsonResult(new
                {
                    sid = s.Sid,
                    name = s.Name,
                    status = s.Status,
                    source = s.Source,
                    image = s.Image,
                    keyword = s.Keyword,
                    description = s.Description,
                    authorid = a.Aid,
                    authorname = a.Name,
                    listcid = sa,
                });
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public IActionResult OnGetEditAuthor(int aid, string authorName)
        {
            try
            {
                Author a = _context.Authors.FirstOrDefault(x => x.Aid == aid);
                a.Name = authorName;
                _context.SaveChanges();
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }

        }

        public IActionResult OnPostAddAuthor(string author)
        {
            try
            {
                Author a = new()
                {
                    Aid = _context.Authors.ToList().Count + 1,
                    Name = author
                };
                _context.Add<Author>(a);
                _context.SaveChanges();
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }

        }

        public IActionResult OnPostAddStory(string name, int[] categories, int author, int status, string source, IFormFile image, string keyword, string description)
        {
            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "StoriesImage", image.FileName);
                using (var file = new FileStream(fullPath, FileMode.Create))
                {
                    image.CopyTo(file);
                }
                Story story = new()
                {
                    Name = name,
                    Status = status,
                    Source = source,
                    View = 0,
                    Image = image.FileName,
                    Keyword = keyword,
                    Description = description,
                    CreatedAt = System.DateTime.Now,
                    UpdatedAt = System.DateTime.Now
                };
                _ = _context.Add(story);
                _ = _context.SaveChanges();
                for (int i = 0; i < categories.Length; i++)
                {
                    StoriesCategory sc = new()
                    {
                        Scid = _context.StoriesCategories.OrderByDescending(x => x.Scid).FirstOrDefault().Scid + 1,
                        Cid = categories[i],
                        Sid = _context.Stories.FirstOrDefault(x => x.Name.Equals(story.Name)).Sid
                    };
                    _ = _context.Add(sc);
                    _ = _context.SaveChanges();
                }
                StoriesAuthor sa = new()
                {
                    Said = _context.StoriesAuthors.OrderByDescending(x => x.Said).FirstOrDefault().Said + 1,
                    Sid = _context.Stories.FirstOrDefault(x => x.Name.Equals(story.Name)).Sid,
                    Aid = author
                };
                _ = _context.Add(sa);
                _ = _context.SaveChanges();
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }
        }

        public IActionResult OnPostEditStory(int sid, string name, int[] categories, int author, int status, string source, IFormFile image, string keyword, string description)
        {

            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "StoriesImage", image.FileName);
                using (var file = new FileStream(fullPath, FileMode.Create))
                {
                    image.CopyTo(file);
                }
                var entity = _context.Stories.FirstOrDefault(x => x.Sid == sid);
                entity.Name = name;
                entity.Source = source;
                entity.Image = image.FileName;
                entity.Status = status;
                entity.Keyword = keyword;
                entity.Description = description;
                entity.UpdatedAt = System.DateTime.Now;
                _context.Entry(entity).CurrentValues.SetValues(entity);
                var entity1 = _context.StoriesAuthors.FirstOrDefault(x => x.Sid == sid);
                entity1.Aid = author;
                _context.Entry(entity1).CurrentValues.SetValues(entity1);
                _context.StoriesCategories.RemoveRange(_context.StoriesCategories.Where(x => x.Sid == sid).ToList());
                _context.SaveChanges();
                for (int i = 0; i < categories.Length; i++)
                {
                    StoriesCategory sc = new()
                    {
                        Scid = _context.StoriesCategories.OrderByDescending(x => x.Scid).FirstOrDefault().Scid + 1,
                        Cid = categories[i],
                        Sid = sid
                    };
                    _context.Add<StoriesCategory>(sc);
                    _context.SaveChanges();
                }
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }
        }

        public IActionResult OnGetDeleteStory(int storyid)
        {
            try
            {
                List<StoriesAuthor> storiesAuthors = _context.StoriesAuthors.Where(x => x.Sid == storyid).ToList();
                List<StoriesCategory> storiesCategories = _context.StoriesCategories.Where(x => x.Sid == storyid).ToList();
                List<Chapter> chapters = _context.Chapters.Where(x => x.Sid == storyid).ToList();
                Story s = _context.Stories.FirstOrDefault(x => x.Sid == storyid);
                _context.StoriesAuthors.RemoveRange(storiesAuthors);
                _context.StoriesCategories.RemoveRange(storiesCategories);
                _context.Chapters.RemoveRange(chapters);
                _context.Remove(s);
                _context.SaveChanges();
                return Redirect("/Admin/ListStory");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi rồi!", ex);
            }
        }
    }
}
