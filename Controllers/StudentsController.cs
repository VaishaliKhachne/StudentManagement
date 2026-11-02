
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using SMS_Application;
using SMS_Application.DTOs;
using SMS_Domain;


namespace StudentManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [RequiredScope("access_as_user")]
    public async Task<ActionResult<List<StudentDto>>> GetAll()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("{id:int}")]
    [RequiredScope("access_as_user")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);

        if (student is null)
            return NotFound();

        return Ok(student);
    }

    [HttpPost]
    [RequiredScope("access_as_user")]
    public async Task<ActionResult<StudentDto>> Create(
        StudentDto student)
    {
        var created = await _studentService.AddAsync(student);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.StudentId },
            created);
    }

    [HttpPut("{id:int}")]
    [RequiredScope("access_as_user")]
    public async Task<IActionResult> Update(
        int id, StudentDto student)
    {
        var updated = await _studentService.UpdateAsync(id, student);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [RequiredScope("access_as_user")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _studentService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("search")]
    [RequiredScope("access_as_user")]
    public async Task<IActionResult> Search(
        [FromQuery] int? studentId,
        [FromQuery] string? name,
        [FromQuery] string? email,
        [FromQuery] string? course
        )
    {
        if (studentId.HasValue && studentId.Value <= 0)
        {
            return BadRequest("Student ID must be greater than 0.");
        }

        var students = await _studentService.SearchAsync(
            studentId,
            name,
            email,
            course
            );

        return Ok(students);
    }
}
