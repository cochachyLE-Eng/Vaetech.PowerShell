using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Vaetech.PowerShell.Types;

namespace Vaetech.PowerShell
{
    public static class GetProcessRequestEx
    {
        public static GetProcessRequest WhereObject(this GetProcessRequest getProcess, Expression<Func<GetProcessResponse, bool>> predicate)
        {
            string condition = GetCondition(predicate.Body);
            return getProcess.AddCommand(GetProcessEnums.WhereObject, $"|{GetProcessTypes.WhereObject} {condition}");
        }
        public static string GetCondition(Expression expression)
        {
            string condition = string.Empty;

            if (expression is BinaryExpression)
            {
                BinaryExpression be = expression as BinaryExpression;

                if (be.Left is MemberExpression)
                {
                    MemberExpression left = be.Left as MemberExpression;

                    string property = left.Member.Name;
                    PropertyInfo propertyInfo = typeof(GetProcessResponse).GetProperty(left.Member.Name);

                    if (propertyInfo == null)
                    {
                        MemberExpression me = left.Expression as MemberExpression;
                        if (me.Type == typeof(DateTime))
                            property = $"{me.Member.Name}.{left.Member.Name}";
                        else
                            property = $"{me.Member.Name}";
                    }

                    if (be.Right is MethodCallExpression)
                    {
                        MethodCallExpression mceRight = be.Right as MethodCallExpression;
                        object value = GetValue<ValueType>(mceRight);

                        if (value.GetType() == typeof(DateTime))
                            condition = $"{{$_.{property} {GetExpressionType(be.NodeType)} {GetDateRequest.GetDate((DateTime)value).GetCommand()}}}";
                        else
                            condition = $"{{$_.{property} {GetExpressionType(be.NodeType)} \"{value}\"}}";
                    }
                    else if (be.Right is MemberExpression)
                    {
                        MemberExpression meRight = be.Right as MemberExpression;
                        object value = null;

                        if (meRight.Expression is MethodCallExpression)
                            value = GetValue<ValueType>(meRight.Expression as MethodCallExpression);
                        else if (meRight.Expression is ConstantExpression)
                            value = (meRight.Member as FieldInfo).GetValue((meRight.Expression as ConstantExpression).Value);
                        else if (meRight.Expression is MemberExpression)
                        {
                            MemberExpression meRightAlt = meRight.Expression as MemberExpression;

                            if (meRightAlt.Expression is MethodCallExpression)
                                value = GetValue<ValueType>(meRight.Expression as MethodCallExpression);
                            else if (meRightAlt.Expression is ConstantExpression)
                                value = (meRightAlt.Member as FieldInfo).GetValue((meRightAlt.Expression as ConstantExpression).Value);                            
                        }

                        if (value.GetType() != typeof(DateTime))
                            condition = $"{{$_.{property} {GetExpressionType(be.NodeType)} \"{value}\"}}";
                        else
                        {
                            if(IsMemberValid<DateTime>(meRight.Member.Name))
                                condition = $"{{$_.{property} {GetExpressionType(be.NodeType)} {GetDateRequest.GetDate((DateTime)value).GetCommand()}.{meRight.Member.Name}}}";
                            else
                                condition = $"{{$_.{property} {GetExpressionType(be.NodeType)} {GetDateRequest.GetDate((DateTime)value).GetCommand()}}}";
                        }
                    }
                }
                else if (be.Left is BinaryExpression && be.Right is BinaryExpression)
                {
                    BinaryExpression beLeft = be.Left as BinaryExpression;
                    BinaryExpression beRight = be.Right as BinaryExpression;
                    string[] conditions = new string[2];

                    if (beLeft.Right is MethodCallExpression)
                    {
                        MemberExpression meLeft = beLeft.Left as MemberExpression;
                        MethodCallExpression meRight = beLeft.Right as MethodCallExpression;
                        object value = GetValue<ValueType>(meRight);

                        if (value.GetType() == typeof(DateTime))
                            conditions[0] = $"$_.{meLeft.Member.Name} {GetExpressionType(beLeft.NodeType)} {GetDateRequest.GetDate((DateTime)value).GetCommand()}";
                        else
                            conditions[0] = $"$_.{meLeft.Member.Name} {GetExpressionType(beLeft.NodeType)} \"{value}\"";
                    }
                    else if (beLeft.Left is MemberExpression && beLeft.Right is MemberExpression)
                    {
                        MemberExpression meLeft = beLeft.Left as MemberExpression;
                        MemberExpression meRight = beLeft.Right as MemberExpression;
                        object value = GetValue<ValueType>(meRight.Expression as MethodCallExpression);

                        if (value.GetType() == typeof(DateTime))
                            conditions[0] = $"$_.{meLeft.Member.Name} {GetExpressionType(beLeft.NodeType)} {GetDateRequest.GetDate((DateTime)value).GetCommand()}.{meRight.Member.Name}";
                        else
                            conditions[0] = $"$_.{meLeft.Member.Name} {GetExpressionType(beLeft.NodeType)} \"{value}\"";
                    }
                    else
                    {
                        MemberExpression meLeft = beLeft.Left as MemberExpression;

                        if (beLeft.Right.NodeType == ExpressionType.ArrayIndex)
                        {
                            var beArray = (beLeft.Right as BinaryExpression);
                            var meArray = (beArray.Left as MemberExpression);
                            object value = null;

                            if (meArray.Expression is MethodCallExpression)
                                value = GetValue<ValueType>(meArray.Expression as MethodCallExpression);
                            else if (meArray.Expression is ConstantExpression)
                                value = (meArray.Member as FieldInfo).GetValue((meArray.Expression as ConstantExpression).Value);

                            if (value.GetType() == typeof(DateTime[]))                            
                                conditions[0] = $"$_.{meLeft.Member.Name} {GetExpressionType(beLeft.NodeType)} {GetDateRequest.GetDate(((DateTime[])value)[0]).GetCommand()}";
                            else
                                conditions[0] = $"$_.{meLeft.Member.Name} {GetExpressionType(beLeft.NodeType)} \"{((object[])value)[0]}\"";                           
                        }
                    }

                    if (beRight.Right is MethodCallExpression)
                    {
                        MemberExpression meLeft = beLeft.Left as MemberExpression;
                        MethodCallExpression meRight = beRight.Right as MethodCallExpression;
                        object value = GetValue<ValueType>(meRight);

                        if (value.GetType() == typeof(DateTime))
                            conditions[1] = $"$_.{meLeft.Member.Name} {GetExpressionType(beRight.NodeType)} {GetDateRequest.GetDate((DateTime)value).GetCommand()}";
                        else
                            conditions[1] = $"$_.{meLeft.Member.Name} {GetExpressionType(beRight.NodeType)} \"{value}\"";
                    }
                    else if (beRight.Left is MemberExpression && beRight.Right is MemberExpression)
                    {
                        MemberExpression meLeft = beRight.Left as MemberExpression;
                        MemberExpression meRight = beRight.Right as MemberExpression;
                        object value = GetValue<ValueType>(meRight.Expression as MethodCallExpression);

                        if (value.GetType() == typeof(DateTime))
                            conditions[1] = $"$_.{meLeft.Member.Name} {GetExpressionType(beRight.NodeType)} {GetDateRequest.GetDate((DateTime)value).GetCommand()}.{meRight.Member.Name}";
                        else
                            conditions[1] = $"$_.{meLeft.Member.Name} {GetExpressionType(beRight.NodeType)} \"{value}\"";
                    }
                    else
                    {
                        MemberExpression meLeft = beRight.Left as MemberExpression;

                        if (beRight.Right.NodeType == ExpressionType.ArrayIndex)
                        {
                            var beArray = (beRight.Right as BinaryExpression);
                            var meArray = (beArray.Left as MemberExpression);
                            object value = null;

                            if (meArray.Expression is MethodCallExpression)
                                value = GetValue<ValueType>(meArray.Expression as MethodCallExpression);
                            else if (meArray.Expression is ConstantExpression)
                                value = (meArray.Member as FieldInfo).GetValue((meArray.Expression as ConstantExpression).Value);

                            if (value.GetType() == typeof(DateTime[]))
                                conditions[1] = $"$_.{meLeft.Member.Name} {GetExpressionType(beRight.NodeType)} {GetDateRequest.GetDate(((DateTime[])value)[1]).GetCommand()}";
                            else
                                conditions[1] = $"$_.{meLeft.Member.Name} {GetExpressionType(beRight.NodeType)} \"{((object[])value)[1]}\"";
                        }
                    }

                    condition = $"{{{conditions[0]} {GetExpressionType(be.NodeType)} {conditions[1]}}}";
                }
            }
            else
            {                
                throw new Exception("Not supported");             
            }
            return condition;
        }
        private static bool IsMemberValid<T>(string name)
        {
            if (typeof(T) == typeof(DateTime))
            {
                if (name == "Date"/*||...*/)
                    return true;
            }
            return false;
        }
        private static string GetExpressionType(ExpressionType expressionType)
        {
            string operator_;
            switch (expressionType)
            {
                case ExpressionType.LessThan:
                    operator_ = "-lt";
                    break;
                case ExpressionType.GreaterThan:
                    operator_ = "-gt";
                    break;
                case ExpressionType.LessThanOrEqual:
                    operator_ = "-le";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    operator_ = "-ge";
                    break;
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    operator_ = "-and";
                    break;
                case ExpressionType.Or:
                    operator_ = "-or";
                    break;
                case ExpressionType.Equal:
                default:
                    operator_ = "-eq";
                    break;
            }
            return operator_;
        }
        private static T GetValue<T>(MethodCallExpression expression)
        {
            var objectMember = Expression.Convert(expression, typeof(T));
            var getterLambda = Expression.Lambda<Func<T>>(objectMember);
            var getter = getterLambda.Compile();

            return getter();
        }
        public static GetProcessRequest Where(Expression<Func<GetProcessResponse, bool>> predicate)
        {
            string condition = predicate.ToString();

            return new GetProcessRequest($"|{GetProcessTypes.Where} {condition}");
        }
        public static GetProcessRequest FormatTable(this GetProcessRequest getProcess, params string[] properties)
        {
            List<Tuple<int, string>> properties_ = new List<Tuple<int, string>>();
            foreach (string property in properties)
            {
                PropertyInfo propertyInfo = typeof(GetProcessResponse).GetProperty(property);
                if (propertyInfo != null)
                {
                    int position = propertyInfo.GetCustomAttribute<DisplayAttribute>().GetOrder().Value;
                    properties_.Add(new Tuple<int, string>(position, property));
                }
            }
            return getProcess.AddCommand(GetProcessEnums.FormatTable, $"|{GetProcessTypes.FormatTable} {string.Join(", ", properties_.Select(c => c.Item2).ToArray())}");
        }
        public static GetProcessRequest FormatTable(this GetProcessRequest getProcess, Expression<Func<GetProcessResponse, object>> formatExpression)
        {
            var expression = formatExpression.Body as NewExpression;
            List<Tuple<int, string>> properties = new List<Tuple<int, string>>();
            foreach (MemberExpression member in expression.Arguments)
            {
                PropertyInfo propertyInfo = typeof(GetProcessResponse).GetProperty(member.Member.Name);
                if (propertyInfo != null)
                {
                    int position = propertyInfo.GetCustomAttribute<DisplayAttribute>().GetOrder().Value;
                    properties.Add(new Tuple<int, string>(position, member.Member.Name));
                }
            }
            return getProcess.AddCommand(GetProcessEnums.FormatTable, $"|{GetProcessTypes.FormatTable} {string.Join(", ", properties.Select(c => c.Item2).ToArray())}");
        }
        public static GetProcessRequest FormatList(this GetProcessRequest getProcess, Expression<Func<GetProcessResponse, object>> formatExpression)
        {
            var expression = formatExpression.Body as NewExpression;
            List<Tuple<int, string>> properties = new List<Tuple<int, string>>();
            foreach (MemberExpression member in expression.Arguments)
            {
                PropertyInfo propertyInfo = typeof(GetProcessResponse).GetProperty(member.Member.Name);
                if (propertyInfo != null)
                {
                    int position = propertyInfo.GetCustomAttribute<DisplayAttribute>().GetOrder().Value;
                    properties.Add(new Tuple<int, string>(position, member.Member.Name));
                }
            }
            return getProcess.AddCommand(GetProcessEnums.FormatList, $"|{GetProcessTypes.FormatList} {string.Join(", ", properties.Select(c => c.Item2).ToArray())}");
        }
        public static GetProcessRequest SelectObject(this GetProcessRequest getProcess, Expression<Func<GetProcessResponse, object>> formatExpression)
        {
            var expression = formatExpression.Body as NewExpression;
            List<Tuple<int, string>> properties = new List<Tuple<int, string>>();
            foreach (MemberExpression member in expression.Arguments)
            {
                PropertyInfo propertyInfo = typeof(GetProcessResponse).GetProperty(member.Member.Name);
                if (propertyInfo != null)
                {
                    int position = propertyInfo.GetCustomAttribute<DisplayAttribute>().GetOrder().Value;
                    properties.Add(new Tuple<int, string>(position, member.Member.Name));
                }
            }
            return getProcess.AddCommand(GetProcessEnums.FormatList, $"|{GetProcessTypes.SelectObject} {string.Join(", ", properties.Select(c => c.Item2).ToArray())}");
        }
        public static GetProcessRequest ConvertToJson(this GetProcessRequest getProcess)
        {
            return getProcess.AddCommand(GetProcessEnums.ConvertToJson, $"|{GetProcessTypes.ConvertToJson}");
        }        
    }
}
