using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Api.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;
        public BaseApiController(IRepository repository,IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [ExcludeFromCodeCoverage]
        protected virtual bool ValidateCreate<T>(T entity)
        {
            return true;
        }

        [ExcludeFromCodeCoverage]
        protected virtual bool ValidateUpdate<T>(T entity)
        {
            return true;
        }

        [ExcludeFromCodeCoverage]
        protected virtual bool ValidateDelete(Guid id)
        {
            return true;
        }

    }
}
