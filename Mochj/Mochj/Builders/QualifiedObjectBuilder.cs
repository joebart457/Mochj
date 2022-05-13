using Mochj.Models;
using Mochj.Models.Fn;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Builders
{
    public static class QualifiedObjectBuilder
    {
        private static Dictionary<Type, Func<object, QualifiedObject>> _mappings = new Dictionary<Type, Func<object, QualifiedObject>>();

        private static QualifiedObject emptyFunction = null;

        public static bool RegisterMapping<Ty>(Func<object, QualifiedObject> action)
        {
            if (action == null) return false;
            _mappings[typeof(Ty)] = action;
            return true;
        }

        public static QualifiedObject MapValue<Ty>(Ty value)
        {
            if (_mappings.ContainsKey(typeof(Ty)))
            {
                return _mappings[typeof(Ty)](value);
            }
            throw new Exception($"mapping from type {typeof(Ty).FullName} has not been provided");
        }

        public static QualifiedObject MapValue(object value)
        {
            if (value == null) return BuildEmptyValue();
            if (_mappings.ContainsKey(value.GetType()))
            {
                return _mappings[value.GetType()](value);
            }
            throw new Exception($"mapping from type {value.GetType().FullName} has not been provided");
        }

        public static QualifiedObject BuildValue(object obj)
        {
            if (obj == null) return BuildEmptyValue();
            return new QualifiedObject { Object = obj, Type = TypeMediatorService.DataType(obj.GetType()) };
        }

        public static QualifiedObject BuildAnyValue(object obj)
        {
            if (obj == null) return BuildEmptyValue();
            return new QualifiedObject { Object = obj, Type = TypeMediatorService.DataType<object>() };
        }

        public static QualifiedObject BuildEmptyValue()
        {
            return new QualifiedObject { Object = null, Type = new DataType { TypeId = Enums.DataTypeEnum.Empty } };
        }

        public static QualifiedObject BuildBoolean(bool value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.Boolean } };
        }

        public static QualifiedObject BuildString(string value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.String } };
        }

        public static QualifiedObject BuildNumber(double value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.Number } };
        }

        public static QualifiedObject BuildFunction(Models.Fn.Function value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.Fn } };
        }

        public static QualifiedObject EmptyFunction()
        {
            if (emptyFunction != null)
            {
                return emptyFunction;
            }
            var empty = new NativeFunction()
                    .Named("Empty")
                   .Action((Args args) => {
                       return BuildEmptyValue();
                   })
                   .ReturnsEmpty()
                   .Build();

            emptyFunction = BuildFunction(empty);
            return emptyFunction;
        }

        public static QualifiedObject BuildNativeList<Ty>(List<Ty> values)
        {
            return BuildNativeList(values.Select(x => BuildValue(x)).ToList());
        }

        public static QualifiedObject BuildNativeList(List<QualifiedObject> values)
        {
            return BuildNativeList(new NativeList(values));
        }

        public static QualifiedObject BuildNativeList(NativeList value)
        {
            return new QualifiedObject { Object = value, Type = value.Type };
        }
        

        public static QualifiedObject BuildNamespace(_Storage.Environment value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.Namespace } };
        }

        public static QualifiedObject BuildTypeInfo(DataType value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.TypeInfo } };
        }

        public static QualifiedObject BuildTypeInfo<Ty>()
        {
            DataType dt = TypeMediatorService.DataType<Ty>();
            return new QualifiedObject { Object = dt, Type = new DataType { TypeId = Enums.DataTypeEnum.TypeInfo } };
        }
    }
}
