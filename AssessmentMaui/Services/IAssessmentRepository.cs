using AssessmentMaui.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentMaui.Services;
public interface IAssessmentRepository
{
    Task<string> GetTokenFromApi();
    Task<Employee> Login(Employee employee);
    
    //void Register(Employee employee);
}

