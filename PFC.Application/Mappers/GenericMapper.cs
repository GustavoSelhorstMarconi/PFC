using System.Linq.Expressions;
using System.Reflection;

namespace PFC.Application.Mappers;

public static class GenericMapper<TEntity, TDto>
{
    private static readonly Func<TEntity, TDto> _entityToDto;
    private static readonly Func<TDto, TEntity> _dtoToEntity;
    private static readonly Dictionary<string, PropertyMapping> _propertyMappings;

    static GenericMapper()
    {
        _propertyMappings = BuildPropertyMappings();
        _entityToDto = CreateMapper<TEntity, TDto>();
        _dtoToEntity = CreateMapper<TDto, TEntity>();
    }

    public static TDto? ToDto(TEntity entity)
    {
        if (entity == null) return default;
        return _entityToDto(entity);
    }

    public static TEntity? FromDto(TDto dto)
    {
        if (dto == null) return default;
        return _dtoToEntity(dto);
    }

    public static IEnumerable<TDto?> ToDto(IEnumerable<TEntity>? entities)
    {
        if (entities == null) return Enumerable.Empty<TDto>();
        return entities.Select(ToDto);
    }

    public static List<TEntity> FromDto(IEnumerable<TDto>? dtos)
    {
        if (dtos == null) return new List<TEntity>();
        return dtos.Select(FromDto).ToList();
    }

    public static IQueryable<TDto> ProjectToDto(IQueryable<TEntity> query)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var memberInit = CreateMemberInitExpression<TEntity, TDto>(parameter);
        var lambda = Expression.Lambda<Func<TEntity, TDto>>(memberInit, parameter);
        return query.Select(lambda);
    }

    private static Dictionary<string, PropertyMapping> BuildPropertyMappings()
    {
        var sourceProperties = typeof(TEntity).GetProperties();
        var targetProperties = typeof(TDto).GetProperties();
        var mappings = new Dictionary<string, PropertyMapping>();

        foreach (var targetProperty in targetProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(p =>
                p.Name == targetProperty.Name &&
                (p.PropertyType == targetProperty.PropertyType ||
                 IsAssignableOrConvertible(p.PropertyType, targetProperty.PropertyType)));

            if (sourceProperty != null)
                mappings[targetProperty.Name] = new PropertyMapping
                {
                    SourceProperty = sourceProperty,
                    TargetProperty = targetProperty,
                    RequiresConversion = sourceProperty.PropertyType != targetProperty.PropertyType
                };
        }

        return mappings;
    }

    private static bool IsAssignableOrConvertible(Type sourceType, Type targetType)
    {
        if (targetType.IsAssignableFrom(sourceType))
            return true;

        if ((sourceType.IsEnum && targetType == typeof(int)) ||
            (targetType.IsEnum && sourceType == typeof(int)) ||
            (sourceType == typeof(int) && targetType == typeof(string)) ||
            (sourceType == typeof(string) && targetType == typeof(int)) ||
            (sourceType == typeof(DateTime) && targetType == typeof(string)) ||
            (sourceType == typeof(string) && targetType == typeof(DateTime)))
            return true;

        return false;
    }

    private static Func<TSource, TTarget> CreateMapper<TSource, TTarget>()
    {
        var parameter = Expression.Parameter(typeof(TSource), "source");
        var memberInit = CreateMemberInitExpression<TSource, TTarget>(parameter);
        var lambda = Expression.Lambda<Func<TSource, TTarget>>(memberInit, parameter);
        return lambda.Compile();
    }

    private static Expression CreateMemberInitExpression<TSource, TTarget>(ParameterExpression parameter)
    {
        var sourceProperties = typeof(TSource).GetProperties();
        var targetType = typeof(TTarget);

        var ctor = targetType.GetConstructors()
                             .OrderByDescending(c => c.GetParameters().Length)
                             .FirstOrDefault();

        if (ctor != null && ctor.GetParameters().Length > 0)
        {
            var ctorParams = ctor.GetParameters();
            var args = new List<Expression>();

            foreach (var param in ctorParams)
            {
                var sourceProp = sourceProperties
                    .FirstOrDefault(p => string.Equals(p.Name, param.Name, StringComparison.OrdinalIgnoreCase));

                if (sourceProp != null)
                {
                    var sourceAccess = Expression.Property(parameter, sourceProp);

                    args.Add(Expression.Convert(sourceAccess, param.ParameterType));
                }
                else
                {
                    args.Add(Expression.Default(param.ParameterType));
                }
            }

            return Expression.New(ctor, args);
        }

        var newTarget = Expression.New(targetType);
        var targetProperties = targetType.GetProperties();
        var bindings = new List<MemberBinding>();

        foreach (var targetProperty in targetProperties)
        {
            var sourceProperty = sourceProperties.FirstOrDefault(p =>
                p.Name == targetProperty.Name &&
                (p.PropertyType == targetProperty.PropertyType ||
                 IsAssignableOrConvertible(p.PropertyType, targetProperty.PropertyType)));

            if (sourceProperty != null)
            {
                var sourcePropertyAccess = Expression.Property(parameter, sourceProperty);
                Expression valueExpression = sourcePropertyAccess;

                if (sourceProperty.PropertyType != targetProperty.PropertyType)
                {
                    valueExpression = Expression.Convert(sourcePropertyAccess, targetProperty.PropertyType);
                }

                var binding = Expression.Bind(targetProperty, valueExpression);
                bindings.Add(binding);
            }
        }

        return Expression.MemberInit(newTarget, bindings);
    }

    private class PropertyMapping
    {
        public PropertyInfo SourceProperty { get; set; }
        public PropertyInfo TargetProperty { get; set; }
        public bool RequiresConversion { get; set; }
    }
}