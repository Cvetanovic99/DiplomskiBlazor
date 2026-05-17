using AutoMapper;
using AutoMapper.QueryableExtensions;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Diplomski.RatingHub.Infrastructure.Persistence.Repositories;

public class CompanyRepository : DatabaseRepository<Company>, ICompanyRepository
{
    public CompanyRepository(ApplicationDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public async Task<IEnumerable<T>> GetPopularCompaniesAndProject<T>(int cityId, int categoryId, int take)
    {
        ISpecification<Company> spec = new Specification<Company>(c => c.CategoryId == categoryId);
        if (cityId != 0)
        {
            spec.And(c => c.CityId == cityId);
        }
        
        var query = _dbContext.Set<Company>().AsQueryable();

        return await query.Where(spec.Criteria)
            .OrderByDescending(c => c.OverallAverageGrade)
            .ThenByDescending(c => c.ReviewsCount)
            .ThenByDescending(c => c.Created)
            .Take(take)
            .ProjectTo<T>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}