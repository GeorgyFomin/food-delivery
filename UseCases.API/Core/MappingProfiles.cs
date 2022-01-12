﻿using AutoMapper;
using Entities.Domain;
using UseCases.API.Dto;

namespace UseCases.API.Core
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Ingredient, IngredientDto>();
            CreateMap<Delivery, DeliveryDto>();
            CreateMap<Product, ProductDto>();
        }
    }
}