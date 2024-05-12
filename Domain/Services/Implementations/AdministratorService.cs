using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class AdministratorService(IUnitOfWork uow) : BaseService<IAdministratorService, AdministratorService>(uow), IAdministratorService
{
    public Administrator GetAdministrator(Guid guid)
    {
        AllowOnlyAdmins();

        return uow.Repository<Administrator>().GetOrThrow(guid);
    }

    public Administrator CreateAdministrator(string firstName, string lastName, string email, string? externalId = null)
    {
        AllowOnlyAdmins();

        Administrator administrator = new Administrator(firstName, lastName, email, externalId);

        uow.Repository<Administrator>().Add(administrator);
        uow.Commit();

        return administrator;
    }

    public void UpdateAdministrator(Administrator newAdministrator)
    {
        AllowOnlyAdmins();

        Administrator administrator = uow.Repository<Administrator>().GetOrThrow(newAdministrator.Guid);

        administrator.FirstName = newAdministrator.FirstName;
        administrator.LastName = newAdministrator.LastName;
        administrator.Email = newAdministrator.Email;

        uow.Commit();
    }

    public void DeleteAdministrator(Guid guid)
    {
        AllowOnlyAdmins();

        if (!uow.Repository<Administrator>().TryDelete(guid))
            throw new InvalidGuidException<Administrator>();
        uow.Commit();
    }
}
