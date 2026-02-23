namespace Application.Interfaces.Services.Module;

using Domain.Entities;

public interface IModuleService
{
    Task<Module> CreateModule(Module moduleDto);

}