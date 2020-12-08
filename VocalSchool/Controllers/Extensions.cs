using VocalSchool.Models;

namespace VocalSchool.Controllers
{
    public static class Extensions
    {
        public static string Prepend(this string s, string pre)
        {
            return $"{pre} {s}";
        }

        public static T CopyAndPrependNameWith<T>(this T original, string uid)
            where T : ICourseElement, new()
        {
            var reqReading = original.GetType().GetProperty("RequiredReading");
            var copy = new T();

            copy.Name = original.Name.Prepend(uid);
            copy.Description = original.Description;
                        
            if (reqReading != null) 
                reqReading.SetValue(copy, reqReading.GetValue(original, null), null);

            return copy;
        }
    }
}