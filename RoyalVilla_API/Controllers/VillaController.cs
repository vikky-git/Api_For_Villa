using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RoyalVilla_API.Controllers
{
    //[Route("api/[Controller]")] // not define like this because whenever Controller name change then gets a problem
    // so use static 
    [Route("api/villa")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper; // here we inject the mapper
        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            var villas = await _db.Villa.ToListAsync();

            //return Ok(await _db.Villa.ToListAsync());
            return Ok (_mapper.Map<List<VillaDTO>>(villas));
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<VillaDTO>>> GetVillaById(int id)
        {

            try
            {
                if (id <= 0)
                {
                    return new ApiResponse<VillaDTO>()
                    {
                        StatusCode = 400,
                        Error = "Villa Id must be greater than 0",
                        Success = false,
                        Message = "Bad Request"
                    };
                    //return BadRequest("Villa Id must be greater than 0");

                }
                var villa = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);
                if (villa == null)
                {
                    return new ApiResponse<VillaDTO>(){
                        StatusCode = 404,
                        Error = $"villa with id {id} was not found",
                        Success = false,
                        Message = "Not Found"
                    };
                   // return NotFound($"villa with id {id} was not found");
                }
                // return Ok(villa);
                return new ApiResponse<VillaDTO>()
                {
                    StatusCode = 200,
                    Success = true,
                    Message = "Records retrieved Successfully",
                    Data = _mapper.Map<VillaDTO>(villa)
                };
               // return Ok(_mapper.Map<VillaDTO>(villa));
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while retrieving villa with ID {id}:{ex.Message}");
            }
        }
        #region without Dto to create Villas
        //[HttpPost]
        //public async Task<ActionResult<Villa>> CreateVilla(Villa villa)
        //{
        //    try
        //    {
        //        if (villa==null)
        //        {
        //            return BadRequest("villa data is required");
        //        }
        //        await _db.Villa.AddAsync(villa);
        //        await _db.SaveChangesAsync();
        //        return Ok(villa);
        //    }
        //    catch (Exception ex)
        //    {

        //        return StatusCode(StatusCodes.Status500InternalServerError,
        //            $"An error occured while creating the  villa:{ex.Message}");
        //    }
        //}
        #endregion

        #region using Dto to create Villas
        [HttpPost]
        //HERE WE EXPOSE VILLA Entity/Class which is the bad practices
        //So Make DTO CLASS -->VillaDTO
        public async Task<ActionResult<VillaDTO>> CreateVilla(VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest("villa data is required");
                }
                //var villa = new Villa()  this one is old syntax

                //Villa villa = new() // new syntax both are correct
                //{
                //    Name = villaDTO.Name,
                //    Details = villaDTO.Details,
                //    Occupancy = villaDTO.Occupancy,
                //    ImageUrl = villaDTO.ImageUrl,
                //    Sqft = villaDTO.Sqft,
                //    Rate = villaDTO.Rate,
                //    CreatedDate = DateTime.Now
                //};


                //Here we need to Validate the Vilas for duplicate
                var duplicatevilla = await _db.Villa.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower());

                if (duplicatevilla != null)
                {
                    return Conflict($"A Villa with the name '{villaDTO.Name}' already exists");// status code 409
                }


                //below line of use automapper  so don't need to map each object
                Villa villa = _mapper.Map<Villa>(villaDTO);
                await _db.Villa.AddAsync(villa);
                await _db.SaveChangesAsync();
                //return Ok(villa);
                /*instead of returning ok which return status code:200
                 * but we want status code 201 so use Createdataction method
                */
                //return CreatedAtAction(nameof(CreateVilla), new { id = villa.Id });// this one only return Id


                //return CreatedAtAction(nameof(CreateVilla), new { id = villa.Id }, villa);// this one return complete villa object
                return CreatedAtAction(nameof(CreateVilla), new { id = villa.Id }, _mapper.Map<VillaDTO>(villa));


            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while creating the  villa:{ex.Message}");
            }
        }
        #endregion
        #region old code
        // [HttpGet("{id:int}/{name:string}")] //The constraint reference 'string' could not be resolved to a type. Register the constraint type with
        //[HttpGet("{id:int}/{name}")]
        //public string GetVillaByIdAndName(int id,string name)
        //{
        //    return "Get Villa Id:" + id + ":" + name;
        //}

        //[HttpGet("{id:int}/{name}")]
        //public string GetVillaByIdAndName([FromRoute] int id, [FromRoute] string name)
        //{
        //    return "Get Villa Id:" + id + ":" + name;
        //}

        //  [HttpGet("{id:int}/{name}")] // we get an error
        //[HttpGet]
        //public string GetVillaByIdAndName([FromQuery] int id, [FromQuery] string name)
        //{
        //    return "Get Villa Id:" + id + ":" + name;
        //}
        #endregion

        [HttpPut("{id:int}")]
        public async Task<ActionResult<VillaUpdateDTO>>UpdateVilla(int id,VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO==null)
                {
                    return BadRequest("villa data is required");

                }
                if (id!=villaDTO.Id)
                {
                    return BadRequest("Villa ID in URL does not match villa ID in request body");
                }
                var existingvilla = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);
                if (existingvilla==null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                }

                //Here we need to Validate the Vilas for duplicate
                var duplicatevilla=await _db.Villa.FirstOrDefaultAsync(u=>u.Name.ToLower()== villaDTO.Name.ToLower() 
                && u.Id!=id);

                if (duplicatevilla!=null)
                {
                    return Conflict($"A Villa with the name '{villaDTO.Name}' already exists");// status code 409
                }
                _mapper.Map(villaDTO, existingvilla);//use existing object "existingvilla"
               

                existingvilla.UpdatedDate = DateTime.Now;
                await _db.SaveChangesAsync();
                return Ok(villaDTO);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while updating the villa:{ex.Message}");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            try
            {
                
                var existingvilla = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);
                if (existingvilla == null)
                {
                    return NotFound($"Villa with ID {id} was not found");
                }
                _db.Villa.Remove(existingvilla);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An error occured while deleting the villa:{ex.Message}");
            }
        }
    }
}
