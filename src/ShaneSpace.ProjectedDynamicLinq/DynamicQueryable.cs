using AutoMapper;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ShaneSpace.ProjectedDynamicLinq
{
    public static class DynamicQueryable
    {
        public static IConfigurationProvider MappingConfigurationProvider { get; set; }

        public static IConfigurationProvider UseProjectedDynamicLinq(this IConfigurationProvider configuration)
        {
            MappingConfigurationProvider = configuration;
            return configuration;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where<T>((IQueryable)source, null, predicate, values);
        }

        public static IQueryable<TSource> Where<TSource, TDestination>(this IQueryable<TSource> source, string predicate, params object[] values)
        {
            return (IQueryable<TSource>)Where<TDestination>(source, null, predicate, values);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, IConfigurationProvider mapperConfigurationProvider, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where<T>((IQueryable)source, mapperConfigurationProvider, predicate, values);
        }

        public static IQueryable<TSource> Where<TSource, TDestination>(this IQueryable<TSource> source, IConfigurationProvider mapperConfigurationProvider, string predicate, params object[] values)
        {
            return (IQueryable<TSource>)Where<TDestination>(source, mapperConfigurationProvider, predicate, values);
        }

        private static IQueryable Where<TDestination>(this IQueryable source, IConfigurationProvider mapperConfigurationProvider, string predicate, params object[] values)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var configurationProvider = GetMappingConfigurationProvider(mapperConfigurationProvider);

            var lambda = DynamicExpression.ParseLambda<TDestination>(configurationProvider, source.ElementType, typeof(bool), predicate, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy<T>((IQueryable)source, null, ordering, values);
        }

        public static IQueryable<TSource> OrderBy<TSource, TDestination>(this IQueryable<TSource> source, string ordering, params object[] values)
        {
            return (IQueryable<TSource>)OrderBy<TDestination>(source, null, ordering, values);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IConfigurationProvider mapperConfigurationProvider, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy<T>((IQueryable)source, mapperConfigurationProvider, ordering, values);
        }

        public static IQueryable<TSource> OrderBy<TSource, TDestination>(this IQueryable<TSource> source, IConfigurationProvider mapperConfigurationProvider, string ordering, params object[] values)
        {
            return (IQueryable<TSource>)OrderBy<TDestination>(source, mapperConfigurationProvider, ordering, values);
        }

        private static IQueryable OrderBy<TDestination>(this IQueryable source, IConfigurationProvider mapperConfigurationProvider, string ordering, params object[] values)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (ordering == null) throw new ArgumentNullException(nameof(ordering));
            var configurationProvider = GetMappingConfigurationProvider(mapperConfigurationProvider);


            ParameterExpression[] parameters = {
                Expression.Parameter(source.ElementType, "") };
            var parser = new ExpressionParser<TDestination>(parameters, ordering, values, configurationProvider);
            var orderings = parser.ParseOrdering();
            var queryExpr = source.Expression;
            var methodAsc = "OrderBy";
            var methodDesc = "OrderByDescending";
            foreach (var o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), o.Ascending ? methodAsc : methodDesc,
                    new[] { source.ElementType, o.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        private static IConfigurationProvider GetMappingConfigurationProvider(IConfigurationProvider configurationProvider = null)
        {
            var output = configurationProvider
                         ?? MappingConfigurationProvider
                         ?? Mapper.Configuration;

            if (output == null)
            {
                throw new ArgumentNullException($"Please call {nameof(DynamicQueryable)}.{nameof(MappingConfigurationProvider)} prior to calling methods without {nameof(IConfigurationProvider)} parameter or use overloads with {nameof(IConfigurationProvider)} parameters.");
            }

            return output;
        }
    }
}