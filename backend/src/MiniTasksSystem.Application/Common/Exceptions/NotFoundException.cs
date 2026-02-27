namespace MiniTasksSystem.Application.Common.Exceptions;

public sealed class NotFoundException(string entityName, string id) 
    : Exception($"{entityName} with id '{id}' was not found.")
{
}
