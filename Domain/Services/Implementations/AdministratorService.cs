using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class AdministratorService(IUnitOfWork uow) : BaseService<IAdministratorService, AdministratorService>(uow), IAdministratorService
{
    public Administrator GetAdministrator(Guid guid) => uow.Repository<Administrator>().GetOrThrow(guid);

    public Administrator CreateAdministrator(string firstName, string lastName, string email)
    {
        Administrator administrator = new Administrator(firstName, lastName, email);

        uow.Repository<Administrator>().Add(administrator);
        uow.Commit();

        return administrator;
    }

    public void UpdateAdministrator(Administrator newAdministrator)
    {
        Administrator administrator = uow.Repository<Administrator>().GetOrThrow(newAdministrator.Guid);

        administrator.FirstName = newAdministrator.FirstName;
        administrator.LastName = newAdministrator.LastName;
        administrator.Email = newAdministrator.Email;

        uow.Commit();
    }

    public void DeleteAdministrator(Guid guid)
    {
        if (!uow.Repository<Administrator>().TryDelete(guid))
            throw new InvalidGuidException<Administrator>();
        uow.Commit();
    }
}
