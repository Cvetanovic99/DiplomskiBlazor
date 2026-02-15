using AutoMapper;
using AutoMapper.QueryableExtensions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Interfaces.Specifications;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Interfaces;
using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Diplomski.RatingHub.Infrastructure.Persistence.Repositories.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Diplomski.RatingHub.Infrastructure.Persistence.Repositories;

public class DatabaseRepository<TEntity> : IDatabaseRepository<TEntity>, IRepositoryWithProjections<TEntity>
    where TEntity : class, IDatabaseEntity
{
    protected readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    protected DatabaseRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<TEntity?> GetById(int id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<List<TEntity>> GetAll()
    {
        return await _dbContext.Set<TEntity>().ToListAsync();
    }

    public async Task<List<TEntity>> Get(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<TEntity?> GetSingleBySpec(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<int> GetCount(ISpecification<TEntity> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    public async Task<TEntity> Insert(TEntity entity)
    {
        try
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            throw;
        }

        return entity;
    }

    public async Task<ICollection<TEntity>> InsertRange(ICollection<TEntity> entities)
    {
        try
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }

            throw;
        }

        return entities;
    }

    public async Task<TEntity> Update(TEntity entity)
    {
        _dbContext.Set<TEntity>().Update(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task UpdateRange(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(TEntity entity)
    {
        _dbContext.Set<TEntity>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteRange(ICollection<TEntity> entities)
    {
        try
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Detached;
            }

            throw;
        }
    }
    
    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(_dbContext.Set<TEntity>().AsQueryable(), specification);
    }
    
    public async Task<ICollection<T>> GetAndProject<T>(ISpecification<TEntity> spec) where T : class, IMapFrom<TEntity>
    {
        return await ApplySpecification(spec).AsNoTracking().ProjectTo<T>(_mapper.ConfigurationProvider).ToListAsync();
    }
    
    public async Task<ICollection<T>> GetAllAndProject<T>() where T : class, IMapFrom<TEntity>
    {
        var spec = new Specification<TEntity>(e => true);
        return await GetAndProject<T>(spec);
    }
    
    public async Task<T?> GetSingleAndProject<T>(ISpecification<TEntity> spec) where T : class, IMapFrom<TEntity>
    {
        return await ApplySpecification(spec).AsNoTracking().ProjectTo<T>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }
}