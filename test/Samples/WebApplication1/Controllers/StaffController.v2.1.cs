using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/v2_1/[controller]")]
    [ApiController]
    public class StaffControllerv2_1 : ControllerBase
    {
        [HttpPost]
        public void Post([FromBody]Staff staff)
        {
        }

        [HttpPut]
        public void Put([FromBody]Staff staff)
        {
        }
    }
}
