using WebAPI.Backend.Model;

namespace WebAPI.Backend.Interfaces
{
    public interface IPhotoService
    {
        public Task Save(IFormFile file, IWebHostEnvironment webHostEnvironment);
        public Task Delete (string photoUrl);
        public Task GetPhotos(IWebHostEnvironment env, List<PhotoInfo> photos);
    }
}
