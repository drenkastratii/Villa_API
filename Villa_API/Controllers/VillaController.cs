using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Villa_API.Data;
using Villa_API.Models;
using Villa_API.Models.DTO;

namespace Villa_API.Controllers
{
    [Route("api/Villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        #region GetAll
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }
        #endregion

        #region GetVillaById
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVillaById(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDTO>(villa));
        }
        #endregion

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Name should be unique!");
                return BadRequest(ModelState);
            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            Villa model = _mapper.Map<Villa>(createDTO);

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            if(updateDTO == null || updateDTO.Id != id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDTO);

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}",Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if(patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaUpdateDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaUpdateDTO);

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
