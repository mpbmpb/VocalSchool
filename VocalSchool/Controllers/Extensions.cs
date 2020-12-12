using System;
using System.Threading.Tasks;
using VocalSchool.Data;
using VocalSchool.Models;

namespace VocalSchool.Controllers
{
    public static class Extensions
    {
        public static string Prepend(this string s, string pre) => $"{pre} {s}";

        public static async Task<T> CopyAndPrependNameWithAsync<T>(this T original, string uid, DbHandler db)
            where T : ICourseElement, new()
        {
            var reqReading = original.GetType().GetProperty("RequiredReading");
            var copy = new T();

            copy.Name = original.Name.Prepend(uid);
            copy.Description = original.Description;
                        
            if (reqReading != null) 
                reqReading.SetValue(copy, reqReading.GetValue(original));
            
            await db.AddAsync(copy);
            return copy;
        }

        public static async Task CreateMany2ManyAsync<T>(this T entity, int id0, int id1, DbHandler db)
            where T : IMany2Many, new()
        {
            var m2m = new T {[0] = id0, [1] = id1};
            await db.AddAsync(m2m);
        }

        public static string GetUid(this string s)
        {
            var len = s.IndexOf(']') + 1;
            if (s[0] != '[' || len == 0) // if len == 0 IndexOf was -1 
                return string.Empty;

            return s.Substring(0, len);
        }
        
        public static string GetUid<T>(this T courseElement) where T : ICourseElement
            => courseElement.Name.GetUid();

        public static T TrimUid<T>(this T courseElement) where T : ICourseElement
        {
            var start = courseElement.Name.GetUid().Length;
            courseElement.Name = courseElement.Name.Substring(start == 0 ? 0 : ++start); // ++ takes off whitespace after uid
            return courseElement;
        }
        
    }
}