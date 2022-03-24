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
        private static QualifiedObject emptyFunction = null;

        public static QualifiedObject BuildValue(object obj)
        {
            if (obj == null) return BuildEmptyValue();
            return new QualifiedObject { Object = obj, Type = TypeMediatorService.DataType(obj.GetType()) };
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
