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
                   .Action((Args args) => {
                       return BuildEmptyValue();
                   })
                   .ReturnsEmpty()
                   .Build();

            emptyFunction = BuildFunction(empty);
            return emptyFunction;
        }

        public static QualifiedObject BuildList(IEnumerable<QualifiedObject> values)
        {
            var lsNode =
                    new NativeFunction()
                    .Action((Args args) => {
                        return BuildEmptyValue();
                    })
                    .RegisterParameter<BoundFn>("nextVal")
                    .RegisterParameter<object>("data")
                    .ReturnsEmpty()
                    .Build();

            QualifiedObject previousNode = EmptyFunction();
            foreach (QualifiedObject obj in values.Reverse())
            {
                List<Argument> argsToBind = new List<Argument>();
                QualifiedObject nextVal = previousNode;
                argsToBind.Add(new Argument { Position = 0, Value = nextVal });
                argsToBind.Add(new Argument { Position = 1, Value = obj });

                BoundFn currentNode = new BoundFn
                {
                    hFunc = lsNode,
                    BoundArguments = new Args(lsNode.Params.PatchArguments(argsToBind))
                };
                previousNode = BuildFunction(currentNode);
            }

            return previousNode;
        }


        public static QualifiedObject BuildNamespace(_Storage.Environment value)
        {
            return new QualifiedObject { Object = value, Type = new DataType { TypeId = Enums.DataTypeEnum.Namespace } };
        }
    }
}
