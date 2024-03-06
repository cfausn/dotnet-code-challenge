using System;
using CodeChallenge.Models;
using CodeChallenge.Repositories;

public class CompensationService : ICompensationService
{
    private readonly ICompensationRepository _compensationRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public CompensationService(ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
    {
        _compensationRepository = compensationRepository;
        _employeeRepository = employeeRepository;
    }

    public Compensation Create(string employeeId, decimal salary, DateTime effectiveDate)
    {
        var employee = _employeeRepository.GetById(employeeId);
        if (employee == null)
        {
            return null;
        }

        var compensation = new Compensation
        {
            Employee = employee,
            Salary = salary,
            EffectiveDate = effectiveDate
        };

        return _compensationRepository.Create(compensation);
    }


    public Compensation GetByEmployeeId(string employeeId)
    {
        return _compensationRepository.GetByEmployeeId(employeeId);
    }
}