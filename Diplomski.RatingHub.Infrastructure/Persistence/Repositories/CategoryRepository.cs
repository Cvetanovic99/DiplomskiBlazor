using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Diplomski.RatingHub.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : DatabaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext dbContext, IMapper mapper)
        : base(dbContext, mapper)
    {
    }

    public async Task<IEnumerable<CategoryWithBreadCrumbDto>> GetCategoriesWithBreadCrumbs(string filterValue, int take)
    {
        var matched = await _dbContext.Categories
            .Where(c =>
                string.IsNullOrEmpty(filterValue) ||
                c.Name.Contains(filterValue) ||
                c.Keywords.Any(k => k.Keyword.Contains(filterValue)))
            .Select(c => new CategoryNode
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId
            })
            .Take(take)
            .ToListAsync();

        if (!matched.Any())
            return new List<CategoryWithBreadCrumbDto>();
        
        var allNodes = new List<CategoryNode>(matched);

        var parentIds = matched
            .Where(x => x.ParentId.HasValue)
            .Select(x => x.ParentId!.Value)
            .Distinct()
            .ToList();
        
        
        while (parentIds.Any())
        {
            var parents = await _dbContext.Categories
                .Where(c => parentIds.Contains(c.Id))
                .Select(c => new CategoryNode
                {
                    Id = c.Id,
                    Name = c.Name,
                    ParentId = c.ParentId
                })
                .ToListAsync();

            allNodes.AddRange(parents);

            parentIds = parents
                .Where(p => p.ParentId.HasValue)
                .Select(p => p.ParentId!.Value)
                .Except(allNodes.Select(n => n.Id)) 
                .Distinct()
                .ToList();
        }
        
        var dict = allNodes
            .GroupBy(x => x.Id)
            .Select(g => g.First())
            .ToDictionary(x => x.Id);
        
        
        var result = matched
            .Select(x => new CategoryWithBreadCrumbDto
            {
                Id = x.Id,
                Name = x.Name,
                FullPath = BuildPath(x.Id, dict)
            })
            .ToList();

        return result;
    }
    
    private string BuildPath(int id, Dictionary<int, CategoryNode> dict)
    {
        var stack = new Stack<string>();

        var current = dict[id];

        while (true)
        {
            stack.Push(current.Name);

            if (current.ParentId == null)
                break;

            if (!dict.TryGetValue(current.ParentId.Value, out var parent))
                break;

            current = parent;
        }

        return string.Join(" > ", stack);
    }
}