using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class CompensationRepository : ICompensationRepository
{
    private readonly EmployeeContext _context;

    public CompensationRepository(EmployeeContext context)
    {
        _context = context;
    }

    public Compensation Create(Compensation compensation)
    {
        _context.Compensations.Add(compensation);
        _context.SaveChanges();
        return compensation;
    }

    public Compensation Update(Compensation compensation)
    {
        _context.Compensations.Update(compensation);
        _context.SaveChanges();
        return compensation;
    }

    public Compensation GetByEmployeeId(string employeeId)
    {
        return _context.Compensations
            .Include(c => c.Employee)
            .ThenInclude(e => e.DirectReports)
            .FirstOrDefault(c => c.Employee.EmployeeId == employeeId);
    }
}