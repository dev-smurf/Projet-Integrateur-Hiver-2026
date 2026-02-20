using System.Reflection;

namespace Domain.Repositories;

public interface IModuleRepository
{
    List<Module> GetModules();
}