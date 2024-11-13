
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using webapic_.Data;


namespace CRUDWithEFDbFirstWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MyAppDbContext _context;
        public ProductController(MyAppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var products = _context.Products.ToList();
                
                /*if (products == 0)
                {
                    return NotFound("No Found data");
                }*/
                return Ok(products);
            }
            catch (Exception ex) { 
            
                return BadRequest(ex.Message);      
            }

            }   
    }


}

