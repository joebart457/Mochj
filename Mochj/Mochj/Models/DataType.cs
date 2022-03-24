using Mochj.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models
{
    public class DataType
    {
        public DataTypeEnum TypeId { get; set; }
        public DataType ContainedType { get; set; }

        public bool Is(DataTypeEnum dte)
        {
            return TypeId == dte;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((DataType)obj);

        }

        public bool Equals(DataType rhs)
        {
            if (rhs == null) return false;
            // 'Any' is a special type that always passes type checks
            if (TypeId == DataTypeEnum.Any) return true;
            if (TypeId == rhs.TypeId)
            {
                if (ContainedType == null)
                {
                    return rhs.ContainedType == null;
                }
                else
                {
                    return ContainedType.Equals(rhs.ContainedType);
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return TypeId.GetHashCode();
        }
        public override string ToString()
        {
            if (TypeId != DataTypeEnum.NativeList)
            {
                return $"{TypeId}";
            }
            return $"{TypeId}<{ContainedType}>";
        }
    }
}
