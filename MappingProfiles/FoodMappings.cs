﻿using AutoMapper;
using SampleWebApiAspNetCore.Dtos;
using SampleWebApiAspNetCore.Entities;

namespace SampleWebApiAspNetCore.MappingProfiles
{
    public class FoodMappings : Profile
    {
        public FoodMappings()
        {
            CreateMap<FoodEntity, FoodDto>().ReverseMap();
            CreateMap<FoodEntity, FoodUpdateDto>().ReverseMap();
            CreateMap<FoodEntity, FoodCreateDto>().ReverseMap();
            CreateMap<UserEntity, UserResponseDto>();
            CreateMap<UserCreateDto, UserEntity>(); 
            CreateMap<CategoryEntity, CategoryReadDto>();
            CreateMap<CategoryCreateDto, CategoryEntity>();

        }
    }
}
