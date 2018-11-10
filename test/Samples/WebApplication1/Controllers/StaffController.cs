using System.ComponentModel.DataAnnotations;
using Alyio.AspNetCore.ApiMessages;
using Alyio.AspNetCore.ApiMessages.Filters;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class StaffController : Controller
    {
        [ApiMessageFilter]
        [HttpPost]
        public void Post([FromBody]Staff staff)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestMessage(ModelState);
            }
        }

        [HttpPut]
        public void Put([FromBody]Staff staff)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestMessage(ModelState);
            }
        }
    }

    public class Staff
    {
        [Required]
        [MaxLength(2)]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Range(10, 30)]
        public int Age { get; set; }
    }
}
