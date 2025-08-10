using UnityEditor;

namespace AiaalTools.Serialization.Editor
{
    [CustomPropertyDrawer(typeof(DictionaryStringString))]
    [CustomPropertyDrawer(typeof(DictionaryIntGameObject))]
    public class AnySerializeDicPropertyDrawer : SerializableDictionaryPropertyDrawer { }
}