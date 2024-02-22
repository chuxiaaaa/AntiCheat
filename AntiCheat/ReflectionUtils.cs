using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiCheat
{
    public static class ReflectionUtils
    {

        public static T2 GetFieldValue<T,T2>(this T obj,string name)
        {
            return (T2)obj.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(obj);
        }
    }
}
