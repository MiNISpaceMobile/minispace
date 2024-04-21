using Domain.Abstractions;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class AdministratorService : IAdministratorService
{
    private IUnitOfWork uow;

    public AdministratorService(IUnitOfWork uow)
    {
        this.uow = uow;
    }

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
