using Microsoft.AspNetCore.Mvc;
using FamilyNest.Services;
using System.Threading.Tasks;

namespace FamilyNest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FamiliesController : ControllerBase
{
    private readonly SupabaseService _supabaseService;

    public FamiliesController(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    // Получить все семьи
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var families = await _supabaseService.GetAllFamiliesAsync();
        return Ok(families);
    }

    // Получить семью по id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var family = await _supabaseService.GetFamilyByIdAsync(id);
        if (family == null)
            return NotFound($"Семья с id {id} не найдена");

        return Ok(family);
    }

    // Добавить новую семью
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Название семьи не может быть пустым");

        var success = await _supabaseService.AddFamilyAsync(name);
        if (!success)
            return StatusCode(500, "Ошибка при добавлении семьи");

        return Ok("Семья успешно добавлена");
    }

    // Обновить название семьи
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Название семьи не может быть пустым");

        var success = await _supabaseService.UpdateFamilyAsync(id, name);
        if (!success)
            return NotFound($"Семья с id {id} не найдена");

        return Ok("Семья успешно обновлена");
    }

    // Удалить семью
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _supabaseService.DeleteFamilyAsync(id);
        if (!success)
            return NotFound($"Семья с id {id} не найдена");

        return Ok("Семья успешно удалена");
    }
}
