using AssessmentMaui.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;
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

    private async Task<string> GetTokenFromApi(string Username)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AppSettings.JwtToken);
            try
            {

               // Employee employee = new Employee();

                string apiUrl = "https://localhost:44383/api/generateToken?Username=" + Username; // Replace with your API endpoint
                https://localhost:44383/api/generateToken?Username=
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

    public async Task<Employee> Signup(Employee emp)
    {
        try
        {
            //string signinRequeststr = JsonConvert.SerializeObject(emp);

            var client = new HttpClient();
            //var jwtToken = GetTokenFromApi(emp.Email);

            string apiUrl = "https://localhost:44383/api/AddEmployees";
            //client.DefaultRequestHeaders.Add("Authorization", await jwtToken);

            var requestData = new
            {
                NAME = emp.Name,
                EMAIL = emp.Email,
                PASSWORD = emp.Password,
                USERNAME = emp.Username
            };

            string postData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

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

    public async Task<Item> AddItem(Item item)
    {
        try
        {
            var client = new HttpClient();

            string apiUrl = "https://localhost:44383/api/AddItem";

            var requestData = new
            {
                NAME = item.itemName,
                DESCRIPTION = item.description
            };

            string postData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            StringContent content = new StringContent(postData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Ignore property name casing
                };
                Item itm = await response.Content.ReadFromJsonAsync<Item>();
                return await Task.FromResult(itm);
            }
            return null;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            return null;
        }
    }

    public IEnumerable<Item> GetItems()
    {
        try
        {
            var client = new HttpClient();
            string localhostUrl = "https://localhost:44383/api/GetItem";
            client.BaseAddress = new Uri(localhostUrl);
            HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result; // Synchronously wait for the response

            if (response.IsSuccessStatusCode)
            {
                Item item = response.Content.ReadFromJsonAsync<Item>().Result; // Synchronously read from the response
                return new List<Item> { item }; // Return a single-item collection
            }

            return Enumerable.Empty<Item>(); // Return an empty collection instead of null


            //    var client = new HttpClient();
            //    string localhostUrl = "https://localhost:44383//api/GetItem";
            //    client.BaseAddress = new Uri(localhostUrl);
            //    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        Item item = await response.Content.ReadFromJsonAsync<Item>();
            //        return (IEnumerable<Item>)await Task.FromResult(item);
            //    }
            //    return null;

        }
        catch (Exception ex)
        {
            Shell.Current.DisplayAlert("Error", ex.Message, "Ok").Wait(); // Synchronously display an alert
            return Enumerable.Empty<Item>();

            //await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            //return Enumerable.Empty<Item>();
            //return null;
        }
    }

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        try
        {
            var client = new HttpClient();
            string localhostUrl = "https://localhost:44383/api/GetItem";
            client.BaseAddress = new Uri(localhostUrl);

            HttpResponseMessage response = await client.GetAsync(client.BaseAddress);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Ignore property name casing
                };
                var items = await response.Content.ReadFromJsonAsync<IEnumerable<Item>>(options);
                return items ?? Enumerable.Empty<Item>();

                //Item item = await response.Content.ReadFromJsonAsync<Item>();
                //return new List<Item> { item };
            }

            return Enumerable.Empty<Item>();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            return Enumerable.Empty<Item>();
        }
    
}

    public async Task<bool> Delete(int id)
    {
        try
        {
            string apiUrl = $"https://localhost:44383/api/DeleteItem?id=" + id;


            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.DeleteAsync(client.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Ignore property name casing
                    };
                    Item itm = await response.Content.ReadFromJsonAsync<Item>();
                    return true;
                    //return await Task.FromResult(itm);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Failed", "Failed to delete item", "Ok");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., network error)
            Debug.WriteLine($"Error: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "An error occurred while deleting the item", "Ok");
            return false;
        }
    }

    //public async Task<Employee> Signup(Employee emp)
    //{
    //    try
    //    {
    //        string signinRequeststr = JsonConvert.SerializeObject(emp);

    //        var client = new HttpClient();
    //        string localhostUrl = "https://localhost:44383//api/AddEmployees";
    //       // + emp.Email + "&password=" + emp.Password;
    //        client.BaseAddress = new Uri(localhostUrl);
    //        HttpResponseMessage response = await client.PostAsync(client.BaseAddress, new StringContent(signinRequeststr, Encoding.UTF8, "application/json"));
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

