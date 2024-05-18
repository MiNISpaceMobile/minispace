using Domain.Abstractions;
using Domain.BaseTypes;
using Domain.DataModel;
using Domain.Services.Abstractions;

namespace Domain.Services.Implementations;

public class UserService(IUnitOfWork uow, IStudentService studentService, IAdministratorService adminService)
    : BaseService<IUserService, UserService>(uow), IUserService
{
    public IEnumerable<User> GetAllUsers()
    {
        AllowOnlyAdmins();

        return uow.Repository<User>().GetAll();
    }

    public (bool IsAdmin, bool IsOrganizer) UserRoles(Guid targetGuid, bool? isAdmin, bool? isOrganizer)
    {
        AllowOnlyAdmins();

        User user = uow.Repository<User>().GetOrThrow(targetGuid);
        if (isAdmin.HasValue || isOrganizer.HasValue)
        {
            user.IsAdmin = isAdmin ?? user.IsAdmin;
            user.IsOrganizer = isOrganizer ?? user.IsOrganizer;
            uow.Commit();
        }
        return (user.IsAdmin, user.IsOrganizer);
    }
}
