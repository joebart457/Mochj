using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Builders
{
    public static class QualifiedObjectBuilder
    {
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

        public static QualifiedObject BuildList(List<QualifiedObject> value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.List, ContainedType = new DataType { TypeId = Enums.DataTypeEnum.Any } } };
        }

        public static QualifiedObject BuildNamespace(_Storage.Environment value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.Namespace } };
        }
    }
}
