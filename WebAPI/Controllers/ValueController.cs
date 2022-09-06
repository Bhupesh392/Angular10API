using Microsoft.AspNetCore.Mvc;
using WebAPI.Interface;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoFacController : ControllerBase
    {
        private readonly IDataProvider dataProvider;

        public AutoFacController(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        // GET: /<controller>/
        [HttpGet]
        public string Get()
        {
            return dataProvider.Get();
        }
    }
}
