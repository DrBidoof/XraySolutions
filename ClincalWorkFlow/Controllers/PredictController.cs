using ClincalWorkFlow.Services;
using Microsoft.AspNetCore.Mvc;

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

        public class ImageRequest
        {
            public string ImagePath { get; set; }
        }
    }
}
