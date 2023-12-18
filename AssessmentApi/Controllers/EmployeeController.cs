using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AssessmentApi.Data;
using AssessmentApi.Models;
using AssessmentApi.POCO;
using NuGet.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AssessmentApi.Helper;
using AutoMapper;
using AssessmentApi.TableDTO;

namespace AssessmentApi.Controllers
{
    [Authorize]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AssessmentDbContext _context;
        private readonly IConfiguration _configuration;

        public EmployeeController(AssessmentDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;

        }

        /// <summary>
        /// gets a list of Employee records
        /// </summary>
        [Route("api/GetEmployees")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            // Check if there are no employees
            if (!_context.Employees.Any())
            {
                return NotFound();
            }

            // Retrieve the list of employees
            var employees = await _context.Employees.ToListAsync();

            // Map the list of employees to a list of EmployeeDto
            var empDTOs = _mapper.Map<List<EmployeeDto>>(employees);

            return Ok(empDTOs);
        }

        [Route("api/GetEmployeeById")]
        [HttpGet]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            Employee employee = new Employee();
            try
            {
                if (_context.Employees == null)
                {
                    return NotFound();
                }
                employee = await _context.Employees.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (employee == null)
                {
                    return NotFound();
                }
                var empDTOs = _mapper.Map<EmployeeDto>(employee);
                return Ok(empDTOs);

                // return employee;
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        /// <summary>
        /// Add and update Employee information
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("api/AddEmployees")]
        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            StatusMessage statusMessage = new StatusMessage();

            try
            {
                //check if object empty
                var emp = await _context.Employees.Where(x =>
                                                     x.Id == employee.Id).FirstOrDefaultAsync();

                if (emp != null) // for update payroll policy
                {
                    emp.Email = employee.Email;
                    emp.Name = employee.Name;
                    emp.Username = employee.Username;
                    emp.Password = employee.Password;

                    _context.Entry(emp).State = EntityState.Modified;
                    _context.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                }

                else // insert or create payroll policy
                {
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();
                    //_context.Entry(emp).State = EntityState.Added;
                    //_context.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Success";
                    statusMessage.data = GetEmployeeById(employee.Id);
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return CreatedAtAction("GetEmployeeById", new { id = employee.Id }, employee);
        }


        // DELETE: api/Employee/5
        [HttpDelete("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employeeDet = await _context.Employees.FindAsync(id);
                if (employeeDet != null)
                {
                    _context.Entry(employeeDet).State = EntityState.Deleted;
                    _context.SaveChanges();
                    var empDTOs = _mapper.Map<EmployeeDto>(employeeDet);
                    return Ok(empDTOs);
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


        [AllowAnonymous]
        [HttpPost("api/Login")]
        public async Task<ActionResult<Employee>> Login(string email, string password)
        {
            if (email != null || password != null)
            {
                var employee = await _context.Employees.Where(x =>
                                                         x.Email == email &&
                                                         x.Password == password).FirstOrDefaultAsync();
                return employee != null ? Ok(employee) : NotFound();
            }
            else
            {
                return BadRequest("Invalid Email or Password");
            }

        }

        [AllowAnonymous]
        [HttpPost("api/TestLogin")]
        public async Task<ActionResult<StatusMessage>> TestLogin(string Email, string Password)
        {
            StatusMessage statusMessage = new StatusMessage();
            if (Email != null || Password != null)
            {
                var employee = await _context.Employees.Where(x => x.Email == Email &&
                                                                  x.Password == Password).FirstOrDefaultAsync();
                //return employee != null ? Ok(employee) : NotFound();

                var tokenString = generateToken(Email);
                if (employee != null)
                {
                    statusMessage.Status = "true";
                    statusMessage.Message = "success";
                    statusMessage.data = employee;
                    statusMessage.auth_token = tokenString;
                    return Ok(statusMessage);
                }
                else
                {
                    return BadRequest("Invalid Request");
                }
            }
            else
            {
                return BadRequest("Invalid Request");
            }




        }

        [AllowAnonymous]
        [Route("api/generateToken")]
        [HttpPost]
        public string generateToken(string Username)
        {
            try
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
                var TokenExpiryTimeHour = Convert.ToInt64(_configuration["JWTKey:TokenExpiryTimeInSeconds"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {

                    Issuer = _configuration["JWTKey:ValidIssuer"],
                    Audience = _configuration["JWTKey:ValidAudience"],
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                    //Subject = new ClaimsIdentity(claims)
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                      new Claim(ClaimTypes.Name, Username.ToString())
                    }),

                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



    }
}
