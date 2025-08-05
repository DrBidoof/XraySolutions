using ClincalWorkFlow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

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

        [HttpPost]
        public IActionResult Predict([FromBody] ImageRequest request)
        {
            if (!System.IO.File.Exists(request.ImagePath))
                return BadRequest("Image file not found.");

            var result = predictor.Predict(request.ImagePath);
            var minDistance = result.Score.Min();
            var isAnomaly = minDistance > 5.0f; // you define this threshold

            return Ok(new
            {
                Cluster = result.PredictedClusterId,
                MinDistance = minDistance,
                IsAnomaly = isAnomaly
            });
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> PredictUpload([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file received.");

            var tempPath = Path.GetTempFileName();
            using (var stream = System.IO.File.Create(tempPath))
            {
                await image.CopyToAsync(stream);
            }

            var result = predictor.Predict(tempPath);
            System.IO.File.Delete(tempPath);

            var minDistance = result.Score.Min();
            var isAnomaly = minDistance > 5.0f;

            return Ok(new {
                Cluster = result.PredictedClusterId,
                MinDistance = minDistance,
                IsAnomaly = isAnomaly
            });
        }

        public class ImageRequest
        {
            public string ImagePath { get; set; }
        }
    }
}
