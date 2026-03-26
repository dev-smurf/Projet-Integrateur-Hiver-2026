using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Appointments;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly GarneauTemplateDbContext _context;

    public AppointmentRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments
            .Include(a => a.Member)
            .Include(a => a.Admin)
            .FirstOrDefaultAsync(a => a.Id == id && a.Deleted == null);
    }

    public async Task<IEnumerable<Appointment>> GetByMemberIdAsync(Guid memberId)
    {
        return await _context.Appointments
            .Where(a => a.MemberId == memberId && a.Deleted == null)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByAdminIdAsync(Guid adminId)
    {
        return await _context.Appointments
            .Include(a => a.Member)
            .Where(a => a.AdminId == adminId && a.Deleted == null)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasConflictAsync(Guid adminId, DateTime date)
    {
        var endTime = date.AddHours(1);
        return await _context.Appointments
            .AnyAsync(a => a.AdminId == adminId
                          && a.Deleted == null
                          && a.Status != AppointmentStatus.Refused
                          && a.Date < endTime
                          && a.Date.AddHours(1) > date);
    }
}
