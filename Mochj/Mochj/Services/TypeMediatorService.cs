using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Services
{
    public static class TypeMediatorService
    {
        public static DataType DataType<Ty>()
        {
            if (typeof(Ty) == typeof(int))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Number };
            }
            if (typeof(Ty) == typeof(double))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Number };
            }
            if (typeof(Ty) == typeof(float))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Number };
            }
            if (typeof(Ty) == typeof(string))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.String };
            }
            if (typeof(Ty) == typeof(bool))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Boolean };
            }
            if (typeof(Ty) == typeof(DateTime))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.DateTime };
            }
            if (typeof(Ty) == typeof(Models.Fn.Function))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Fn };
            }
            if (typeof(Ty) == typeof(Models.Fn.BoundFn))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Fn };
            }
            if (typeof(Ty) == typeof(_Storage.Environment))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Namespace };
            }
            if (typeof(Ty) == typeof(NativeList))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = new DataType { TypeId = Enums.DataTypeEnum.Any } };
            }
            if (typeof(Ty).IsGenericType && typeof(Ty).GetGenericTypeDefinition() == typeof(List<>))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = DataType(typeof(Ty).GenericTypeArguments[0]) };
            }
            if (typeof(Ty) == typeof(DataType))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.TypeInfo };
            }
            if (typeof(Ty) == typeof(object))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Any };
            }
            throw new Exception($"unable to convert native type {typeof(Ty).FullName} to supported type");
        }

        public static DataType DataType(Type ty)
        {
            if (ty == typeof(int))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Number };
            }
            if (ty == typeof(double))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Number };
            }
            if (ty == typeof(float))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Number };
            }
            if (ty == typeof(string))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.String };
            }
            if (ty == typeof(bool))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Boolean };
            }
            if (ty == typeof(DateTime))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.DateTime };
            }
            if (ty == typeof(Models.Fn.Function))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Fn };
            }
            if (ty == typeof(Models.Fn.BoundFn))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Fn };
            }
            if (ty == typeof(_Storage.Environment))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Namespace };
            }
            if (ty == typeof(NativeList))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = new DataType { TypeId = Enums.DataTypeEnum.Any } };
            }
            if (ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(List<>))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = DataType(ty.GenericTypeArguments[0]) };
            }
            if (ty == typeof(DataType))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.TypeInfo };
            }
            if (ty == typeof(object))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Any };
            }
            throw new Exception($"unable to convert native type {ty.FullName} to supported type");
        }

        public static Ty ToNativeType<Ty>(QualifiedObject value)
        {
            if (typeof(Ty) == typeof(Models.Fn.Function))
            {
                if (value.Type.Is(Enums.DataTypeEnum.Fn)){
                    if (value.Object is Ty tyVal)
                    {
                        return tyVal;
                    }
                    throw new Exception($"error converting object {value} to type {typeof(Ty).FullName}");
                }
                throw new Exception($"unable to convert value of type {value.Type} to type {Enums.DataTypeEnum.Fn}");
            }
            if (typeof(Ty) == typeof(Models.Fn.BoundFn))
            {
                if (value.Type.Is(Enums.DataTypeEnum.Fn))
                {
                    if (value.Object is Ty tyVal)
                    {
                        return tyVal;
                    }
                    throw new Exception($"error converting object {value} to type {typeof(Ty).FullName}");
                }
                throw new Exception($"unable to convert value of type {value.Type} to type {Enums.DataTypeEnum.Fn}");
            }
            if (typeof(Ty) == typeof(_Storage.Environment))
            {
                if (value.Type.Is(Enums.DataTypeEnum.Namespace))
                {
                    if (value.Object is Ty tyVal)
                    {
                        return tyVal;
                    }
                    throw new Exception($"error converting object {value} to type {typeof(Ty).FullName}");
                }
                throw new Exception($"unable to convert value of type {value.Type} to type {Enums.DataTypeEnum.Namespace}");
            }
            if(typeof(Ty) == typeof(NativeList))
            {
                if (value.Type.Is(Enums.DataTypeEnum.NativeList))
                {
                    if (value.Object is Ty tyVal)
                    {
                        return tyVal;
                    }
                    throw new Exception($"error converting object {value} to type {typeof(Ty).FullName}");
                }
                throw new Exception($"unable to convert value of type {value.Type} to type {Enums.DataTypeEnum.NativeList}");
            }
            if (typeof(Ty) == typeof(DataType))
            {
                if (value.Type.Is(Enums.DataTypeEnum.TypeInfo))
                {
                    if (value.Object is Ty tyVal)
                    {
                        return tyVal;
                    }
                    throw new Exception($"error converting object {value} to type {typeof(Ty).FullName}");
                }
                throw new Exception($"unable to convert value of type {value.Type} to type {Enums.DataTypeEnum.TypeInfo}");
            }
            if (value.Object is IConvertible convertibleObj)
            {
                object nativeObj = Convert.ChangeType(convertibleObj, typeof(Ty));
                if (nativeObj is Ty tyObj)
                {
                    return tyObj;
                }
                throw new Exception($"failed conversion of object {value} to type {typeof(Ty).FullName}");
            }
            if (value.Object is Ty typedObject)
            {
                return typedObject;
            }
            throw new Exception($"unable to natively convert object {value} to type {typeof(Ty).FullName}");
        }

        public static List<Ty> ToList<Ty>(QualifiedObject value)
        {
            if (value == null) { return null; }
            if (value.Type.Is(Enums.DataTypeEnum.NativeList))
            {
                if (value.Object is NativeList ls)
                {
                    return ls.Get<Ty>();
                }
                throw new Exception($"error converting object {value} to native type {typeof(NativeList).FullName}");
            }
            throw new Exception($"unable to convert object {value} to type List<{typeof(Ty).FullName}>");
        }

        public static bool IsTruthy(object obj)
        {
            Type ty = obj.GetType();

            if (ty == typeof(int))
            {
                return (int)obj != 0;
            }
            if (ty == typeof(double))
            {
                return (double)obj != 0;
            }
            if (ty == typeof(float))
            {
                return (float)obj != 0;
            }
            if (ty == typeof(string))
            {
                return (string)obj != string.Empty;
            }
            if (ty == typeof(bool))
            {
                return (bool)obj;
            }
            
            throw new Exception($"cannot determine truthiness of native type {ty.FullName}");
        }
    }
}
