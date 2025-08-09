using XrayAPI.Options;           // <— add this
using ClincalWorkFlow.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;       // <— add this
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ClincalWorkFlow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictController : ControllerBase
    {
        private readonly PredictionEngineService _predictor;
        private readonly double _threshold;   // from appsettings.json

        public PredictController(PredictionEngineService predictor, IOptions<AnomalyOptions> opts)
        {
            _predictor = predictor;
            _threshold = opts.Value.MinDistanceThreshold; // e.g., 2.0e8
        }

        // Keep for Postman/server testing only
        [HttpPost]
        public IActionResult Predict([FromBody] ImageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.ImagePath) || !System.IO.File.Exists(request.ImagePath))
                return BadRequest("Image file not found on server.");

            var result = _predictor.Predict(request.ImagePath);
            var minDistance = (double)result.Score.Min();
            var isAnomaly = minDistance > _threshold;

            // camelCase keys to match frontend
            return Ok(new
            {
                cluster = result.PredictedClusterId,
                minDistance,
                isAnomaly
            });
        }

        // Frontend should use this one
        [HttpPost("Upload")]
        [Consumes("multipart/form-data")]
        // [RequestSizeLimit(100_000_000)] // uncomment if needed
        public async Task<IActionResult> PredictUpload([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No file received.");

            var ext = Path.GetExtension(image.FileName);
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{ext}");

            try
            {
                await using (var fs = System.IO.File.Create(tempPath))
                    await image.CopyToAsync(fs);

                var result = _predictor.Predict(tempPath);
                var minDistance = (double)result.Score.Min();
                var isAnomaly = minDistance > _threshold;

                return Ok(new
                {
                    cluster = result.PredictedClusterId,
                    minDistance,
                    isAnomaly
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
