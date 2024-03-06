
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void GetReportingStructure_Returns_ExpectedStructureWithFourDirectReports()
        {
            // Arrange
            var expectedEmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedNumberOfReports = 4;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{expectedEmployeeId}");
            var response = getRequestTask.Result;

            // Logging the response for debugging
            var contentString = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"Response Content: {contentString}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            
            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(expectedEmployeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.NumberOfReports);

            Assert.AreEqual(2, reportingStructure.Employee.DirectReports.Count); // John has 2 direct reports: Paul and Ringo
        }


        [TestMethod]
        public void GetReportingStructure_Returns_ExpectedStructureWithTwoDirectReports()
        {
            // Arrange
            var expectedEmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f";
            var expectedNumberOfReports = 2;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/reportingStructure/{expectedEmployeeId}");
            var response = getRequestTask.Result;

            // Logging the response for debugging
            var contentString = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"Response Content: {contentString}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();

            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(expectedEmployeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.NumberOfReports);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void GetReportingStructure_WithNoDirectReports_Returns_ZeroReports()
        {
            // Arrange
            string employeeId = "62c1084e-6e34-4630-93fd-9153afb65309"; 

            // Act
            var response = _httpClient.GetAsync($"api/employee/reportingStructure/{employeeId}").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();

            Assert.IsNotNull(reportingStructure);
            Assert.AreEqual(0, reportingStructure.NumberOfReports); 
        }

        [TestMethod]
        public void GetReportingStructure_WithInvalidEmployeeId_Returns_NotFound()
        {
            // Arrange
            string invalidEmployeeId = "NonExistentEmployeeId";

            // Act
            var response = _httpClient.GetAsync($"api/employee/reportingStructure/{invalidEmployeeId}").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [TestMethod]
        public void CreateCompensation_Returns_CreatedWithExpectedValues()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"; 
            var compensation = new
            {
                salary = 60000,
                effectiveDate = new DateTime(2023, 1, 1)
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(compensation), Encoding.UTF8, "application/json");

            // Act
            var response = _httpClient.PostAsync($"api/compensation/{employeeId}", requestContent).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var createdCompensation = JsonConvert.DeserializeObject<Compensation>(response.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(compensation.salary, createdCompensation.Salary);
            Assert.AreEqual(compensation.effectiveDate, createdCompensation.EffectiveDate);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_CompensationWithDirectReports()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            // Act
            var response = _httpClient.GetAsync($"api/compensation/{employeeId}").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensation = JsonConvert.DeserializeObject<Compensation>(response.Content.ReadAsStringAsync().Result);
            Assert.IsNotNull(compensation.Employee.DirectReports);
            Assert.IsTrue(compensation.Employee.DirectReports != null && compensation.Employee.DirectReports.Any());
        }

        [TestMethod]
        public void CreateCompensation_WithInvalidEmployeeId_Returns_NotFound()
        {
            // Arrange
            string invalidEmployeeId = "InvalidEmployeeId"; 
            var compensation = new
            {
                salary = 50000,
                effectiveDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(compensation), Encoding.UTF8, "application/json");

            // Act
            var response = _httpClient.PostAsync($"api/compensation/{invalidEmployeeId}", requestContent).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_WithNegativeSalary_Returns_BadRequest()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"; 
            var compensation = new
            {
                salary = -10000, // Intentionally negative to test validation
                effectiveDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(compensation), Encoding.UTF8, "application/json");

            // Act
            var response = _httpClient.PostAsync($"api/compensation/{employeeId}", requestContent).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_WithTooHighSalary_Returns_BadRequest()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var compensation = new
            {
                salary = 1000000000, // Intentionally too high
                effectiveDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(compensation), Encoding.UTF8, "application/json");

            // Act
            var response = _httpClient.PostAsync($"api/compensation/{employeeId}", requestContent).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void UpdateExistingCompensation_Returns_UpdatedValues()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"; // Ensure this is a valid employee ID
            decimal initialSalary = 50000;
            DateTime initialEffectiveDate = DateTime.UtcNow.AddDays(-30);
            decimal updatedSalary = 55000;
            DateTime updatedEffectiveDate = DateTime.UtcNow;

            // Initial Compensation
            var compensation = new
            {
                salary = initialSalary,
                effectiveDate = initialEffectiveDate.ToString("yyyy-MM-dd")
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(compensation), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync($"api/compensation/{employeeId}", requestContent).Result;
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            // Updated Compensation
            var updatedCompensation = new
            {
                salary = updatedSalary,
                effectiveDate = updatedEffectiveDate.ToString("yyyy-MM-dd")
            };
            var updateRequestContent = new StringContent(JsonConvert.SerializeObject(updatedCompensation), Encoding.UTF8, "application/json");

            // Act
            var updateResponse = _httpClient.PostAsync($"api/compensation/{employeeId}", updateRequestContent).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, updateResponse.StatusCode);
            var createdCompensation = JsonConvert.DeserializeObject<Compensation>(updateResponse.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(updatedSalary, createdCompensation.Salary);
            Assert.AreEqual(updatedEffectiveDate.Date, createdCompensation.EffectiveDate.Date);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_WithNoCompensationData_Returns_NotFound()
        {
            // Arrange
            string employeeIdWithoutCompensation = "62c1084e-6e34-4630-93fd-9153afb65309"; 

            // Act
            var response = _httpClient.GetAsync($"api/compensation/{employeeIdWithoutCompensation}").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateThenGetCompensation_Returns_ConsistentData()
        {
            // Arrange - Create compensation
            string employeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var newCompensation = new
            {
                salary = 70000,
                effectiveDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            var createRequestContent = new StringContent(JsonConvert.SerializeObject(newCompensation), Encoding.UTF8, "application/json");

            // Act - Create
            var createResponse = _httpClient.PostAsync($"api/compensation/{employeeId}", createRequestContent).Result;
            Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);

            // Act - Retrieve
            var getResponse = _httpClient.GetAsync($"api/compensation/{employeeId}").Result;

            // Assert - Check consistency
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);
            var retrievedCompensation = JsonConvert.DeserializeObject<Compensation>(getResponse.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(newCompensation.salary, retrievedCompensation.Salary);
            Assert.AreEqual(DateTime.Parse(newCompensation.effectiveDate), retrievedCompensation.EffectiveDate);
        }

        [TestMethod]
        public void CreateCompensation_Concurrently_ForSameEmployee_Returns_ConsistentResponses()
        {
            // Arrange
            string employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"; // Assuming this is a valid employee ID
            var compensation = new
            {
                salary = 75000,
                effectiveDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            var requestContent = new StringContent(JsonConvert.SerializeObject(compensation), Encoding.UTF8, "application/json");

            // Use TPL (Task Parallel Library) to simulate concurrent requests
            int numberOfConcurrentRequests = 5;
            var tasks = new Task<HttpResponseMessage>[numberOfConcurrentRequests];

            for (int i = 0; i < numberOfConcurrentRequests; i++)
            {
                tasks[i] = _httpClient.PostAsync($"api/compensation/{employeeId}", requestContent);
            }

            Task.WaitAll(tasks); // Wait for all tasks to complete

            // Assert
            // Check that all responses are Created (201)
            bool allResponsesAreValid = tasks.All(task => task.Result.StatusCode == HttpStatusCode.Created);
            Assert.IsTrue(allResponsesAreValid, "Not all responses are valid. Expected all requests to either succeed or fail with a conflict status.");

            var getResponse = _httpClient.GetAsync($"api/compensation/{employeeId}").Result;
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode, "Expected to successfully retrieve compensation for the employee.");
        }
    }
}
