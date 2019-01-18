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
            //using (var db=new ApplicationDbContext())
            //{
            //    ViewBag.idImage = db.Images.FirstOrDefault().Id;
            //}
            //var userId = ApplicationUser.GetUserId();
            //if (userId == null)
            //{

            //}
            //else
            //{
            //    var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
            //    user.LoadLikedImages();

            //    using (var db = new ApplicationDbContext())
            //    {

            //    }
            //}
            //Response.StatusCode = 500;

            //Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //return Json(new { jobId = -1 });
            //HttpContext.Request.Cookies["FavoritedImgMass"].Value = " ";
            //HttpContext.Request.Cookies["LikedImgMass"].Value = " ";

            return View();
        }



        //
        /// <summary>
        /// подгрузка списка картинок
        /// </summary>
        /// <param name="Place -массив фильтров"></param>
        /// <param name="Type -массив фильтров"></param>
        /// <param name="startId id последней загруженной картинки"></param>
        /// <returns></returns>
        public ActionResult ListImages(string[] Place, string[] Type, int startId = 0)//int count,
        {
            try
            {
                var mass = Image.GetListId(Place, Type, startId);//count,
                ViewBag.lastImageId = mass.LastOrDefault();
                ViewBag.massImages = mass;
                //return RedirectToRoute(new { action = "Index", controller= "Home" });
                //return RedirectToAction("Index","Home",new { });

               
                    }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }


            return PartialView();
        }

        //
        /// <summary>
        /// подгрузка лайкнутых пользователем картинок
        /// </summary>
        /// <param name="Place -массив фильтров"></param>
        /// <param name="Type -массив фильтров"></param>
        /// <param name="startId  id последней загруженной картинки"></param>
        /// <returns></returns>
        public ActionResult ListLikedImages(string[] Place, string[] Type, int startId = 0)//int count,
        {
            try
            {
                var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());

                if (Place == null)
                    Place = new string[0];
                if (Type == null)
                    Type = new string[0];

                List<int> mass = new List<int>();
                if (user == null)
                {
                    if (HttpContext.Request.Cookies["LikedImgMass"] != null)
                    {
                        var imgslikeC = HttpContext.Request.Cookies["LikedImgMass"].Value ?? "";
                        string newFavStr;
                        mass = Image.GetFromCookies(Place, Type, imgslikeC, out newFavStr, startId);
                        HttpContext.Request.Cookies["LikedImgMass"].Value = newFavStr;
                    }
                }
                else
                {
                    mass = user.GetLikedImages(Place, Type, startId);
                    //mass = user.ImagesFavorites.Select(x1 => x1.Id).ToList();

                }

                ViewBag.lastImageLikedId = mass.LastOrDefault();
                ViewBag.massImagesLiked = mass;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }


            return PartialView();
        }

        //
        /// <summary>
        /// подгрузка добавленных в избранное пользователем картинок
        /// </summary>
        /// <param name="Place -массив фильтров"></param>
        /// <param name="Type -массив фильтров"></param>
        /// <param name="startId  id последней загруженной картинки"></param>
        /// <returns></returns>
        public ActionResult ListFavoriteImages(string[] Place, string[] Type, int startId = 0)//int count,
        {
            try
            {
                if (Place == null)
                    Place = new string[0];
                if (Type == null)
                    Type = new string[0];

                var user = ApplicationUser.GetUser(ApplicationUser.GetUserId());
                List<int> mass = new List<int>();
                if (user == null)
                {
                    if (HttpContext.Request.Cookies["FavoritedImgMass"] != null)
                    {
                        var imgsFavC = HttpContext.Request.Cookies["FavoritedImgMass"].Value ?? "";
                        string newFavStr;
                        mass = Image.GetFromCookies(Place, Type, imgsFavC, out newFavStr, startId);

                        HttpContext.Request.Cookies["FavoritedImgMass"].Value = newFavStr;
                    }

                }
                else
                {
                    mass = user.GetFavoritedImages(Place, Type, startId);
                    //mass = user.ImagesFavorites.Select(x1=>x1.Id).ToList();

                }


                ViewBag.lastImageFavoritedId = mass.LastOrDefault();
                ViewBag.massImagesFavorited = mass;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }


            return PartialView();
        }


        //
        /// <summary>
        /// удаление картинки
        /// </summary>
        /// <param name="id id картинки для удаления"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult DeleteImage(int id)
        {
            try
            {
                Image.Delete(id);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }


            return RedirectToAction("Index");
        }


        //
        /// <summary>
        /// добавление пака картинок
        /// 
        /// </summary>
        /// <param name="file файл с координатами (разделитель- перевод строки('\n'))"></param>
        /// <param name="uploadImage  картинки"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult AddImages(HttpPostedFileBase file, HttpPostedFileBase[] uploadImage)
        {
            try
            {
                var massCords = new StreamReader(file.InputStream).ReadToEnd().Split('\n');


                for (int i = 0; i < (massCords.Length < uploadImage.Length ? massCords.Length : uploadImage.Length); ++i)
                {
                    var img = new Image(massCords[i].Trim());
                    bool sc = img.AddNew(uploadImage[i]);
                    if (!sc)
                    {
                        Image.Delete(img.Id);
                        throw new Exception("проблемы с файлом");
                    }

                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }



            return RedirectToAction("Index");
        }


        /// <summary>
        /// добавление картинки
        /// </summary>
        /// <param name="a -свойства для создания объекта"></param>
        /// <param name="uploadImage -картинка"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult AddImage(Image a, HttpPostedFileBase uploadImage)//string name, string description, string cords, HttpPostedFileBase uploadImage
        {
            try
            {
                var img = new Image(a.Name, a.Description, a.Place, a.Type, a.Cords);
                bool sc = img.AddNew(uploadImage);
                if (!sc)
                {
                    Image.Delete(img.Id);
                    throw new Exception("проблемы с файлом");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }



            return RedirectToAction("Index");
        }


        /// <summary>
        /// редактирование картинки
        /// </summary>
        /// <param name="a -свойства для редактирования объекта"></param>
        /// <returns></returns>
        //для связывания параметров передавать их по именам свойств Image (Id,Name и тд)(как обычные параметры)
        public ActionResult EditImage(Image a)
        {
            try
            {
                var img = Image.Get(a.Id);
                //if (img==null||img.Deleted)
                //    throw new Exception("картинка не найдена");
                img.Edit(a);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }



            return RedirectToAction("Index");
        }



        /// <summary>
        /// добавление картинки в избранное
        /// </summary>
        /// <param name="id- id картинки для добавления в избранное"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult FavoriteImage(int id)
        {
            try
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
                //if (img == null||img.Deleted)
                //    throw new Exception("картинка не найдена");
                img.Favorite(out setFav, userId);
                ViewBag.setFav = setFav;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }


            return RedirectToAction("Index");

        }


        /// <summary>
        /// лайк картинки
        /// </summary>
        /// <param name="id -id картинки для добавления в лайкнутые"></param>
        /// <returns></returns>
        //[HttpPost]
        public ActionResult LikeImage(int id)
        {
            try
            {
                var userId = ApplicationUser.GetUserId();
                bool setLike;
                if (userId == null)
                {
                    var imgsLikedC = HttpContext.Request.Cookies["LikedImgMass"]?.Value ?? "";
                    string newC = "";

                    Image.AddDelCookies(id.ToString(), imgsLikedC, out newC, out setLike);
                    HttpContext.Response.Cookies["LikedImgMass"].Value = newC;
                    ViewBag.setLike = setLike;
                    return RedirectToAction("Index");
                }
                var img = Image.Get(id);
                //if (img == null || img.Deleted)
                //    throw new Exception("картинка не найдена");
                img.Like(out setLike, userId);
                ViewBag.setLike = setLike;
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }


            return RedirectToAction("Index");
        }




        //корзина


        /// <summary>
        /// возвращает id img которые находятся в корзине
        /// </summary>
        /// <returns></returns>
        public ActionResult BasketImages()
        {
            try
            {
                ViewBag.BasketImages = Image.LoadBasket();
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// очищает корзину, не удаляя картинки
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearBasketImages()
        {
            try
            {
                Image.ClearBasket();
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// удаляет все картинки находящиеся в корзине
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteAllBasketImages()
        {
            try
            {
                Image.DeleteAllFromBasket();
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// добавляет в корзину
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddToBasketImages(int id)
        {
            try
            {
                var img = Image.Get(id);
                //if (img == null || img.Deleted)
                //    throw new Exception("картинка не найдена");
                bool er = img.AddToBasket();
                if (!er)
                    throw new Exception("картинка уже удалена");
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// удалить из корзины
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveBasketImage(int id)
        {
            try
            {
                var img = Image.Get(id);
                //if (img == null || img.Deleted)
                //    throw new Exception("картинка не найдена");
                img.DeleteFromBasket();
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new { errorText = e.Message, inner = e.InnerException?.Message });
            }
            return RedirectToAction("Index");
        }





    }
}

