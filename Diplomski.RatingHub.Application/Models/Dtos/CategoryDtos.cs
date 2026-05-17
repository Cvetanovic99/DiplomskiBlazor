using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Domain.Models;

namespace Diplomski.RatingHub.Application.Models.Dtos;

public class CreateCategoryDto
{
    [Required(ErrorMessage = "Unesite naziv kategorije")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Unesite slug")]
    public string Slug { get; set; }
    public int SortOrder { get; set; }
    public string? Icon { get; set; }
    public bool ShowOnHomePage { get; set; }
    public int? ParentId  {get; set; }
    public List<CreateCategoryKeywordDto> Keywords { get; set; } = new();
    public List<CreateRatingCriterionDto> RatingCriteria { get; set; } = new();
}

public class CreateCategoryKeywordDto : IMapFrom<CategoryKeyword>
{
    public string Keyword { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CategoryKeyword, CreateCategoryKeywordDto>().ReverseMap();
    }
}

public class CreateRatingCriterionDto : IMapFrom<RatingCriterion>
{
    public string Name { get; set; } 
    public int SortOrder { get; set; } 
    public bool IsActive { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<RatingCriterion, CreateRatingCriterionDto>().ReverseMap();
    }
}