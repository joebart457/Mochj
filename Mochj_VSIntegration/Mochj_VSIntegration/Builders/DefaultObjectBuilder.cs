using Mochj.Builders;
using Mochj.Models;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE.Builders
{
    static class DefaultObjectBuilder
    {
        public static QualifiedObject BuildDefault(DataType dt)
        {
            if (dt == null) return BuildUnknown();
            if (dt.Is(Mochj.Enums.DataTypeEnum.Empty))
            {
                return BuildEmpty();
            }
            if (dt.Is(Mochj.Enums.DataTypeEnum.Number))
            {
                return BuildNumber();
            }
            if (dt.Is(Mochj.Enums.DataTypeEnum.String))
            {
                return BuildString();
            }
            if (dt.Is(Mochj.Enums.DataTypeEnum.Boolean))
            {
                return BuildBoolean();
            }
            if (dt.Is(Mochj.Enums.DataTypeEnum.NativeList))
            {
                return QualifiedObjectBuilder.BuildNativeList(new List<QualifiedObject> { BuildDefault(dt.ContainedType) });
            }
            if (dt.Is(Mochj.Enums.DataTypeEnum.Any))
            {
                return BuildAny();
            }
            if (dt.Is(Mochj.Enums.DataTypeEnum.TypeInfo))
            {
                return BuildTypeInfo();
            }
            if (dt.Is(Mochj.Enums.DataTypeEnum.Fn))
            {
                return BuildFn();
            }

            return BuildUnknown();
        }

        public static QualifiedObject BuildEmpty()
        {
            return QualifiedObjectBuilder.BuildEmptyValue();
        }

        public static QualifiedObject BuildNumber()
        {
            return QualifiedObjectBuilder.BuildNumber(0);
        }

        public static QualifiedObject BuildString()
        {
            return QualifiedObjectBuilder.BuildString("<string>");
        }
        public static QualifiedObject BuildBoolean()
        {
            return QualifiedObjectBuilder.BuildBoolean(true);
        }

        public static QualifiedObject BuildTypeInfo()
        {
            return QualifiedObjectBuilder.BuildTypeInfo<object>();
        }

        public static QualifiedObject BuildAny()
        {
            return new QualifiedObject { Object = "<any>", Type = new DataType { TypeId = Mochj.Enums.DataTypeEnum.Any } };
        }

        public static QualifiedObject BuildUnknown()
        {
            return new QualifiedObject { Object = "<unkown>", Type = new DataType { TypeId = Mochj.Enums.DataTypeEnum.Unknown } };
        }
        public static QualifiedObject BuildFn()
        {
            return new QualifiedObject { Object = new NativeFunction(), Type = new DataType { TypeId = Mochj.Enums.DataTypeEnum.Fn } };
        }
    }
}
