using Azure;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.ObjectPool;
using NuGet.ContentModel;
using NuGet.Protocol.Plugins;

public class ApiHandler
{
    private readonly HttpClient _httpClient;
    private readonly string api_key;
    private readonly string _base_url;
    private readonly string _hardware_url;
    private readonly string _user_url;

    public ApiHandler()
    {
        _httpClient = new HttpClient();
        api_key = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIxIiwianRpIjoiOTRiMmRkZjgxMjkzNTg4MTgzNzhmYjlhZjNjMTU0ZDQ5ZDVjNzQxZTk4MDExYjU3Nzk0OTcyNjBmNWExZjM5NWIwMTBiMGE2ZTU4ZDdiYjAiLCJpYXQiOjE3MTU2MTQ4NDguMzU3NzkxLCJuYmYiOjE3MTU2MTQ4NDguMzU3NzkyLCJleHAiOjIxODg5MTQwNDguMjc2NzA5LCJzdWIiOiIyMTQ5MiIsInNjb3BlcyI6W119.XB9P_wlc3FqQZ1zNeM7xcnZsJPnVohYn9pREzyw9opkRaI7zIilIlPDAeUIiVoChwRN83ytyqcteMeuBU22mfWQCUAB3Ww4zooiBWMlMbpDMhjuY6bH28HBKMTXcBOeacnzfzGU265OYgOKs1jAqJwR1m7SdxKYm_gGauOVgcEeFkYWnldOTl3iy4kgB3loiLZ4ly3aBKvWH2J8QapwHFCj9avfJ3yBlTzzShTWqPTCOz9V8jn5QKyq_XoY5Q1EbyqabiPYZVX-1-e6Vsm3GhPvdpQ0Y28bN9jzzvwHnz52FmGUF2gvGgvV-8mM4cYkHAoexwZjoC3IMxIO96zsHIwfQNX8pTmzdij2sUr917P0z0yMlAlWD4zWQQIdKbeeLppgiu639p4IOhxSx3HsP_RN02xNi8ww5dfYwtvpz3BPo4DnSzFmV-_LTqb4qGSR_jX4FfimV_wA3pnRSget19_3UxIWFDNmoERQgoYyzns-ubMg5V_99d8ORUkLGXyjR6YyIkqWD18gTYpzXqN1IaWMJH4LhXUwQg_YZP7bfPr-GWtAuwOBbuDZKWhn-3hSyDoK6mQ2GVcQGy_oq3Ifmmn5Ss6Drhk-Mz3_aXde9Hq25GHKeGyqfrVveCutNodiLyEqYPglypyLRJFQozJdoAhZAb4uk6btRCDAII_YZVws";
        _base_url = "https://itservice6.eku.edu/api/v1/";
        _hardware_url = "https://itservice6.eku.edu/api/v1/hardware";
        _user_url = "https://itservice6.eku.edu/api/v1/users";   

        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {api_key}");
    }
    
