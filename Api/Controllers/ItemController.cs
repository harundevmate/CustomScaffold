//using Api.Controllers.Base;
//using Api.Error;
//using Api.Helper;
//using AutoMapper;
//using BusinessCore.Helper;
//using DarkUnionEngine.Infrastructure.Data.Dto;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Shared.Interfaces;

//namespace Api.Controllers
//{
//    public class ItemController : BaseApiController
//    {
//        private readonly IRepository repository;
//        private readonly IMapper mapper;
//        public ItemController(IRepository repository, IMapper mapper) : base(repository, mapper)
//        {
//            this.repository = repository;
//            this.mapper = mapper;
//        }

//        // GET: api/Item
//        [HttpGet]
//        public async Task<IActionResult> ListAsync()
//        {
//            try
//            {
//                var items = await repository.GetQueryable<Item>().AsNoTracking().ToListAsync();

//                var result = mapper.Map<List<ItemDTO>>(items);

//                return Requests.Response(this, new ApiStatus(200), result, "");

//            }
//            catch (Exception ex)
//            {
//                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
//            }
//        }

//        // GET: api/Item
//        [HttpGet("{id}")]

//        public async Task<IActionResult> GetByIdAsync(Guid id)
//        {
//            try
//            {
//                var items = await repository.GetByIdAsync<Item>(id);

//                var result = mapper.Map<Item>(items);
//                return Requests.Response(this, new ApiStatus(200), result, "");
//            }
//            catch (Exception ex)
//            {
//                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
//            }
//        }

//        // POST: api/Item
//        [HttpPost]
//        public async Task<IActionResult> AddAsync([FromBody] ItemDTO itemDTO)
//        {
//            try
//            {
//                var item = mapper.Map<Item>(itemDTO);

//                if (ModelState.IsValid && ValidateCreate(item))
//                {
//                    item.Id = Guid.NewGuid();

//                    var (Added, Message) = await repository.AddAsync<Item>(item);
//                    return !Added && Message != "" ? Requests.Response(this, new ApiStatus(500), null, Message) : Requests.Response(this, new ApiStatus(200), null, Message);

//                }
//                else
//                {
//                    return Requests.Response(this, new ApiStatus(500), ModelState, "");
//                }
//            }
//            catch (Exception ex)
//            {
//                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
//            }
//        }

//        // PATCH: api/Item
//        [HttpPatch]
//        public async Task<IActionResult> UpdateAsync([FromBody] ItemDTO itemDTO)
//        {
//            try
//            {
//                var existingItems = await repository.GetByIdAsync<Item>(itemDTO.Id);
//                if (existingItems == null)
//                {
//                    return Requests.Response(this, new ApiStatus(409), null, Constant.Message.NotFound);
//                }
//                var item = mapper.Map<ItemDTO, Item>(itemDTO, existingItems);
//                if (ModelState.IsValid && ValidateUpdate(item))
//                {

//                    var (Updated, Message) = await repository.UpdateAsync<Item>(item);
//                    return !Updated ? Requests.Response(this, new ApiStatus(500), null, Message) : Requests.Response(this, new ApiStatus(200), null, Message);

//                }
//                else
//                {
//                    return Requests.Response(this, new ApiStatus(500), ModelState, "");
//                }
//            }
//            catch (Exception ex)
//            {
//                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
//            }

//        }

//        // DELETE: api/Item/1
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteAsync(Guid id)
//        {
//            bool Deleted = false;
//            string Message = "";
//            try
//            {
//                if (ValidateDelete(id))
//                {
//                    var existingItems = await repository.GetByIdAsync<Item>(id);
//                    if (existingItems == null) return Requests.Response(this, new ApiStatus(409), null, Constant.Message.NotFound);
//                    (Deleted, Message) = await repository.DeleteAsync<Item>(id);
//                }
//                return !Deleted ? Requests.Response(this, new ApiStatus(500), null, Message) : Requests.Response(this, new ApiStatus(200), null, Message);

//            }
//            catch (Exception ex)
//            {
//                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
//            }
//        }

//        // POST: api/Item
//        [HttpPost("gridlist")]
//        [Produces("application/json")]
//        public async Task<IActionResult> GetListForDatatable()
//        {
//            try
//            {
//                var data = await repository.GetQueryable<Item>().AsNoTracking().ToListAsync();
//                return Requests.Response(this, new ApiStatus(200), null, Constant.Message.Success);
//            }
//            catch (Exception ex)
//            {
//                return Requests.Response(this, new ApiStatus(500), null, ex.Message);
//            }
//        }
//    }
//}
