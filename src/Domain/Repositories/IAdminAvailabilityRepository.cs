using Domain.Entities;

namespace Domain.Repositories;

public interface IAdminAvailabilityRepository
{
    Task<IEnumerable<AdminAvailability>> GetByAdminIdAsync(Guid adminId);
    Task<AdminAvailability> CreateAsync(AdminAvailability availability);
    Task DeleteAsync(Guid id);
    Task DeleteAllByAdminIdAsync(Guid adminId);
    Task SaveRecurringAsync(Guid adminId, IEnumerable<AdminAvailability> slots);

    Task<IEnumerable<AdminAvailabilityOverride>> GetOverridesByAdminIdAsync(Guid adminId);
    Task<AdminAvailabilityOverride> CreateOverrideAsync(AdminAvailabilityOverride overrideEntry);
    Task DeleteOverrideAsync(Guid id);
}
