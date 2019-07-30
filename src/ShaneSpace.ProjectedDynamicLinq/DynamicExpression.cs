using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapper;

namespace ShaneSpace.ProjectedDynamicLinq
{
    public static class DynamicExpression
    {
        public static LambdaExpression ParseLambda<TDest>(IConfigurationProvider mapperConfigurationProvider, Type itType, Type resultType, string expression, params object[] values)
        {
            return ParseLambda<TDest>(mapperConfigurationProvider, new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expression, values);
        }

        public static LambdaExpression ParseLambda<TDest>(IConfigurationProvider mapperConfigurationProvider, ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            ExpressionParser<TDest> parser = new ExpressionParser<TDest>(parameters, expression, values, mapperConfigurationProvider);
            return Expression.Lambda(parser.Parse(resultType), parameters);
        }

        public static Type CreateClass(IEnumerable<DynamicProperty> properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }
    }
}