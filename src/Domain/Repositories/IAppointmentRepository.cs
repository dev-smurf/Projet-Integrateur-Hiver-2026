using Domain.Entities;

namespace Domain.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Appointment>> GetByMemberIdAsync(Guid memberId);
    Task<IEnumerable<Appointment>> GetByAdminIdAsync(Guid adminId);
    Task<Appointment> CreateAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
    Task<bool> HasConflictAsync(Guid adminId, DateTime date);
}
