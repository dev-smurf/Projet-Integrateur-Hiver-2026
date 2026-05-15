using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Repositories.Module
{
    public class EquipeModuleRepository : IEquipeModuleRepository
    {
        private readonly GarneauTemplateDbContext _context;

        public EquipeModuleRepository(GarneauTemplateDbContext context)
        {
            _context = context;
        }

        public async Task<EquipeModule> AssignAsync(EquipeModule equipeModule)
        {
            _context.EquipeModules.Add(equipeModule);

            await _context.SaveChangesAsync();

            return equipeModule;
        }
        public async Task UnassignAsync(EquipeModule equipeModule)
        {
            _context.EquipeModules.Remove(equipeModule);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsAssignedAsync(Guid equipeId, Guid moduleId)
        {
            return await _context.EquipeModules.AnyAsync(x =>
                x.EquipeId == equipeId &&
                x.ModuleId == moduleId);
        }
        public async Task<EquipeModule?> GetByEquipeAndModuleAsync(Guid equipeId, Guid moduleId)
        {
            return await _context.EquipeModules
                .FirstOrDefaultAsync(x =>
                    x.EquipeId == equipeId &&
                    x.ModuleId == moduleId);
        }

        public async Task<List<EquipeModule>> GetByEquipeIdAsync(Guid equipeId)
        {
            return await _context.EquipeModules
                .Include(x => x.Module)
                .Where(x => x.EquipeId == equipeId)
                .ToListAsync();
        }
        public async Task<List<EquipeModule>> GetByModuleIdAsync(Guid moduleId)
        {
            return await _context.EquipeModules
                .Include(x => x.Equipe)
                .Where(x => x.ModuleId == moduleId)
                .ToListAsync();
        }
    }
}
