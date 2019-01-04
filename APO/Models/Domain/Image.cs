using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace APO.Models.Domain
{
    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cords { get; set; }
        public bool Deleted { get; set; }
        //public string Path { get; set; }
        public string Creator { get; set; }

        //filter
        public string Place { get; set; }
        public string Type { get; set; }

        //ICollection
        public List<ApplicationUser> UsersLiked { get; set; }
        public List<ApplicationUser> UsersFavorited { get; set; }
        
        public Image()
        {
            Id = 0;
            Name = null;
            Description = null;
            Cords = null;
            Deleted = false;
            Creator= ApplicationUser.GetUserId();
            //Path = null;

            Place = null;
            Type = null;

            UsersLiked = new List<ApplicationUser>();
            UsersFavorited = new List<ApplicationUser>();
        }

        public Image(string cords):base()
        {
            Cords = cords;
        }
        public Image(string name, string description, string place, string type, string cords)
        {
            Id = 0;
            Name = name;
            Description = description;
            Cords = cords;
            Deleted = false;
            Creator = ApplicationUser.GetUserId();
            //Path = null;
            Place = place;
            Type = type;

            UsersLiked = new List<ApplicationUser>();
            UsersFavorited = new List<ApplicationUser>();
        }

        public static List<int> GetListId(string[] Place, string[] Type, int startId = 0)//int count,
        {
            List<int> res = new List<int>();
            
            if (Place == null)
                Place = new string[0];
            if (Type == null)
                Type = new string[0];
            int placeLength = Place.Length;
            int typeLength = Type.Length;
            using (var db = new ApplicationDbContext())
            {
                //var query= db.Images.
                //    Where(x1 => x1.Id > startId && !x1.Deleted );
                //if (Place.Length > 0)
                //    query.Where(x1 => x1.Place != null && Place.Contains(x1.Place));
                //if (Type.Length > 0)
                //    query.Where(x1 => x1.Type != null && Type.Contains(x1.Type));
                //res = query.Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();


                res = db.Images.
                    Where(x1 => x1.Id > startId && !x1.Deleted &&
                    (placeLength > 0 ? Place.Contains(x1.Place) : true) &&
                    (typeLength > 0 ? Type.Contains(x1.Type) : true)).
                Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();


            }

            return res;
        }




        public bool Favorite(out bool set, string userId)
        {

            using (var db = new ApplicationDbContext())
            {

                db.Set<Image>().Attach(this);
                if (!db.Entry(this).Collection(x1 => x1.UsersFavorited).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.UsersFavorited).Load();

                var likedObj = this.UsersFavorited.FirstOrDefault(x1 => x1.Id == userId);
                if (likedObj == null)
                {
                    this.UsersFavorited.Add(ApplicationUser.GetUser(userId, db));
                    set = true;
                }

                else
                {
                    this.UsersFavorited.Remove(likedObj);
                    set = false;
                }

                db.SaveChanges();
            }

            return true;
        }


        public bool Like(out bool set,string userId)
        {
           
            using (var db = new ApplicationDbContext())
            {
                
                db.Set<Image>().Attach(this);
                if (!db.Entry(this).Collection(x1=>x1.UsersLiked).IsLoaded)
                    db.Entry(this).Collection(x1 => x1.UsersLiked).Load();

                var likedObj = this.UsersLiked.FirstOrDefault(x1 => x1.Id == userId);
                if (likedObj == null)
                {
                    this.UsersLiked.Add(ApplicationUser.GetUser(userId, db));
                    set = true;
                }

                else
                {
                    this.UsersLiked.Remove(likedObj);
                    set = false;
                }
                    
                db.SaveChanges();
            }

            return true;
        }


        public bool Edit(Image a)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Set<Image>().Attach(this);
                this.Name = a.Name;
                this.Description = a.Description;
                this.Cords = a.Cords;
                this.Place = a.Place;
                this.Type= a.Type;
                db.SaveChanges();
            }

            return true;
            }


        
        public static List<int> GetFromCookies(string[] Place, string[] Type, string oldC, out string newC,int startId=0)//,string properties
        {
            //startId = startId > 0 ? startId : startId + 1;
           // startId ++;
            if (Place == null)
                Place = new string[0];
            if (Type == null)
                Type = new string[0];

            List<int> res = null;
            string startIdStr = startId.ToString();
            var oldCMass = oldC.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var oldCMassInt = oldCMass.Select(x1 => int.Parse(x1)).Where(x1 => x1 > startId);
            int placeLength = Place.Length;
            int typeLength = Type.Length;
            using (var db = new ApplicationDbContext())
            {

                //var query = db.Images.
                //    Where(x1 => oldCMassInt.Contains(x1.Id) && !x1.Deleted);
                //if (Place.Length > 0)
                //    query.Where(x1 => x1.Place != null && Place.Contains(x1.Place));
                //if (Type.Length > 0)
                //    query.Where(x1 => x1.Type != null && Type.Contains(x1.Type));
                //res = query.Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();




                res = db.Images.
                    Where(x1 => oldCMassInt.Contains(x1.Id) && !x1.Deleted &&
                    (placeLength > 0 ? Place.Contains(x1.Place) : true) &&
                    (typeLength > 0 ? Type.Contains(x1.Type) : true)).
                Take(Constants.CountLoadItem).Select(x1 => x1.Id).ToList();

            }
            newC = string.Join("@", res);
            return res;
        }


        public static bool AddDelCookies(string id,string oldC, out string newC,out bool set)//,string properties
        {
            var oldCMass = oldC.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (oldCMass.Contains(id))
            {
                oldCMass.Remove(id);
                set = false;
            }

            else
            {
                oldCMass.Add(id);
                set = true;
            }

            newC =string.Join("@", oldCMass);



            return true;
        }

        public static Image Get(int id)
        {
            Image res = null;
            using (var db = new ApplicationDbContext())
            {
                res=db.Images.FirstOrDefault(x1=>x1.Id==id);
            }
            return res;
        }

        public static bool Delete(int id)
        {
            string path = HostingEnvironment.MapPath($"~/Content/DbImages/" + id);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            using (var db = new ApplicationDbContext())
            {
                var img = db.Images.FirstOrDefault(x1 => x1.Id == id);
                img.Deleted = true;
                //if (!db.Entry(img).Collection(x1 => x1.UsersFavorited).IsLoaded)
                //    db.Entry(img).Collection(x1 => x1.UsersFavorited).Load();
                //if (!db.Entry(img).Collection(x1 => x1.UsersLiked).IsLoaded)
                //    db.Entry(img).Collection(x1 => x1.UsersLiked).Load();
                //img.UsersFavorited.RemoveAll(x1=>true);
                //img.UsersLiked.RemoveAll(x1 => true);
                db.SaveChanges();

            }

                return true;
        }

       

        public bool AddNew(HttpPostedFileBase uploadImage)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Images.Add(this);

                db.SaveChanges();
            }
            var bytes = Image.GetBytesFromUpload(uploadImage);
            if (bytes == null)
                return false;
            //List<System.Drawing.Image> mass = new List<System.Drawing.Image>() {
            //    Image.byteArrayToImage(bytes)
            //};
            //mass.Add(Image.ResizeImage(100, mass[0]));
            //mass.Add(Image.ResizeImage(50, mass[0]));
            Dictionary<string, System.Drawing.Image> mass = new Dictionary<string, System.Drawing.Image>();
            mass.Add("original", Image.byteArrayToImage(bytes));
            mass.Add("optimal", Image.ResizeImage(100, mass["original"]));
            mass.Add("min", Image.ResizeImage(50, mass["original"]));
            string path = HostingEnvironment.MapPath($"~/Content/DbImages/" + this.Id);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            foreach (var i in mass)
            {
                var imgB = Image.ImageToByteArray(i.Value, mass["original"].RawFormat);
               
                Directory.CreateDirectory(path);
                File.WriteAllBytes($@"{path}\\{i.Key}.jpeg", imgB);//{mass["original"].RawFormat}
            }
            
            
            return true;
        }


        //public static bool AddToDb(HttpPostedFileBase[] uploadImage)
        //{



        //    return true;
        //}

        //public bool GetFilesFromUpload(HttpPostedFileBase[] uploadImage,string filename)
        //{
        //    foreach(var i in uploadImage)
        //    {
        //        if (i != null && i.ContentLength > 0 && !string.IsNullOrEmpty(filename))
        //        {
        //            string fileName = filename;

        //            i.SaveAs(Path.Combine(HostingEnvironment.MapPath($"~/Content/DbImages/{i}.txt"), fileName));
        //        }
        //    }
        //    return true;

        //}


        public static byte[] GetBytesFromUpload(HttpPostedFileBase uploadImage)
        {
            byte[] res = null;
            if (uploadImage != null)
            {

                try
                {
                    byte[] imageData = null;
                    // считываем переданный файл в массив байтов
                    using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                    }
                    // установка массива байтов
                    res = imageData;
                }
                catch
                { }


            }

            return res;
        }
        
        private static System.Drawing.Image ResizeImage(int newSize, System.Drawing.Image originalImage)
        {
            if (originalImage.Width <= newSize)
                newSize = originalImage.Width;

            var newHeight = originalImage.Height * newSize / originalImage.Width;

            if (newHeight > newSize)
            {
                // Resize with height instead
                newSize = originalImage.Width * newSize / originalImage.Height;
                newHeight = newSize;
            }

            return originalImage.GetThumbnailImage(newSize, newHeight, null, IntPtr.Zero);
        }


        //image from byte
        public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            foreach(var i in returnImage.PropertyItems)
            returnImage.RemovePropertyItem(i.Id);

            return returnImage;
        }



        
        //baseFormat = (Image)imageIn.RawFormat
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn, ImageFormat baseFormat)
        {
            using (var ms = new MemoryStream())
            {
                //var encoder = GetEncoderInfo("image/jpg");
                imageIn.Save(ms, baseFormat);//System.Drawing.Imaging.ImageFormat.Jpeg 
                return ms.ToArray();
            }
        }


        public static byte[] FileToByteArray(string fileName)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }


    }
}



