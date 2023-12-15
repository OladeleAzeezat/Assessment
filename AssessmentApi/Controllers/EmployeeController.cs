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

namespace AssessmentApi.Controllers
{
    [Authorize]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AssessmentDbContext _context;
        private readonly IConfiguration _configuration;

        public EmployeeController(AssessmentDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/Employee
        /// <summary>
        /// gets a list of Employee records
        /// </summary>
        [Route("api/GetEmployees")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
          if (_context.Employees == null )
          {
              return NotFound();
          }
            return await _context.Employees.ToListAsync();
        }

        /// <summary>
        /// gets Employee record by Employee Id
        /// </summary>
        [Route("api/GetEmployeeById")]
        [HttpGet]
        public async Task<ActionResult<Employee>> GetEmployeeById(int id)
        {
            Employee employee = new Employee();
          try
          { 
              if (_context.Employees == null)
              {
                  return  NotFound();
              }
              employee = await _context.Employees.Where(x => x.Id == id).FirstOrDefaultAsync();

             if (employee == null)
             {
                 return NotFound();
             }

                   // return employee;
          }
          catch (Exception ex)
          {
                throw;
          }
          return employee;
           
        }

        /// <summary>
        /// Add and update Employee information
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [Route("api/AddEmployees")]
        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            //if (!_context.Employees.Contains(employee))
            //{
            //    return Problem("Entity set 'AssessmentDbContext.Employees'  is null.");
            //}


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
            StatusMessage statusMessage = new StatusMessage();
            try
            {
                var employeeDet = await _context.Employees.FindAsync(id);
                if (employeeDet != null)
                {
                    _context.Entry(employeeDet).State = EntityState.Deleted;
                    _context.SaveChanges();

                    statusMessage.Status = "Success";
                    statusMessage.Message = "Deleted Successfully";

                }
                else
                {
                    statusMessage.Status = "Failed";
                    statusMessage.Message = "Record does not exist";
                }
            }
            catch (Exception ex)
            {
                statusMessage.Status = "Failed";
                statusMessage.Message = ex.Message;
            }
            return Ok(statusMessage);
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
