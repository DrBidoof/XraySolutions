using ClincalWorkFlow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace ClincalWorkFlow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictController : ControllerBase
    {
        private readonly PredictionEngineService predictor;

        public PredictController(PredictionEngineService predictor)
        {
            this.predictor = predictor;
        }

        // Keep for server-side/Postman testing only
        [HttpPost]
        public IActionResult Predict([FromBody] ImageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.ImagePath) || !System.IO.File.Exists(request.ImagePath))
                return BadRequest("Image file not found on server.");

            var result = predictor.Predict(request.ImagePath);
            var minDistance = result.Score.Min();
            var isAnomaly = minDistance > 5.0f;

            return Ok(new
            {
                Cluster = result.PredictedClusterId,
                MinDistance = minDistance,
                IsAnomaly = isAnomaly
            });
        }

        // Frontend should use this one
        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        // [RequestSizeLimit(100_000_000)] // uncomment if needed (100 MB)
        public async Task<IActionResult> PredictUpload([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file received.");

            // create a temp file that preserves the original extension
            var ext = Path.GetExtension(image.FileName);
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{ext}");

            try
            {
                await using (var fs = System.IO.File.Create(tempPath))
                {
                    await image.CopyToAsync(fs);
                }

                var result = predictor.Predict(tempPath);
                var minDistance = result.Score.Min();
                var isAnomaly = minDistance > 5.0f;

                return Ok(new
                {
                    Cluster = result.PredictedClusterId,
                    MinDistance = minDistance,
                    IsAnomaly = isAnomaly
                });
            }
            finally
            {
                try { System.IO.File.Delete(tempPath); } catch { /* ignore */ }
            }
        }

        public class ImageRequest
        {
            public string ImagePath { get; set; }
        }
    }
}
