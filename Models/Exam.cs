namespace backend_assignment.Models;

public class Exam
{
    public int Id { get; set; }
    public int QuestionPaperId { get; set; }
    public QuestionPaper QuestionPaper { get; set; } = null!;
    public Response[] Responses { get; set; } = null!;
}