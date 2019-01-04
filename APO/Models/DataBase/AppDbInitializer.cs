using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web;

namespace APO.Models.DataBase
{
    public class AppDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext db)//название котекста
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            //доп инфа по ролям и созданию пользователей по тегу #roles
            var admin = new ApplicationUser { Email = "admin@mail.ru", UserName = "admin@mail.ru", Name = "zsuz", Surname = "zsuzSUR", Birthday = DateTime.Now };
            string password = "Admin1!";
            var result = userManager.Create(admin, password);


            base.Seed(db);
        }
    }


}
