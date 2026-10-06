#if DEBUG
using Microsoft.AspNetCore.Mvc;

namespace MonteCarlo.NET.Controllers
{
    // Throws on purpose so the exception health check can be demoed. Compiled out of Release builds.
    public class TestController : Controller
    {
        [HttpGet]
        [Route("/testError")]
        public IActionResult ThrowTestError()
        {
            throw new InvalidOperationException("Test exception for the health check demo.");
        }
    }
}
#endif
