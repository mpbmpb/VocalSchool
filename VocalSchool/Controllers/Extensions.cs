using System.Reflection;

namespace VocalSchool.Controllers
{
    public static class Extensions
    {
        public static string Prepend(this string s, string pre) => $"{pre} {s}";
        
        public static T CopyAndPrependNameWith<T>(this T original, string uid) 
            where T : new()
        {
            var copy = new T();
            var name = original.GetType().GetProperty("Name");
            var description = original.GetType().GetProperty("Description");
            var reqReading = original.GetType().GetProperty("RequiredReading");
            if (name != null)
            {
                name.SetValue(copy, name.GetValue(original,  null)?.ToString().Prepend(uid), null);
            }

            if (description != null)
            {
                description.SetValue(copy, description.GetValue(original,  null), null);
            }
            
            if (reqReading != null)
            {
                reqReading.SetValue(copy, reqReading.GetValue(original,  null), null);
            }
            
            return copy;
        }
    }
    
    
}