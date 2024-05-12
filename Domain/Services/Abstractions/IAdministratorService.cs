using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services.Abstractions;

public interface IAdministratorService : IBaseService<IAdministratorService>
{
    public Administrator GetAdministrator(Guid guid);

    public Administrator CreateAdministrator(string firstName, string lastName, string email, string? externalId = null);

    public void UpdateAdministrator(Administrator newAdministrator);

    public void DeleteAdministrator(Guid guid);
}
