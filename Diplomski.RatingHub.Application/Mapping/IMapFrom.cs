using AutoMapper;

namespace Diplomski.RatingHub.Application.Mapping;

public interface IMapFrom<TEntity>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(TEntity), GetType());
}