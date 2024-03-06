using CodeChallenge.Models;

public interface ICompensationRepository
{
    Compensation Create(Compensation compensation);
    Compensation Update(Compensation compensation);
    Compensation GetByEmployeeId(string employeeId);
}