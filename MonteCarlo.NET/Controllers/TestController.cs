using Microsoft.AspNetCore.Mvc;

namespace MonteCarlo.NET.Controllers
{
    public class TestController : Controller
    {

        [HttpGet]
        [Route("/testError")]
        public void testError()
        {
            var x = 1;
            var y = 3 / (x - 1);
        }

    }
}
