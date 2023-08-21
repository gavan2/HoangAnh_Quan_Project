using BookStory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStory.Pages.Book
{
    public class SearchModel : PageModel
    {
        private readonly StoryDBContext _context;
        public SearchModel(StoryDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<Category> listCategory { get; set; }

        [BindProperty]
        public List<Story> ListStoryHighestView { get; set; }

        [BindProperty]
        public List<Author> ListAuthors { get; set; }

        [BindProperty]
        public List<Story> ListSearch { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnGetSearch(string id)
        {
            try
            {
                listCategory = _context.Categories.ToList();
                ListStoryHighestView = _context.Stories.OrderByDescending(x => x.View).Take(10).ToList();
                _ = _context.Chapters.ToList();
                _ = _context.StoriesAuthors.ToList();
                ListAuthors = _context.Authors.ToList();
                var storiesPage = _context.Stories.Where(x => x.StoriesCategories.Where(s => s.Cid.ToString() == id).Any()).ToList();
                if (storiesPage.Count == 0)
                {
                    storiesPage = _context.Stories.Where(x => x.Name.Contains(id) || x.Keyword.Contains(id)).ToList();
                    if (storiesPage.Count == 0)
                    {
                        storiesPage = GetStoryByAuthor(id);
                    }
                }
                if (id.Equals("searchfull"))
                {
                    storiesPage = _context.Stories.ToList();
                }
                ListSearch = storiesPage;
                return Page();

            }
            catch (Exception ex)
            {
                return Page();
            }
        }

        public List<Story> GetStoryByAuthor(string authorname)
        {
            /*  List<Story> stories = new();
              foreach (Author a in _context.Authors.ToList())
              {
                  foreach (StoriesAuthor sa in _context.StoriesAuthors.ToList())
                  {
                      if (a.Aid == sa.Aid && a.Name.ToLower().Contains(authorname.ToLower()))
                      {
                          stories.AddRange(_context.Stories.Where(x => x.Sid == sa.Sid).ToList());
                      }
                  }
              }*/

            var stories = (
                    from a in _context.Authors
                    join sa in _context.StoriesAuthors on a.Aid equals sa.Aid
                    join s in _context.Stories on sa.Sid equals s.Sid
                    where a.Name.ToLower().Contains(authorname.ToLower())
                    select s
                           ).ToList();

            return stories;
        }
    }
}
