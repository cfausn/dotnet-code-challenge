using System;
using CodeChallenge.Models;

public interface ICompensationService
{
    Compensation Create(string employeeId, decimal salary, DateTime effectiveDate);
    Compensation GetByEmployeeId(string employeeId);
}