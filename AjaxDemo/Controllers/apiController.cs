using AjaxDemo.Models;
using AjaxDemo.Models.DOT;
using AjaxDemo.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mime;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace AjaxDemo.Controllers
{
    public class apiController : Controller
    {
        private readonly MyDbContext _context;
        private readonly IWebHostEnvironment _iwhe;
        public apiController(MyDbContext context,IWebHostEnvironment iwhe)
        {
            _context = context;
            _iwhe = iwhe;
        }
        public IActionResult Index()
        {
            //System.Threading.Thread.Sleep(5000);
            return Content("<h2>Hollow Knight~哈哈哈~</h2>", "text/html", System.Text.Encoding.UTF8);
        }
        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(x => x.City).Distinct();
            return Json(cities);
        }

        public IActionResult Districts(string city)
        {
            var districts = _context.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }
        
        public IActionResult Roads(string districts)
        {
            var roads = _context.Addresses.Where(a => a.SiteId == districts).Select(a => a.Road);
            return Json(roads);
        }

        public IActionResult CheckAccount(string name)
        {
            var member = _context.Members.Any(m => m.Name == name);

            return Content(member.ToString(), "text/plain", System.Text.Encoding.UTF8);
        }


        public IActionResult Avatar(int id = 1)
        {
            Member? member = _context.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                if (img != null)
                    return File(img, "image/jpeg");
            }
            return NotFound();
        }

        public IActionResult Register(Member member, IFormFile avatar)
        {
            if (string.IsNullOrEmpty(member.Name))
            {
                member.Name = "guest";
            }
            //寫進資料庫
            byte[]? imgByte = null; 
            using (var memoryString = new MemoryStream())
            {
                avatar.CopyTo(memoryString);
                imgByte= memoryString.ToArray();
            }
            member.FileName = avatar.FileName;
            member.FileData = imgByte;

            _context.Members.Add(member);
            _context.SaveChanges();
            //return Content($"Hello {member.Name}. You're over {member.Age}. E-mail: {member.Email}");
            //string info = $"{avatar.FileName} - {avatar.Length} - {avatar.ContentType}";
            //string info = _iwhe.ContentRootPath;

            //檔案上傳-->實際路徑
            //string uploadPath = @"C:\Users\User\Documents\workspace\MSIT158Site\wwwroot\uploads\a.png";
            //WebRootPath：C:\Users\User\Documents\workspace\MSIT158Site\wwwroot
            //ContentRootPath：C:\Users\User\Documents\workspace\MSIT158Site
            string uploadPath = Path.Combine(_iwhe.WebRootPath, "uploads", avatar.FileName);
            string info = uploadPath;
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                avatar.CopyTo(fileStream);
            }

            return Content(info, "text/plain", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO searchDTO)
        {
            //根據分類編號搜尋景點資料(?"True":"False";)
            var spots = searchDTO.categoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == searchDTO.categoryId);

            //根據關鍵字搜尋景點資料(title、desc)
            if (!string.IsNullOrEmpty(searchDTO.keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(searchDTO.keyword) || s.SpotDescription.Contains(searchDTO.keyword));
            }

            //排序
            switch (searchDTO.sortBy)
            {
                case "spotTitle":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = searchDTO.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //總共有多少筆資料
            int totalCount = spots.Count();
            //每頁要顯示幾筆資料
            int pageSize = searchDTO.pageSize;//?? 9;
            //目前第幾頁
            int page = searchDTO.page;
            //計算總共有幾頁
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            //分頁
            spots = spots.Skip((page - 1) * pageSize).Take(pageSize);


            //包裝要傳給client端的資料
            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalCount = totalCount;
            spotsPaging.TotalPages = totalPages;
            spotsPaging.SpotsResult = spots.ToList();
            return Json(spotsPaging);
        }


        public IActionResult AutoComplete(string keyword)
        {
            var spots = _context.SpotImagesSpots;
            if (!string.IsNullOrEmpty(keyword))
            {
                var titles = spots.Where(s => s.SpotTitle.Contains(keyword)).Select(s => s.SpotTitle).Take(8);
                return Json(titles);
            }
            return NotFound();
        }
    }
}
