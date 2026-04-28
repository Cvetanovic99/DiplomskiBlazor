using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Queries;

public class GetNewCategorySuggestionsQuery : IRequest<IPaginatedList<NewCategorySuggestionDto>>
{
    public QueryArgs QueryArgs { get; set; }
}

public class GetNewCategorySuggestionsQueryValidator : AbstractValidator<GetNewCategorySuggestionsQuery>
{
    public GetNewCategorySuggestionsQueryValidator()
    {
        RuleFor(x => x.QueryArgs).NotEmpty();
    }
}

public class GetNewCategorySuggestionsQueryHandler : IRequestHandler<GetNewCategorySuggestionsQuery, IPaginatedList<NewCategorySuggestionDto>>
{
    private readonly IDatabaseRepository<NewCategorySuggestion> _newCategorySuggestionRepository;

    public GetNewCategorySuggestionsQueryHandler(
        IDatabaseRepository<NewCategorySuggestion> newCategorySuggestionRepository)
    {
        _newCategorySuggestionRepository = newCategorySuggestionRepository;
    }

    public async Task<IPaginatedList<NewCategorySuggestionDto>> Handle(GetNewCategorySuggestionsQuery request, CancellationToken cancellationToken)
    {
        return await _newCategorySuggestionRepository.GetAndProjectAsPaginatedList<NewCategorySuggestionDto>(
            new Specification<NewCategorySuggestion>(s => true), request.QueryArgs);
    }
}

public class NewCategorySuggestionDto : IMapFrom<NewCategorySuggestion>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
    public NewCategorySuggestionStatus Status { get; set; }

    public string? SuggestedParent { get; set; }
    public int? ParentCategoryId { get; set; }
    public DateTime Created { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<NewCategorySuggestion, NewCategorySuggestionDto>()
            .ForMember(dest => dest.SuggestedParent,
                options => options.MapFrom((src) =>
                    src.ParentCategory.Name));
    }
}