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
            return null; // Employee must exist to create a compensation
        }

        // Check if a Compensation already exists for this employee
        var existingCompensation = _compensationRepository.GetByEmployeeId(employeeId);
        if (existingCompensation != null)
        {
            // If so, update the existing record instead of creating a new one
            existingCompensation.Salary = salary;
            existingCompensation.EffectiveDate = effectiveDate;
            _compensationRepository.Update(existingCompensation);
            return existingCompensation;
        }
        else
        {
            // If no existing Compensation, create a new one
            var compensation = new Compensation
            {
                Employee = employee,
                Salary = salary,
                EffectiveDate = effectiveDate
            };
            return _compensationRepository.Create(compensation);
        }
    }


    public Compensation GetByEmployeeId(string employeeId)
    {
        return _compensationRepository.GetByEmployeeId(employeeId);
    }
}