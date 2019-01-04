using APO.Models;
using APO.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APO.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db=new ApplicationDbContext())
            {
                ViewBag.idImage = db.Images.FirstOrDefault().Id;
            }
            var userId = ApplicationUser.GetUserId();
            if (userId == null)
            {

            }
            else
            {
                var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
                user.LoadLikedImages();
                
                using (var db = new ApplicationDbContext())
                {
                   
                }
            }



            return View();
        }




        public ActionResult ListImages(string [] Place, string[] Type,int startId= 0)//int count,
        {
            var mass = Image.GetListId(Place, Type, startId);//count,
            ViewBag.lastImageId = mass.LastOrDefault();
            ViewBag.massImages = mass;
            //return RedirectToRoute(new { action = "Index", controller= "Home" });
            //return RedirectToAction("Index","Home",new { });
            return PartialView();
        }

        public ActionResult ListLikedImages(string[] Place, string[] Type , int startId=0)//int count,
        {
            var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());

            if (Place == null)
                Place = new string[0];
            if (Type == null)
                Type = new string[0];

            List<int> mass = new List<int>();
            if (user == null)
            {
                var imgslikeC = HttpContext.Request.Cookies["LikedImgMass"]?.Value??"";
                string newFavStr;
                mass = Image.GetFromCookies(Place, Type,imgslikeC, out newFavStr, startId);
                HttpContext.Request.Cookies["LikedImgMass"].Value = newFavStr;
            }
            else
            {
                user.LoadLikedImages();
                //SkipWhile

                var query = user.ImagesLikes.Where(x1 => x1.Id > startId && !x1.Deleted);
                if (Place.Length > 0)
                    query.Where(x1 => x1.Place != null && Place.Contains(x1.Place));
                if (Type.Length > 0)
                    query.Where(x1 => x1.Type != null && Type.Contains(x1.Type));
                mass = query.Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();



                
            }
                
            ViewBag.lastImageLikedId = mass.LastOrDefault();
            ViewBag.massImagesLiked = mass;
            return PartialView();
        }


        public ActionResult ListFavoriteImages( string[] Place, string[] Type, int startId=0)//int count,
        {
            if (Place == null)
                Place = new string[0];
            if (Type == null)
                Type = new string[0];

            var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            List<int> mass = new List<int>();
            if (user == null)
            {
                var imgsFavC = HttpContext.Request.Cookies["FavoritedImgMass"]?.Value ?? "";
                string newFavStr;
                mass=Image.GetFromCookies(Place,Type, imgsFavC, out newFavStr, startId);
                HttpContext.Request.Cookies["FavoritedImgMass"].Value = newFavStr;
            }
            else
            {
                user.LoadFavoritedImages();
                //SkipWhile

                var query = user.ImagesFavorites.Where(x1 => x1.Id > startId && !x1.Deleted);
                if (Place.Length > 0)
                    query.Where(x1 => x1.Place != null && Place.Contains(x1.Place));
                if (Type.Length > 0)
                    query.Where(x1 => x1.Type != null && Type.Contains(x1.Type));
                mass = query.Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();

               
            }
               
            
            ViewBag.lastImageFavoritedId = mass.LastOrDefault();
            ViewBag.massImagesFavorited = mass;
            return PartialView();
        }

        //[HttpPost]
        public ActionResult DeleteImage(int id)
        {
            Image.Delete(id);
            return RedirectToAction("Index");
        }

        //[HttpPost]
        public ActionResult AddImages(HttpPostedFileBase file, HttpPostedFileBase[] uploadImage)
        {
            var massCords = new StreamReader(file.InputStream).ReadToEnd().Split('\n');


            for(int i= 0;i<(massCords.Length < uploadImage.Length ? massCords.Length : uploadImage.Length);++i)
            {
                var img = new Image( massCords[i].Trim());
                img.AddNew(uploadImage[i]);
            }

            return RedirectToAction("Index");
        }

        //[HttpPost]
        public ActionResult AddImage(Image a, HttpPostedFileBase uploadImage)//string name, string description, string cords, HttpPostedFileBase uploadImage
        {
            var img = new Image(a.Name, a.Description, a.Place, a.Type,a.Cords);
            img.AddNew(uploadImage);

            return RedirectToAction("Index");
        }

        //для связывания параметров передавать их по именам свойств Image (Id,Name и тд)(как обычные параметры)
        public ActionResult EditImage(Image a)
        {
            var img = Image.Get(a.Id);
            img.Edit(a);

            return RedirectToAction("Index");
        }



        //[HttpPost]
        public ActionResult FavoriteImage(int id)
        {
            var userId = ApplicationUser.GetUserId();
            bool setFav;
            if (userId == null)
            {
                var imgsFavC = HttpContext.Request.Cookies["FavoritedImgMass"]?.Value ?? "";
                string newC = "";

                Image.AddDelCookies(id.ToString(), imgsFavC, out newC, out setFav);
                HttpContext.Response.Cookies["FavoritedImgMass"].Value = newC;
                ViewBag.setLike = setFav;
                return RedirectToAction("Index");
            }
            var img = Image.Get(id);

            img.Favorite(out setFav, userId);
            ViewBag.setFav = setFav;


            
                return RedirectToAction("Index");
            


        }
        //[HttpPost]
        public ActionResult LikeImage(int id)
        {
            var userId = ApplicationUser.GetUserId();
            bool setLike;
            if (userId == null)
            {
                var imgsLikedC = HttpContext.Request.Cookies["LikedImgMass"]?.Value ?? "";
                string newC = "";
              
                Image.AddDelCookies(id.ToString(), imgsLikedC,out newC,out setLike);
                HttpContext.Response.Cookies["LikedImgMass"].Value = newC;
                ViewBag.setLike = setLike;
                return RedirectToAction("Index");
            }
            var img = Image.Get(id);
           
            img.Like(out setLike, userId);
            ViewBag.setLike = setLike;

            
            
            return RedirectToAction("Index");
        }


    }
}

