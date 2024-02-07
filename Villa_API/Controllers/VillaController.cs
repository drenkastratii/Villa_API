using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Villa_API.Data;
using Villa_API.Models;
using Villa_API.Models.DTO;
using Villa_API.Repository.IRepository;

namespace Villa_API.Controllers
{
    [Route("api/Villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        public VillaController(IVillaRepository villaRepository, IMapper mapper)
        {
            _villaRepository = villaRepository;
            _mapper = mapper;
        }

        #region GetAll
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _villaRepository.GetAllAsync();
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

            var villa = await _villaRepository.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDTO>(villa));
        }
        #endregion

        #region CreateVilla
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            if (await _villaRepository.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Name should be unique!");
                return BadRequest(ModelState);
            }
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            Villa model = _mapper.Map<Villa>(createDTO);

            await _villaRepository.CreateAsync(model);

            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }
        #endregion

        #region DeleteVilla
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

            var villa = await _villaRepository.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            await _villaRepository.RemoveAsync(villa);

            return NoContent();
        }
        #endregion

        #region UpdateVilla
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

            await _villaRepository.UpdateAsync(model);
            return NoContent();
        }
        #endregion

        #region UpdatePartialVilla
        [HttpPatch("{id:int}",Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if(patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _villaRepository.GetAsync(u=>u.Id == id, tracked:false);
            VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaUpdateDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaUpdateDTO);

            await _villaRepository.UpdateAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return NoContent();
        }
        #endregion
    }
}
