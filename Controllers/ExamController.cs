using backend_assignment.Data;
using backend_assignment.Dtos;
using backend_assignment.Mappers;
using backend_assignment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_assignment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExamController(AppDbContext context) : ControllerBase
{
  private readonly AppDbContext _context = context;

  // GET: api/Exam
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Exam>>> GetExams()
  {
    return await _context.Exams.ToListAsync();
  }

  // GET: api/Exam/5
  [HttpGet("latest")]
  public async Task<ActionResult<ExamDto>> GetExam()
  {
    var exam = await _context.Exams.Include(e => e.QuestionPaper).OrderBy(e => e.Id).LastAsync();

    if (exam == null)
    {
      return NotFound();
    }
    return exam.ToExamDto();
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ExamDto>> GetExam(int id)
  {
    var exam = await _context
      .Exams.Include(e => e.QuestionPaper)
      .FirstOrDefaultAsync(e => e.Id == id);

    if (exam == null)
    {
      return NotFound();
    }
    return exam.ToExamDto();
  }

  private static Tuple<int, int> ScoreAndMax(Response response, QuestionPaper paper)
  {
    int score = 0;
    if (response.Q1AnswerId == paper.Q1.CorrectAnswerId)
    {
      ++score;
    }
    if (response.Q2AnswerId == paper.Q2.CorrectAnswerId)
    {
      ++score;
    }
    if (response.Q3AnswerId == paper.Q3.CorrectAnswerId)
    {
      ++score;
    }
    if (response.Q4AnswerId == paper.Q4.CorrectAnswerId)
    {
      ++score;
    }
    if (response.Q5AnswerId == paper.Q5.CorrectAnswerId)
    {
      ++score;
    }
    int max = 5;
    return new Tuple<int, int>(score, max);
  }

  [HttpPost("latest/responses/{id}")]
  public async Task<ActionResult<Tuple<int, int>>> SubmitResponse(int id)
  {
    var latestExam = await _context
      .Exams.Include(e => e.QuestionPaper)
      .Include(e => e.Responses)
      .OrderBy(e => e.Id)
      .LastAsync();
    if (latestExam == null)
    {
      return NotFound();
    }
    var targetResponse = await _context.Responses.FirstOrDefaultAsync(r => r.Id == id);
    _ = latestExam.Responses.Append(new() { Id = id });
    if (targetResponse == null)
    {
      return NotFound();
    }
    return ScoreAndMax(targetResponse, latestExam.QuestionPaper);
  }
}
