using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Controllers
{
    //[Route("api/[Controller]")] // not define like this because whenever Controller name change then gets a problem
                                // so use static 
    [Route("api/villa")]
    [ApiController]
    public class VillaController:ControllerBase
    {
        private readonly ApplicationDbContext _db;
       public VillaController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]       
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            return Ok(await _db.Villa.ToListAsync());
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Villa>> GetVillaById(int id)
        {

            try
            {
                if (id<=0)
                {
                    return BadRequest("Villa Id must be greater than 0");

                }
                var villa = await _db.Villa.FirstOrDefaultAsync(u => u.Id == id);
                if(villa==null)
                {
                    return NotFound($"villa with id {id} was not found");
                }
                return Ok(villa);
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
        public async Task<ActionResult<Villa>> CreateVilla(VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest("villa data is required");
                }
                //var villa = new Villa()  this one is old syntax
                Villa villa = new() // new syntax both are correct
                {
                    Name = villaDTO.Name,
                    Details = villaDTO.Details,
                    Occupancy = villaDTO.Occupancy,
                    ImageUrl = villaDTO.ImageUrl,
                    Sqft = villaDTO.Sqft,
                    Rate = villaDTO.Rate,
                    CreatedDate = DateTime.Now
                };
                await _db.Villa.AddAsync(villa);
                await _db.SaveChangesAsync();
                return Ok(villa);
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

    }
}
