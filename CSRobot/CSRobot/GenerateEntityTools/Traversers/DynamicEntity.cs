using System;
using System.Collections.Generic;
using System.Dynamic;


namespace CSRobot.GenerateEntityTools.Traversers
{
    /// <summary>
    /// 动态实体
    /// </summary>
    public class DynamicEntity : DynamicObject
    {
        private Dictionary<string, object> _values;
        public DynamicEntity()
        {
            _values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_values.ContainsKey(binder.Name))
            {
                result = _values[binder.Name];
            }
            else
            {
                throw new System.MissingMemberException("The property " + binder.Name + " does not exist");
            }

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetMember(binder.Name, value);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _values.Keys;
        }

        internal void SetMember(string propertyName, object value)
        {
            if (object.ReferenceEquals(value, DBNull.Value))
            {
                //这里处理成字符串的空
                _values[propertyName] = "";
            }
            else
            {
                _values[propertyName] = value;
            }
        }
    }
}
