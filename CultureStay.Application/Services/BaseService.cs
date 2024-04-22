using AutoMapper;
using CultureStay.Application.Common.Interfaces;
using CultureStay.Domain.Repositories.Base;

namespace CultureStay.Application.Services;

public abstract class BaseService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUser currentUser)
{
    protected readonly IUnitOfWork UnitOfWork = unitOfWork;
    protected readonly IMapper Mapper = mapper;
    protected readonly ICurrentUser CurrentUser = currentUser;
}
