using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Availability;

public class AdminAvailabilityRepository : IAdminAvailabilityRepository
{
    private readonly GarneauTemplateDbContext _context;

    public AdminAvailabilityRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AdminAvailability>> GetByAdminIdAsync(Guid adminId)
    {
        return await _context.AdminAvailabilities
            .Where(a => a.AdminId == adminId && a.Deleted == null)
            .OrderBy(a => a.DayOfWeek)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<AdminAvailability> CreateAsync(AdminAvailability availability)
    {
        _context.AdminAvailabilities.Add(availability);
        await _context.SaveChangesAsync();
        return availability;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.AdminAvailabilities.FindAsync(id);
        if (entity != null)
        {
            _context.AdminAvailabilities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllByAdminIdAsync(Guid adminId)
    {
        var entities = await _context.AdminAvailabilities
            .Where(a => a.AdminId == adminId)
            .ToListAsync();
        _context.AdminAvailabilities.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task SaveRecurringAsync(Guid adminId, IEnumerable<AdminAvailability> slots)
    {
        var existing = await _context.AdminAvailabilities
            .Where(a => a.AdminId == adminId)
            .ToListAsync();
        _context.AdminAvailabilities.RemoveRange(existing);

        var newSlots = slots.ToList();
        foreach (var slot in newSlots)
        {
            slot.AdminId = adminId;
        }
        _context.AdminAvailabilities.AddRange(newSlots);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AdminAvailabilityOverride>> GetOverridesByAdminIdAsync(Guid adminId)
    {
        return await _context.AdminAvailabilityOverrides
            .Where(a => a.AdminId == adminId && a.Deleted == null)
            .OrderBy(a => a.Date)
            .ToListAsync();
    }

    public async Task<AdminAvailabilityOverride> CreateOverrideAsync(AdminAvailabilityOverride overrideEntry)
    {
        _context.AdminAvailabilityOverrides.Add(overrideEntry);
        await _context.SaveChangesAsync();
        return overrideEntry;
    }

    public async Task DeleteOverrideAsync(Guid id)
    {
        var entity = await _context.AdminAvailabilityOverrides.FindAsync(id);
        if (entity != null)
        {
            _context.AdminAvailabilityOverrides.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
