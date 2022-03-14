using Mochj.Builders;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models
{
    public class NativeList
    {
        private List<QualifiedObject> _ls = new List<QualifiedObject>();
        public DataType Type { get; set; }
        public NativeList(List<QualifiedObject> items)
        {
            _ls = items;
            Type = ExtractListType(_ls);
        }
        public NativeList(DataType internalType)
        {
            _ls = new List<QualifiedObject>();
            Type = new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = internalType };
        }

        private static DataType ExtractListType(List<QualifiedObject> ls)
        {
            if (!ls.Any()) { return new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = new DataType { TypeId = Enums.DataTypeEnum.Any } }; }
            DataType dt = ls.First().Type;
            foreach (QualifiedObject item in ls)
            {
                if (!dt.Equals(item.Type)) return new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = new DataType { TypeId = Enums.DataTypeEnum.Any } };
            }
            return new DataType { TypeId = Enums.DataTypeEnum.NativeList, ContainedType = dt };
        }

        public bool IsEmpty()
        {
            return _ls.Any();
        }

        public List<Ty> Get<Ty>()
        {
            return _ls.Select(x => TypeMediatorService.ToNativeType<Ty>(x)).ToList();
        }

        public void ForEach(Fn.Function fn)
        {
            foreach (var item in _ls)
            {
                fn.Call(fn.ResolveArguments(item));
            }
        }

        public QualifiedObject At(int index)
        {
            return _ls.ElementAt(index);
        }

        public Ty At<Ty>(int index)
        {
            return TypeMediatorService.ToNativeType<Ty>(_ls.ElementAt(index));
        }

        public void Remove(int index)
        {
            _ls.RemoveAt(index);
        }

        public void Remove(QualifiedObject item)
        {
            _ls.RemoveAll(x => x.Equals(item));
        }

        public void Add(QualifiedObject item)
        {
            _ls.Add(item);
        }
        public void AddRange(List<QualifiedObject> items)
        {
            _ls.AddRange(items);
        }

        public QualifiedObject Find(Fn.Function fn)
        {
            if (!fn.ReturnType.Is(Enums.DataTypeEnum.Boolean))
            {
                throw new Exception($"expect comparer to have return type {Enums.DataTypeEnum.Boolean} but got {fn.ReturnType}");
            }

            var item = _ls.Find(x => TypeMediatorService.ToNativeType<bool>(fn.Call(fn.ResolveArguments(x))));
            if (item == null)
            {
                return QualifiedObjectBuilder.BuildEmptyValue();
            }
            return item;
        }
    }
}
