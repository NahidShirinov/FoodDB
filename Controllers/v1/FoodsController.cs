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
        private readonly IAuditLogService _auditLogService;
       

        public FoodsController(
            IFoodRepository foodRepository,
            IMapper mapper,
            IAuditLogService auditLogService)
        {
            _foodRepository = foodRepository;
            _mapper = mapper;
            _auditLogService = auditLogService;
    
        }

        [HttpGet(Name = nameof(GetAllFoods))]
        public ActionResult<IEnumerable<FoodDto>> GetAllFoods([FromQuery] QueryParameters queryParameters)
        {
            var foodItems = _foodRepository.GetAll(queryParameters).ToList();

            var allItemCount = _foodRepository.Count();
            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var dtos = foodItems.Select(x => _mapper.Map<FoodDto>(x));
            return Ok(dtos);
        }

        [HttpGet("{id:int}", Name = nameof(GetSingleFood))]
        public ActionResult<FoodDto> GetSingleFood(int id)
        {
            var foodItem = _foodRepository.GetSingle(id);
            if (foodItem == null)
                return NotFound(new { error = $"Food item with id {id} not found." });

            var foodDto = _mapper.Map<FoodDto>(foodItem);
            return Ok(foodDto);
        }

        [HttpPost(Name = nameof(AddFood))]
        public async Task<ActionResult<FoodDto>> AddFood([FromBody] FoodCreateDto foodCreateDto)
        {
            if (foodCreateDto == null)
                return BadRequest(new { error = "Food create model is null." });

            // CategoryId mövcuddursa, yoxla:
            var categoryExists = _foodRepository.CategoryExists(foodCreateDto.CategoryId);
            if (!categoryExists)
                return BadRequest($"Category with id {foodCreateDto.CategoryId} does not exist.");

            var toAdd = _mapper.Map<FoodEntity>(foodCreateDto);
            _foodRepository.Add(toAdd);

            if (!_foodRepository.Save())
                return StatusCode(500, new { error = "Failed to save the new food item." });

            await _auditLogService.LogAsync("POST", HttpContext.Request.Path, nameof(AddFood), foodCreateDto);

            var foodDto = _mapper.Map<FoodDto>(toAdd);
            return CreatedAtRoute(nameof(GetSingleFood), foodDto);
        }


        [HttpPut("{id:int}", Name = nameof(UpdateFood))]
        public async Task<ActionResult<FoodDto>> UpdateFood(int id, [FromBody] FoodUpdateDto foodUpdateDto)
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

            await _auditLogService.LogAsync("PUT", HttpContext.Request.Path, nameof(UpdateFood), foodUpdateDto);

            var foodDto = _mapper.Map<FoodDto>(existingFoodItem);
            return Ok(foodDto);
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateFood))]
        public async Task<ActionResult<FoodDto>> PartiallyUpdateFood(int id, [FromBody] JsonPatchDocument<FoodUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest(new { error = "Patch document is null or invalid." });

            var existingEntity = _foodRepository.GetSingle(id);
            if (existingEntity == null)
                return NotFound(new { error = $"Food item with id {id} not found." });

            var foodToPatch = _mapper.Map<FoodUpdateDto>(existingEntity);

            patchDoc.ApplyTo(foodToPatch, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _mapper.Map(foodToPatch, existingEntity);
            var updated = _foodRepository.Update(id, existingEntity);

            await _auditLogService.LogAsync("PATCH", HttpContext.Request.Path, nameof(PartiallyUpdateFood), patchDoc);

            if (!_foodRepository.Save())
                return StatusCode(500, new { error = "Failed to save the updated food item." });

            var foodDto = _mapper.Map<FoodDto>(updated);
            return Ok(foodDto);
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

            await _auditLogService.LogAsync("DELETE", HttpContext.Request.Path, nameof(RemoveFood), foodItem);

            return Ok(new { result = "Food item successfully deleted.", id = foodItem.Id });
        }

        /*[HttpGet("GetRandomMeal", Name = nameof(GetRandomMeal))]
        public ActionResult<IEnumerable<FoodDto>> GetRandomMeal()
        {
            var foodItems = _foodRepository.GetRandomMeal();
            var dtos = foodItems.Select(x => _mapper.Map<FoodDto>(x));
            return Ok(dtos);
        }*/
    }
}
