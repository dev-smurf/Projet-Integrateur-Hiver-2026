using Domain.Repositories;
using Persistence;


namespace Infrastructure.Repositories.Module;

public class ModuleRepository: IModuleRepository
{
    
    private readonly GarneauTemplateDbContext _context;

    public ModuleRepository(GarneauTemplateDbContext context)
    {
        _context = context;
    }

    public async Task Create(Domain.Entities.Module module)
    {
        // if(_context.Modules.Any(x=>x.Contenu.Trim() == module.Contenu.Trim()))
        //     throw new MOduleWhitContenuAlredyExist($"A book with isbn {book.Isbn} already exists.");

        _context.Modules.Add(module);
        await _context.SaveChangesAsync();
    }
}