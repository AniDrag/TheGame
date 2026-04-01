#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
// Sankyu GPT. I was ossing my brains here. Know this toomy 50% of my sanity today
public static class SerializedPropertyExtensions
{
    public static T GetValue<T>(this SerializedProperty property)
    {
        object obj = property.serializedObject.targetObject;
        string path = property.propertyPath;
        string[] parts = path.Split('.');
        foreach (string part in parts)
        {
            if (part == "Array") continue; // skip array entries
            // handle array indices
            if (part.Contains("data["))
            {
                string indexStr = part.Substring(part.IndexOf("[") + 1, part.IndexOf("]") - part.IndexOf("[") - 1);
                if (int.TryParse(indexStr, out int index))
                {
                    obj = ((System.Collections.IList)obj)[index];
                }
            }
            else
            {
                FieldInfo field = obj.GetType().GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null) return default(T);
                obj = field.GetValue(obj);
            }
        }
        return (T)obj;
    }
}
#endif