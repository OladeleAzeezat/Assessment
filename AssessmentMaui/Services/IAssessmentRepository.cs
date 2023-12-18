using AssessmentMaui.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentMaui.Services;
public interface IAssessmentRepository
{
    Task<Item> AddItem(Item item);
    Task<bool> Delete(int id);
    IEnumerable<Item> GetItems();
    Task<IEnumerable<Item>> GetItemsAsync();
    Task<Employee> Login(Employee employee);
    Task<Employee> Signup(Employee employee);

    //void Register(Employee employee);
}

