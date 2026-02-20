namespace Application.Interfaces.Services.Module;

using Application.Services.Module.Dto;
using Domain.Entities;

public interface IModuleService
{
    Task<Module> CreateModule(ModuleDto moduleDto);

}