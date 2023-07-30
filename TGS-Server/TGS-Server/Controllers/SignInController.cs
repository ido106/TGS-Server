using Microsoft.AspNetCore.Mvc;
using Services.UserServices;
using System.Net.Mail;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TGS_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        // key:string email, value: Pair of 4 digit pin and Time
        private static Dictionary<string, Tuple<string, DateTime>> _pins = new Dictionary<string, Tuple<string, DateTime>>();
        // login service
        private readonly ILoginService _loginService;

        public SignInController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Pin { get; set; }
        }

        // POST api/<SignInController>
        [HttpPost]
        public ActionResult<string> VerifyEmail([FromBody] string email)
        {
            try
            {
                const string from = "GeomeTrygo.Solver@outlook.com";

                MailMessage msg = new MailMessage();
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                msg.From = new MailAddress(from);
                msg.To.Add(email);

                msg.Subject = "GeomeTrygo Solver 4 digit password";

                // generate 4 digit password
                Random random = new Random();
                int digit1 = random.Next(0, 10);
                int digit2 = random.Next(0, 10);
                int digit3 = random.Next(0, 10);
                int digit4 = random.Next(0, 10);
                // make First string of the 4 digit password
                string pin = digit1.ToString() + digit2.ToString() + digit3.ToString() + digit4.ToString();

                msg.Body = "Your 4 digit pin is: " + pin + ". This pin will expire in 5 minutes.";

                // if email already exists in _pins, remove it
                if (_pins.ContainsKey(email))
                {
                    _pins.Remove(email);
                }

                // add to _pins
                _pins.Add(email, new Tuple<string, DateTime>(pin, DateTime.Now));

                client.Port = 587;
                client.Credentials = new System.Net.NetworkCredential(from, "SI2023TGS");
                client.EnableSsl = true;
                client.Send(msg);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<SignInController>/login
        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] LoginRequest request)
        {
            // check if email exists in _pins
            if (!_pins.ContainsKey(request.Email))
            {
                return BadRequest("Email not found");
            }

            // check if pin is correct
            if (!_pins[request.Email].Item1.Equals(request.Pin))
            {
                return BadRequest("Wrong or expired pin");
            }

            // check if pin has expired
            if (_pins[request.Email].Item2.AddMinutes(5) < DateTime.Now)
            {
                // remove from _pins
                _pins.Remove(request.Email);

                // invalid or expired pin
                return BadRequest("Wrong or expired pin");
            }

            string id = _loginService.HandleLogin(request.Email);
            return Ok(id);
        }
    }
}
