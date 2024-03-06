using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public Employee GetByIdWithDirectReports(string employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);
            if (employee != null)
            {
                PopulateDirectReports(employee); 
            }
            return employee;
        }

        private void PopulateDirectReports(Employee employee)
        {
            var populatedDirectReports = new List<Employee>();

            if (employee.DirectReports != null && employee.DirectReports.Any())
            {
                foreach (var directReport in employee.DirectReports)
                {
                    // Recursive call to populate each direct report's own direct reports
                    var fullDirectReport = _employeeRepository.GetById(directReport.EmployeeId);
                    if (fullDirectReport != null)
                    {
                        PopulateDirectReports(fullDirectReport);  
                        populatedDirectReports.Add(fullDirectReport);
                    }
                }

                // Replace the original direct reports list with the fully populated one
                employee.DirectReports = populatedDirectReports;
            }
        }


        public int CalculateTotalReports(Employee employee)
        {
            HashSet<string> uniqueReportIds = new HashSet<string>();
            CountReportsRecursive(employee, uniqueReportIds);
            return uniqueReportIds.Count;
        }

        private void CountReportsRecursive(Employee employee, HashSet<string> uniqueReportIds)
        {
            if (employee.DirectReports != null)
            {
                foreach (var directReport in employee.DirectReports)
                {
                    if (uniqueReportIds.Add(directReport.EmployeeId))  // Returns true if the set did not already contain the specified element
                    {
                        CountReportsRecursive(directReport, uniqueReportIds);
                    }
                }
            }
        }
    }
}
