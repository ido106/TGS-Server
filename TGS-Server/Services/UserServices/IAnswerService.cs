using Domain.Solutions;

namespace Services.UserServices
{
    public interface IAnswerService
    {
        // get all solutions
        List<Answer> GetAllAnswers(string id);

        // get saved solutions
        List<Answer> GetSavedAnswers(string id);

        string SaveAnswer(string id, Answer answer);

        void StarAnswer(string user_id, string answer_id);

        List<Answer> RemoveStar(string user_id, string answer_id);

        List<Answer> RemoveAnswer(string user_id, string answer_id);

        Answer GetAnswer(string user_id, string answer_id);
    }
}
