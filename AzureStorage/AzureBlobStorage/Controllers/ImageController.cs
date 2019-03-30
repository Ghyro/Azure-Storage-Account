using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureBlobStorage.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            this._imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        [HttpGet]
        public async Task<ActionResult> GetAllIFileList()
        {
            var result = await _imageService.GetAllImagesNameAsync();

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> GetFileByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var stream = await _imageService.GetImageAsync(name);

            if (stream == null)
            {
                return NotFound();
            }

            return File(stream, "image/jpg");
        }

        [HttpPost]
        public async Task<ActionResult> PostFile(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest();
            }

            using (var stream = file.OpenReadStream())
            {
                await _imageService.AddImageAsync(stream, file.FileName);

                return Ok(file.FileName);
            }
        }
    }
}