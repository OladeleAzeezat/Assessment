using AssessmentApi.Data;
using AssessmentApi.Models;
using AssessmentApi.POCO;
using AssessmentApi.TableDTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssessmentApi.Controllers
{
    //[Authorize]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AssessmentDbContext _context;
        private readonly IConfiguration _configuration;

        public ItemController(AssessmentDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;

        }
        /// <summary>
        /// gets a list of item records
        /// </summary>
        [Route("api/GetItem")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetEmployees()
        {
            // Check if there are no employees
            if (!_context.Items.Any())
            {
                return NotFound();
            }

            // Retrieve the list of employees
            var items = await _context.Items.ToListAsync();

            // Map the list of employees to a list of EmployeeDto
            var itemDTOs = _mapper.Map<List<ItemDto>>(items);

            return Ok(itemDTOs);
        }

        [Route("api/GetItemById")]
        [HttpGet]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            try
            {
                if (_context.Items == null)
                {
                    return NotFound();
                }
                var item = await _context.Items.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound();
                }
                var itemDTOs = _mapper.Map<ItemDto>(item);
                return Ok(itemDTOs);

                // return employee;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [Route("api/AddItem")]
        [HttpPost]
        public async Task<ActionResult<Item>> AddItem(Item item)
        {
            try
            {
                //check if object empty
                var item1 = await _context.Items.Where(x =>
                                                     x.Id == item.Id).FirstOrDefaultAsync();

                if (item1 != null) // for update payroll policy
                {
                    item1.ItemName = item.ItemName;
                    item1.Description = item.Description;

                    _context.Entry(item1).State = EntityState.Modified;
                    _context.SaveChanges();

                    return Ok("Successfully Updated");
                }

                else // insert or create payroll policy
                {
                    _context.Items.Add(item);
                    await _context.SaveChangesAsync();
                    //_context.Entry(emp).State = EntityState.Added;
                    //_context.SaveChanges();
                    var itemDTOs = _mapper.Map<ItemDto>(item);
                    return Ok(itemDTOs);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("DeleteItem")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                var itemDet = await _context.Items.FindAsync(id);
                if (itemDet != null)
                {
                    _context.Entry(itemDet).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Ok("Deleted Successfully");
                }
                return Ok("Record does not exist");

            }
            catch (Exception ex)
            {
                throw;
            }
            //if (_context.Employees == null)
            //{
            //    return NotFound();
            //}
            //var employee = await _context.Employees.FindAsync(id);
            //if (employee == null)
            //{
            //    return NotFound();
            //}

            //_context.Employees.Remove(employee);
            //await _context.SaveChangesAsync();

            //return No("Deleted Successfully");
        }

    }
}
