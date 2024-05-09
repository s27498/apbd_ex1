
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers;
// obyczaj
[ApiController]
[Route("api/[controller]")]

public class AnimalController : ControllerBase
{
    private readonly AnimalRepository _animalRepository;
    public AnimalController(AnimalRepository animalRepository)
    {
        _animalRepository = animalRepository;
    }

    [HttpGet]
    [Route("{id}")]
    
    public async Task<IActionResult> GetAnimal(int id)
    {
        if (!await _animalRepository.doesAnimalExist(id))
        {
            return NotFound("Animal with given id does not exist");
        }
        var animal = await _animalRepository.getAnimal(id);
        return Ok(animal);
    }
    [HttpPost]
    //[Route("{id}")]
    public async Task<IActionResult> AddAnimal(AnimalWithProcedures animalWithProcedures)
    {
        if (!await _animalRepository.doesOwnerExist(animalWithProcedures.OwnerID))
        {
            return NotFound("Owner with given id does not exist");
        }

        foreach (var VARIABLE in animalWithProcedures.Procedures)
        {
            if (!await _animalRepository.doesProcedureExist(VARIABLE.ProcedureID))
            {
                return NotFound("Procedure with given id does not exist");
            }
        }
        await _animalRepository.addAnimalWithProcedure(animalWithProcedures);
        return Created();

    }
}