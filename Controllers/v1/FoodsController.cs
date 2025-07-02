using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Helpers;
using SampleWebApiAspNetCore.Services;
using SampleWebApiAspNetCore.Models;
using SampleWebApiAspNetCore.Repositories;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using IAuditLogService = SampleWebApiAspNetCore.Repositories;

namespace SampleWebApiAspNetCore.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodRepository _foodRepository;
        private readonly IMapper _mapper;
        private readonly ILinkService<FoodsController> _linkService;
        private readonly Services.IAuditLogService _auditLogService;
       

        public FoodsController(
            IFoodRepository foodRepository,
            IMapper mapper,
            ILinkService<FoodsController> linkService,
            Services.IAuditLogService auditLogService)
        
        
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
            _linkService = linkService;
            _auditLogService =auditLogService; ;
        }
      
      
        [HttpGet(Name = nameof(GetAllFoods))]
        public ActionResult GetAllFoods(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<FoodEntity> foodItems = _foodRepository.GetAll(queryParameters).ToList();

            var allItemCount = _foodRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var links = _linkService.CreateLinksForCollection(queryParameters, allItemCount, version);
            var toReturn = foodItems.Select(x => _linkService.ExpandSingleFoodItem(x, x.Id, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleFood))]
        public ActionResult GetSingleFood(ApiVersion version, int id)
        {
            var foodItem = _foodRepository.GetSingle(id);
            if (foodItem == null)
            {
                return NotFound(new { error = $"Food item with id {id} not found." });
            }

            var foodDto = _mapper.Map<FoodDto>(foodItem);
            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpPost(Name = nameof(AddFood))]
        public async Task<ActionResult<FoodDto>> AddFood(ApiVersion version, [FromBody] FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null)
                return BadRequest(new { error = "Food create model is null." });
           

            var toAdd = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(toAdd);

            if (!_foodRepository.Save())
                return StatusCode(500, new { error = "Failed to save the new food item." });

            var newFoodItem = _foodRepository.GetSingle(toAdd.Id);
            var foodDto = _mapper.Map<FoodDto>(newFoodItem);

            // ✅ Audit log yaz
            await _auditLogService.LogAsync(
                action: "POST",
                endpoint: HttpContext.Request.Path,
                method: nameof(AddFood),
                requestBody: foodCreateDto
            );

            return CreatedAtRoute(nameof(GetSingleFood),
                new { version = version.ToString(), id = newFoodItem.Id },
                _linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpPut("{id:int}", Name = nameof(UpdateFood))]
        public async Task<ActionResult<FoodDto>> UpdateFood(ApiVersion version, int id, [FromBody] FoodUpdateDto foodUpdateDto)
        {
            if (foodUpdateDto == null)
                return BadRequest(new { error = "Update model is null." });

            var existingFoodItem = _foodRepository.GetSingle(id);
            if (existingFoodItem == null)
                return NotFound(new { error = $"Food item with id {id} not found." });

            _mapper.Map(foodUpdateDto, existingFoodItem);
            _foodRepository.Update(id, existingFoodItem);

            if (!_foodRepository.Save())
                return StatusCode(500, new { error = "Failed to update the food item." });

            // ✅ Audit log yaz
            await _auditLogService.LogAsync(
                action: "PUT",
                endpoint: HttpContext.Request.Path,
                method: nameof(UpdateFood),
                requestBody: foodUpdateDto
            );

            var foodDto = _mapper.Map<FoodDto>(existingFoodItem);
            return Ok(StatusCode(200,"created succesfully"));
        }


        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public ActionResult<FoodDto> PartiallyUpdateFood(ApiVersion version, int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(new { error = "Patch document is null or invalid." });
            }

            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null)
            {
                return NotFound(new { error = $"Food item with id {id} not found." });
            }

            var foodToPatch = _mapper.Map<FoodUpdateDto>(existingEntity);

            try
            {
                patchDoc.ApplyTo(foodToPatch, ModelState);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }
            catch (JsonPatchException ex)
            {
                return BadRequest(new { error = "Invalid JSON Patch operation.", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Unexpected error during patch operation.", detail = ex.Message });
            }

            _mapper.Map(foodToPatch, existingEntity);
            var updated = _foodRepository.Update(id, existingEntity);
             _auditLogService.LogAsync(
                action: "PATCH",
                endpoint: HttpContext.Request.Path,
                method: nameof(PartiallyUpdateFood),
                requestBody: patchDoc
            );


            if (!_foodRepository.Save())
            {
                return StatusCode(500, new { error = "Failed to save the updated food item." });
            }

            var foodDto = _mapper.Map<FoodDto>(updated);
            return Ok(_linkService.ExpandSingleFoodItem(foodDto, foodDto.Id, version));
        }

        [HttpDelete("{id:int}", Name = nameof(RemoveFood))]
        public async Task<ActionResult> RemoveFood(int id)
        {
            var foodItem = _foodRepository.GetSingle(id);
            if (foodItem == null)
                return NotFound(new { error = $"Food item with id {id} not found." });

            _foodRepository.Delete(id);

            if (!_foodRepository.Save())
                return StatusCode(500, new { error = "Failed to delete the food item." });

            // ✅ Audit log yaz
            await _auditLogService.LogAsync(
                action: "DELETE",
                endpoint: HttpContext.Request.Path,
                method: nameof(RemoveFood),
                requestBody: foodItem
            );

            return StatusCode(200, new { result = "Food item successfully deleted.", id = foodItem.Id });
        }


        [HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult GetRandomMeal()
        {
            var foodItems = _foodRepository.GetRandomMeal();
            var dtos = foodItems.Select(x => _mapper.Map<FoodDto>(x));

            var links = new List<LinkDto>
            {
                new LinkDto(Url.Link(nameof(GetRandomMeal), null), "self", "GET")
            };

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }
    }
}
