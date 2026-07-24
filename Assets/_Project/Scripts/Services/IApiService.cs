using System.Threading;
using System.Threading.Tasks;

public interface IApiService
{
    Task<LeaderboardPage> GetLeaderBoard(int page, CancellationToken token);
    Task<QuestionData> GetQuestions(CancellationToken token);
}
