// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Review_Guard.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet("ip")]
        public IActionResult GetIp()
        {
            return Ok(new
            {
                RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Forwarded = HttpContext.Request.Headers["X-Forwarded-For"].ToString(),
                Headers = HttpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString())
            });
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
