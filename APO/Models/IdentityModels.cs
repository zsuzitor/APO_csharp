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


        public static string GetUserId()
        {
            return System.Web.HttpContext.Current.User.Identity.GetUserId();
        }

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
        public static ApplicationUser GetUser(string id, ApplicationDbContext db)
        {
            //string check_id = ApplicationUser.GetUserId();
            ApplicationUser res = null;
            if (string.IsNullOrWhiteSpace(id))
                return res;
            res = db.Users.FirstOrDefault(x1 => x1.Id == id);

            return res;
        }


        public  bool LoadLikedImages()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.ImagesLikes).IsLoaded)
                    db.Entry(this).Reference(x1 => x1.ImagesLikes).Load();
            }
                
            
            return true;
        }
        public bool LoadFavoritedImages()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                db.Set<ApplicationUser>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.ImagesFavorites).IsLoaded)
                    db.Entry(this).Reference(x1 => x1.ImagesFavorites).Load();
            }


            return true;
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