    public async Task<List<UserInfo>> User_Search(string? name, string? email, string? id, string? company_id)
    {
        string? endpoint = null;

        Console.WriteLine("Evaluate Parameters for `User_Search`...");
        if (id != null)
        {
            Console.WriteLine("Resolving Endpoint...");
            endpoint = $"?employee_num={id}";
        }
        else if (email != null)
        {
            Console.WriteLine("Resolving Endpoint...");
            endpoint = $"?email={email}";
        }
        else if (name != null)
        {
            string[] name_split = name.Split();
            string? first_name = name_split.Length > 1 ? name_split[0] : null;
            string? last_name = name_split.Length > 1 ? name_split[1] : name_split[0];

            if (first_name == null)
            {
                endpoint = $"?last_name={last_name}";
            }
            else
            {
                endpoint = $"?first_name={first_name}&last_name={last_name}";
            }
            Console.WriteLine("Resolving Endpoint...");
        }
        else
        {
            Console.WriteLine("Cannot Endpoint.");
        }

        // Add the company ID to the endpoint
        endpoint += $"&company_id={company_id}";

        Console.WriteLine("Endpoint resolved.");
        Console.WriteLine($"Using endpoint `{endpoint}`");
        string base_url = _user_url;    // Use the url for users because we are doing a user search

        // Send GET request
        Console.WriteLine("Begin API Request...");
        HttpResponseMessage response = await _httpClient.GetAsync(base_url + endpoint);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            string json = @responseContent;

            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            JArray rows = jsonObject["rows"] as JArray;

            if (rows != null && rows.Count > 0)
            {
                List<Dictionary<string, object>> users = rows.Select(row => row.ToObject<Dictionary<string, object>>()).ToList();
                List<UserInfo> userInfoList = new List<UserInfo>();

                foreach (var user in users)
                {
                    string firstName = user.ContainsKey("first_name") ? user["first_name"].ToString() : "N/A";
                    string lastName = user.ContainsKey("last_name") ? user["last_name"].ToString() : "N/A";
                    string user_email = user.ContainsKey("email") ? user["email"].ToString() : "N/A";
                    string employeeNumber = user.ContainsKey("employee_num") ? user["employee_num"].ToString() : "N/A";
                    string? groups = user.ContainsKey("groups") ? user["groups"].ToString() : "N/A";
                    bool no_checkout_list = groups.Contains("no checkout", StringComparison.OrdinalIgnoreCase);

                    Console.WriteLine($"User: {firstName} {lastName}, Email: {user_email}, Employee Number: {employeeNumber}, No Checkout List: {no_checkout_list}");
                    // Create a new UserInfo object and add it to the list
                    userInfoList.Add(new UserInfo
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = user_email,
                        EmployeeNumber = employeeNumber,
                        NoCheckoutList = no_checkout_list
                    });
                }

                return userInfoList;
            }

        }
        else
        {
            Console.WriteLine("The API request was denied.");
            throw new HttpRequestException($"Failed to search: {response.StatusCode}");
        }


        return [];
    }


    public async Task<List<UserInfo>> staffFac_Search(string? name, string? email, string? id, string? company_id)
    {
        string? endpoint = null;

        Console.WriteLine("Evaluate Parameters for `User_Search`...");
        if (id != null)
        {
            Console.WriteLine("Resolving Endpoint...");
            endpoint = $"?employee_num={id}";
        }
        else if (email != null)
        {
            Console.WriteLine("Resolving Endpoint...");
            endpoint = $"?email={email}";
        }
        else if (name != null)
        {
            string[] name_split = name.Split();
            string? first_name = name_split.Length > 1 ? name_split[0] : null;
            string? last_name = name_split.Length > 1 ? name_split[1] : name_split[0];

            if (first_name == null)
            {
                endpoint = $"?last_name={last_name}";
            }
            else
            {
                endpoint = $"?first_name={first_name}&last_name={last_name}";
            }
            Console.WriteLine("Resolving Endpoint...");
        }
        else
        {
            Console.WriteLine("Cannot Endpoint.");
        }

        // Add the company ID to the endpoint
        endpoint += $"&company_id={company_id}";

        Console.WriteLine("Endpoint resolved.");
        Console.WriteLine($"Using endpoint `{endpoint}`");
        string base_url = _user_url;    // Use the url for users because we are doing a user search

        // Send GET request
        Console.WriteLine("Begin API Request...");
        HttpResponseMessage response = await _httpClient.GetAsync(base_url + endpoint);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            string json = @responseContent;

            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            JArray rows = jsonObject["rows"] as JArray;

            if (rows != null && rows.Count > 0)
            {
                List<Dictionary<string, object>> users = rows.Select(row => row.ToObject<Dictionary<string, object>>()).ToList();
                List<UserInfo> userInfoList = new List<UserInfo>();

                foreach (var user in users)
                {
                    string firstName = user.ContainsKey("first_name") ? user["first_name"].ToString() : "N/A";
                    string lastName = user.ContainsKey("last_name") ? user["last_name"].ToString() : "N/A";
                    string user_email = user.ContainsKey("email") ? user["email"].ToString() : "N/A";
                    string employeeNumber = user.ContainsKey("employee_num") ? user["employee_num"].ToString() : "N/A";
                    string department = "N/A";
                    if (user.ContainsKey("department") && user["department"] is JObject deptObject && deptObject.ContainsKey("name"))
                    {
                        department = deptObject["name"].ToString();
                    }
                    string phone = user.ContainsKey("phone") ? user["phone"].ToString() : "N/A";
                    string userGroup = user.ContainsKey("user_group") ? user["user_group"].ToString() : "N/A";

                    Console.WriteLine($"User: {firstName} {lastName}, Email: {user_email}, Employee Number: {employeeNumber}, Dept: {department}, Phone: {phone}");

                    userInfoList.Add(new UserInfo
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = user_email,
                        EmployeeNumber = employeeNumber,
                        Department = department,
                        Phone = phone,
                        UserGroup = userGroup
                    });
                }

                return userInfoList;
            }

        }
        else
        {
            Console.WriteLine("The API request was denied.");
            throw new HttpRequestException($"Failed to search: {response.StatusCode}");
        }


        return [];
    }

}


public class UserInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string EmployeeNumber { get; set; }
    public bool NoCheckoutList { get; set; }
    public string Department { get; set; }
    public string Phone { get; set; }
    public string UserGroup { get; set; }
}