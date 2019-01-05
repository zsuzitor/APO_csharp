using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using APO.Models.DataBase;
using APO.Models.Domain;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace APO.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }


        //ICollection
        public List<Image> ImagesLikes { get; set; }
        public List<Image> ImagesFavorites { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public ApplicationUser():base()
        {
            Name = null;
            Surname = null;
            Birthday = DateTime.Now;


            ImagesLikes = new List<Image>();
            ImagesFavorites = new List<Image>();
        }

        /// <summary>
        /// получить id пользователя
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            return System.Web.HttpContext.Current.User.Identity.GetUserId();
        }
        /// <summary>
        ///  получить объект пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ApplicationUser GetUser(string id)
        {
            //string check_id = ApplicationUser.GetUserId();
            ApplicationUser res = null;
            //if (string.IsNullOrWhiteSpace(id))
            //    return res;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                res = ApplicationUser.GetUser(id, db);
            }

            return res;
        }
        /// <summary>
        /// получить объект пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static ApplicationUser GetUser(string id, ApplicationDbContext db)
        {
            //string check_id = ApplicationUser.GetUserId();
            ApplicationUser res = null;
            if (string.IsNullOrWhiteSpace(id))
                return res;
            res = db.Users.FirstOrDefault(x1 => x1.Id == id);

            return res;
        }

        /// <summary>
        /// загрузить все лайкнутые картинки
        /// </summary>
        /// <returns></returns>
        public  bool LoadLikedImages()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.ImagesLikes).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.ImagesLikes).Load();
            }
                
            
            return true;
        }
        /// <summary>
        /// загрузить все добавленные в избранное картинки
        /// </summary>
        /// <returns></returns>
        public bool LoadFavoritedImages()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.ImagesFavorites).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.ImagesFavorites).Load();
            }


            return true;
        }

        /// <summary>
        /// получить часть лайкнутых картинок
        /// </summary>
        /// <param name="Place"></param>
        /// <param name="Type"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
        public List<int> GetLikedImages(string[] Place, string[] Type, int startId = 0)
        {
            //this.LoadLikedImages();
            //SkipWhile

            //var query = user.ImagesLikes.Where(x1 => x1.Id > startId && !x1.Deleted);
            //if (Place.Length > 0)
            //    query.Where(x1 => x1.Place != null && Place.Contains(x1.Place));
            //if (Type.Length > 0)
            //    query.Where(x1 => x1.Type != null && Type.Contains(x1.Type));
            //mass = query.Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();

            int placeLength = Place.Length;
            int typeLength = Type.Length;


            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);



               return db.Entry(this).Collection(x1 => x1.ImagesLikes).Query().
               Where(x1 => x1.Id > startId && !x1.Deleted &&
               (placeLength > 0 ? Place.Contains(x1.Place) : true) &&
              (typeLength > 0 ? Type.Contains(x1.Type) : true)).
           Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();//


            }

            //return this.ImagesLikes.
            //    Where(x1 => x1.Id > startId && !x1.Deleted &&
            //    (placeLength > 0 ? Place.Contains(x1.Place) : true) &&
            //   (typeLength > 0 ? Type.Contains(x1.Type) : true)).
            //Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();



        }


        /// <summary>
        /// получить часть добавленных в избранное картинок
        /// </summary>
        /// <param name="Place"></param>
        /// <param name="Type"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
        public List<int> GetFavoritedImages(string[] Place, string[] Type, int startId = 0)
        {
            //this.LoadFavoritedImages();
            //SkipWhile

            //var query = user.ImagesFavorites.Where(x1 => x1.Id > startId && !x1.Deleted);
            //if (Place.Length > 0)
            //    query.Where(x1 => x1.Place != null && Place.Contains(x1.Place));
            //if (Type.Length > 0)
            //    query.Where(x1 => x1.Type != null && Type.Contains(x1.Type));
            //mass = query.Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();


            int placeLength = Place.Length;
            int typeLength = Type.Length;


            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);



                return db.Entry(this).Collection(x1 => x1.ImagesFavorites).Query().
               Where(x1 => x1.Id > startId && !x1.Deleted &&
               (placeLength > 0 ? Place.Contains(x1.Place) : true) &&
               (typeLength > 0 ? Type.Contains(x1.Type) : true)).
           Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();//


            }


            //return this.ImagesFavorites.
            //    Where(x1 => x1.Id > startId && !x1.Deleted &&
            //    (placeLength > 0 ? Place.Contains(x1.Place) : true) &&
            //    (typeLength > 0 ? Type.Contains(x1.Type) : true)).
            //Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();

        }



    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Image> Images { get; set; }




        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new AppDbInitializer());
        }

        //настраиваем связь многие ко многим
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.ImagesLikes)
                .WithMany(s => s.UsersLiked)
                .Map(t => t.MapLeftKey("ApplicationUserId")
                .MapRightKey("ImageId")
                .ToTable("ApplicationUserImageLikes"));

            modelBuilder.Entity<ApplicationUser>().HasMany(c => c.ImagesFavorites)
                .WithMany(s => s.UsersFavorited)
                .Map(t => t.MapLeftKey("ApplicationUserId")
                .MapRightKey("ImageId")
                .ToTable("ApplicationUserImageFavorites"));

            base.OnModelCreating(modelBuilder);
        }
    }
}