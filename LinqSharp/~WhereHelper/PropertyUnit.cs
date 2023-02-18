﻿// Copyright 2020 zmjack
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqSharp
{
    public class PropertyUnit<TSource>
    {
        public readonly string PropertyName;
        public readonly Type PropertyType;
        private readonly ParameterExpression Parameter;
        private readonly Expression Exp;

        internal PropertyUnit(ParameterExpression parameter, string propertyName, Type propertyType)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            Parameter = parameter;
            Exp = Expression.Property(Parameter, PropertyName);
        }

        internal PropertyUnit(ParameterExpression parameter, Expression exp, Type propertyType)
        {
            Exp = exp;
            Parameter = parameter;
            PropertyType = propertyType;
        }

        internal PropertyUnit(ParameterExpression parameter, LambdaExpression exp)
        {
            if ((exp.Body as MemberExpression)?.Member is PropertyInfo prop)
            {
                Exp = exp.Body;
                Parameter = parameter;
                PropertyType = prop.PropertyType;
            }
            else throw new NotSupportedException("Invalid lambda expression.");
        }

        public WhereExpression<TSource> Contains(string value)
        {
            if (PropertyType == typeof(string)) return Invoke(MethodUnit.StringContains, value);
            else throw new NotSupportedException($"{nameof(Contains)} does not support type {PropertyType?.FullName ?? "null"}.");
        }

        public WhereExpression<TSource> In(Array array)
        {
            ConstantExpression constant;
            if (array is object[])
            {
                var ofType = MethodUnit.GenericOfType.MakeGenericMethod(PropertyType);
                constant = Expression.Constant(ofType.Invoke(null, new object[] { array }));
            }
            else constant = Expression.Constant(array);

            var method = MethodUnit.GenericContains.MakeGenericMethod(PropertyType);
            var body = Expression.Call(method, constant, Exp);
            var exp = Expression.Lambda<Func<TSource, bool>>(body, Parameter);
            return new WhereExpression<TSource>(exp);
        }

        public static PropertyUnit<TSource> operator +(PropertyUnit<TSource> @this, object value) => @this.UnitAddOp(value);
        public static PropertyUnit<TSource> operator -(PropertyUnit<TSource> @this, object value) => @this.UnitOp(Expression.SubtractChecked, value);
        public static PropertyUnit<TSource> operator *(PropertyUnit<TSource> @this, object value) => @this.UnitOp(Expression.MultiplyChecked, value);
        public static PropertyUnit<TSource> operator /(PropertyUnit<TSource> @this, object value) => @this.UnitOp(Expression.Divide, value);
        public static PropertyUnit<TSource> operator %(PropertyUnit<TSource> @this, object value) => @this.UnitOp(Expression.Modulo, value);
        public static WhereExpression<TSource> operator ==(PropertyUnit<TSource> @this, object value) => @this.CompareOp(Expression.Equal, value);
        public static WhereExpression<TSource> operator !=(PropertyUnit<TSource> @this, object value) => @this.CompareOp(Expression.NotEqual, value);
        public static WhereExpression<TSource> operator >(PropertyUnit<TSource> @this, object value) => @this.CompareOp(Expression.GreaterThan, value);
        public static WhereExpression<TSource> operator <(PropertyUnit<TSource> @this, object value) => @this.CompareOp(Expression.LessThan, value);
        public static WhereExpression<TSource> operator >=(PropertyUnit<TSource> @this, object value) => @this.CompareOp(Expression.GreaterThanOrEqual, value);
        public static WhereExpression<TSource> operator <=(PropertyUnit<TSource> @this, object value) => @this.CompareOp(Expression.LessThanOrEqual, value);

        public static PropertyUnit<TSource> operator +(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.UnitAddOp(other);
        public static PropertyUnit<TSource> operator -(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.UnitOp(Expression.SubtractChecked, other);
        public static PropertyUnit<TSource> operator *(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.UnitOp(Expression.MultiplyChecked, other);
        public static PropertyUnit<TSource> operator /(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.UnitOp(Expression.Divide, other);
        public static PropertyUnit<TSource> operator %(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.UnitOp(Expression.Modulo, other);
        public static WhereExpression<TSource> operator ==(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.CompareOp(Expression.Equal, other);
        public static WhereExpression<TSource> operator !=(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.CompareOp(Expression.NotEqual, other);
        public static WhereExpression<TSource> operator >(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.CompareOp(Expression.GreaterThan, other);
        public static WhereExpression<TSource> operator <(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.CompareOp(Expression.LessThan, other);
        public static WhereExpression<TSource> operator >=(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.CompareOp(Expression.GreaterThanOrEqual, other);
        public static WhereExpression<TSource> operator <=(PropertyUnit<TSource> @this, PropertyUnit<TSource> other) => @this.CompareOp(Expression.LessThanOrEqual, other);

        private Expression GetValueExpression(object value)
        {
            if (value.GetType() == PropertyType) return Expression.Constant(value);
            else return Expression.Convert(Expression.Constant(value), PropertyType);
        }

        private PropertyUnit<TSource> UnitAddOp(object value)
        {
            var operand = GetValueExpression(value);
            if (PropertyType == typeof(string))
                return new PropertyUnit<TSource>(Parameter, Expression.Add(Exp, operand, MethodUnit.StringConcat), typeof(string));
            else return new PropertyUnit<TSource>(Parameter, Expression.AddChecked(Exp, operand), PropertyType);
        }
        private PropertyUnit<TSource> UnitOp(Func<Expression, Expression, BinaryExpression> func, object value)
        {
            var operand = GetValueExpression(value);
            return new PropertyUnit<TSource>(Parameter, func(Exp, operand), PropertyType);
        }
        private WhereExpression<TSource> CompareOp(Func<Expression, Expression, BinaryExpression> func, object value)
        {
            var operand = GetValueExpression(value);
            var body = func(Exp, operand);
            var exp = Expression.Lambda<Func<TSource, bool>>(body, Parameter);
            return new WhereExpression<TSource>(exp);
        }

        private PropertyUnit<TSource> UnitAddOp(PropertyUnit<TSource> unit)
        {
            var operand = unit.Exp;
            if (PropertyType == typeof(string))
                return new PropertyUnit<TSource>(Parameter, Expression.Add(Exp, operand, MethodUnit.StringConcat), typeof(string));
            else return new PropertyUnit<TSource>(Parameter, Expression.AddChecked(Exp, operand), PropertyType);
        }
        private PropertyUnit<TSource> UnitOp(Func<Expression, Expression, BinaryExpression> func, PropertyUnit<TSource> unit)
        {
            var operand = unit.Exp;
            return new PropertyUnit<TSource>(Parameter, func(Exp, operand), PropertyType);
        }
        private WhereExpression<TSource> CompareOp(Func<Expression, Expression, BinaryExpression> func, PropertyUnit<TSource> unit)
        {
            var operand = unit.Exp;
            var body = func(Exp, operand);
            var exp = Expression.Lambda<Func<TSource, bool>>(body, Parameter);
            return new WhereExpression<TSource>(exp);
        }

        private WhereExpression<TSource> Invoke(MethodInfo method, params object[] parameters)
        {
            var body = Expression.Call(Exp, method, parameters.Select(x => Expression.Constant(x)));
            var exp = Expression.Lambda<Func<TSource, bool>>(body, Parameter);
            return new WhereExpression<TSource>(exp);
        }

    }
}
