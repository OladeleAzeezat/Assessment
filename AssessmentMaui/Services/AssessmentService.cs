using AssessmentMaui.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace AssessmentMaui.Services;
public class AssessmentService : IAssessmentRepository
{
    public async Task<Employee> Login(Employee emp)
    {
        try
        {
            string signinRequeststr = JsonConvert.SerializeObject(emp);

            var client = new HttpClient();
            string localhostUrl = "https://localhost:44383/api/Login?email=" + emp.Email + "&password=" + emp.Password;
            client.BaseAddress = new Uri(localhostUrl);
            HttpResponseMessage response = await client.PostAsync(client.BaseAddress, new StringContent(signinRequeststr, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Employee employee = await response.Content.ReadFromJsonAsync<Employee>();
                return await Task.FromResult(employee);
            }
            return null;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            return null;
        }
    }

    public async Task<string> GetTokenFromApi()
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppSettings.JwtToken);
            try
            {

                Employee employee = new Employee();
                var emp = Login(employee);

                string apiUrl = "https://localhost:44383/api/generateToken?Username=" + employee.Email; // Replace with your API endpoint
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string token = await response.Content.ReadAsStringAsync();
                    // Now, 'token' contains your JWT token

                    // You can store the token in AppSettings or use it as needed
                    AppSettings.JwtToken = token;
                    return await Task.FromResult(token);

                    // Perform other actions based on the token
                }
                return "Error";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }    
    }




    //public async Task<Employee> Dign(string email, string password)
    //{
    //    try
    //    {
    //        var client = new HttpClient();
    //        //string localhostUrl = "https://localhost:44383/employee/login/(email)/(password)?" + email + "&" + password;
    //        string localhostUrl = "https://localhost:44383/api/Employee/login/(email)/(password)?email=" + email +"&password=" + password;
    //        //string localhostUrl = "https://localhost:44383/api/Employee/TestLogin?Email=" + email +"&Password=" + password;
    //        client.BaseAddress = new Uri(localhostUrl);
    //        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
    //        if (response.IsSuccessStatusCode)
    //        {
    //            Employee employee = await response.Content.ReadFromJsonAsync<Employee>();
    //            return await Task.FromResult(employee);
    //        }
    //        return null;
    //    }
    //    catch (Exception ex)
    //    {
    //        await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
    //        return null;
    //    }
    //}
}

