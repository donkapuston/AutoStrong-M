using Microsoft.AspNetCore.Mvc;
using WebAPI.Backend.Interfaces;
using WebAPI.Backend.Model;

namespace WebAPI.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly ILogger<PhotoController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPhotoService _photo;      

        public PhotoController(ILogger<PhotoController> logger, IWebHostEnvironment webHostEnvironment, IPhotoService uploadFile)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _photo = uploadFile;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var httpContent = HttpContext.Request;
                if (httpContent is null)
                    return BadRequest();

                if (httpContent.Form.Files.Count>0)
                {
                    foreach(var file in httpContent.Form.Files)
                    {                      
                        await _photo.Save(file,_webHostEnvironment);
                    }
                    return Ok(httpContent.Form.Files.Count.ToString() + " файлов загружено");
                }
                else
                {
                    return BadRequest("Файлы не выбраны");
                }                
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task <IActionResult> Get()
        {
            try
            {
                List<PhotoInfo> photos = new List<PhotoInfo>();
                await _photo.GetPhotos(_webHostEnvironment, photos);              
                return Ok(photos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }
        [HttpDelete]
        public async  Task<IActionResult> Delete([FromBody] string photoUrl)
        {
            try
            {
                await _photo.Delete(photoUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(); ;
            }
        }
    }
}
