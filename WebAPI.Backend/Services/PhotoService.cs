using Microsoft.AspNetCore.Hosting;
using WebAPI.Backend.Interfaces;
using WebAPI.Backend.Model;

namespace WebAPI.Backend.Services
{
    public class PhotoService : IPhotoService
    {
       
        public async Task Delete(string photoUrl)
        {           
                   
            if (File.Exists(photoUrl))
            {
                using (var stream = new FileStream(photoUrl, FileMode.Open, FileAccess.Read, FileShare.Delete))
                {
                    File.Delete(photoUrl);
                }
            }
            else
            {               
                throw new FileNotFoundException("Файл не найден", photoUrl);
            }
        }
        public async Task GetPhotos(IWebHostEnvironment webHost, List<PhotoInfo> photos)
        {        
            var filesPath = Path.Combine(webHost.ContentRootPath, "PhotoFolder");
            var files = Directory.GetFiles(filesPath);
            foreach (var file in files)
            {
                PhotoInfo photo = new PhotoInfo();
                photo.PhotoUrl = file;
                photo.Description = Path.GetFileNameWithoutExtension(file);
                photos.Add(photo);
            }          
        }

        public async Task Save(IFormFile file, IWebHostEnvironment webHostEnvironment)
        {
            var filePath = Path.Combine(webHostEnvironment.ContentRootPath, "PhotoFolder");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                File.WriteAllBytes(Path.Combine(filePath, file.FileName), memoryStream.ToArray());
            }
        }

    }
}
