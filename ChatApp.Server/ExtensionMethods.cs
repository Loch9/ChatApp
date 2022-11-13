using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Server
{
    public static class ExtensionMethods
    {
        public static List<T> RemoveElement<T>(this List<T> list, T element)
        {
            list.Remove(element);
            return list;
        }
    }
}
