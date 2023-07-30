using Domain;
using Domain.Solutions;
using Microsoft.AspNetCore.Mvc;
using PeterO.Numbers;
using Services.UserServices;
using System.Globalization;
using static Domain.Solutions.Question;


namespace TGS_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolutionController : ControllerBase
    {
        private readonly IAnswerService _answerService;

        public SolutionController(IAnswerService service)
        {
            _answerService = service;
        }

        public class UserInput
        {
            public string id { get; set; }
            public Dictionary<TypeQ, List<PairWrapper<string, List<string>>>> q { get; set; }
            public string name { get; set; }
            public bool isTrigo { get; set; }

            public List<PairWrapper<string, List<String>>> quest { get; set; }
            public string svg { get; set; }
        }

        public class StarAnswer
        {
            public string user_id { get; set; }
            public string answer_id { get; set; }

        }

        [HttpPost]
        public ActionResult<Answer> GetSolution([FromBody] UserInput info)
        {
            try
            {
                if (info.id != null)
                {
                    Question question = new Question(info.q);
                    HandleQuestion handleQuestion = new HandleQuestion(question, info.isTrigo);

                    var solution = handleQuestion.GetSolution();
                    // if the solution is null or of size 0
                    if (solution == null || solution.Count == 0)
                    {
                        return BadRequest();
                    }

                    Answer ans = new Answer();
                    ans.ClaimAndReason = solution;
                    ans.Time = GetTimeString();
                    ans.Name = info.name;
                    ans.Quest = info.quest;
                    ans.Svg = info.svg;

                    // return the answer
                    var answer_id = _answerService.SaveAnswer(info.id, ans);
                    ans.Id = answer_id;
                    return Ok(ans);
                }
            }
            catch
            {
                return BadRequest();
            }
            return BadRequest();

        }

        // http post request to star (save) answer
        [HttpPost("Star")]
        public ActionResult SaveAnswer([FromBody] StarAnswer sa)
        {
            if (sa.answer_id == null || sa.user_id == null)
            {
                return BadRequest();
            }
            // try to get the answer from the db
            var answer = _answerService.GetAnswer(sa.user_id, sa.answer_id);
            if (answer == null)
            {
                return BadRequest();
            }

            // star the answer
            _answerService.StarAnswer(sa.user_id, sa.answer_id);

            return Ok();
        }

        // remove star
        [HttpPost("RemoveStar")]
        public ActionResult<List<Answer>> RemoveStar([FromBody] StarAnswer sa)
        {
            if (sa.answer_id == null || sa.user_id == null)
            {
                return BadRequest();
            }
            // try to get the answer from the db
            var answer = _answerService.GetAnswer(sa.user_id, sa.answer_id);
            if (answer == null)
            {
                return BadRequest();
            }

            // remove star
            var new_list = _answerService.RemoveStar(sa.user_id, sa.answer_id);

            return Ok(new_list);
        }

        // remove answer
        [HttpPost("RemoveAnswer")]
        public ActionResult<List<Answer>> RemoveAnswer([FromBody] StarAnswer sa)
        {
            if (sa.answer_id == null || sa.user_id == null)
            {
                return BadRequest();
            }
            // try to get the answer from the db
            var answer = _answerService.GetAnswer(sa.user_id, sa.answer_id);
            if (answer == null)
            {
                return BadRequest();
            }

            // remove answer
            var new_list = _answerService.RemoveAnswer(sa.user_id, sa.answer_id);

            return Ok(new_list);
        }

        // get all history
        [HttpGet("GetHistory")]
        public ActionResult<List<Answer>> GetAllHistory(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var history = _answerService.GetAllAnswers(id);
            if (history == null)
            {
                return BadRequest();
            }

            return Ok(history);
        }


        // get starred history
        [HttpGet("GetStarred")]
        public ActionResult<List<Answer>> GetStarredHistory(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var starred = _answerService.GetSavedAnswers(id);
            if (starred == null)
            {
                return BadRequest();
            }

            return Ok(starred);
        }

        // get answer
        [HttpGet("GetAnswer")]
        public ActionResult<Answer> GetAnswer(string user_id, string answer_id)
        {
            var answer = _answerService.GetAnswer(user_id, answer_id);
            if (answer == null)
            {
                return BadRequest();
            }
            return Ok(answer);
        }
       
       


        private string GetTimeString()
        {
            DateTime now = DateTime.Now;
            CultureInfo hebrewCulture = new CultureInfo("he-IL");

            string formattedDate = now.ToString("dddd dd MMMM yyyy", hebrewCulture);
            string formattedTime = now.ToString("HH:mm", hebrewCulture);

            string result = $"{formattedDate} בשעה {formattedTime}";

            return result;
        }
    }
}

