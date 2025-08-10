using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace AiaalTools.Serialization.Editor
{
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer 
    {
        private const string keysFieldName = "_keys";
        private const string valuesFieldName = "_values";

        private static GUIContent _iconPlus = IconContent("Toolbar Plus", "Add entry");
        private static GUIContent _iconMinus = IconContent("Toolbar Minus", "Remove entry");
        private static GUIContent _warningIconConflict = IconContent("console.warnicon.sml", "Conflicting key, this entry will be lost");
        private static GUIContent _warningIconOther = IconContent("console.infoicon.sml", "Conflicting key");
        private static GUIContent _warningIconNull = IconContent("console.warnicon.sml", "Null key, this entry will be lost");
        private static GUIStyle _buttonStyle = GUIStyle.none;

        private object _conflictKey = null;
        private object _conflictValue = null;
        private int _conflictIndex = -1;
        private int _conflictOtherIndex = -1;
        private bool _conflictKeyPropertyExpanded = false;
        private bool _conflictValuePropertyExpanded = false;
        private float _conflictLineHeight = 0f;

        private enum Action 
        {
            None,
            Add,
            Remove
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
        {
            label = EditorGUI.BeginProperty(position, label, property);

            Action buttonAction = Action.None;
            int buttonActionIndex = 0;

            var keyArrayProperty = property.FindPropertyRelative(keysFieldName);
            var valueArrayProperty = property.FindPropertyRelative(valuesFieldName);

            if(_conflictIndex != -1) 
            {
                keyArrayProperty.InsertArrayElementAtIndex(_conflictIndex);
                var keyProperty = keyArrayProperty.GetArrayElementAtIndex(_conflictIndex);
                SetPropertyValue(keyProperty, _conflictKey);
                keyProperty.isExpanded = _conflictKeyPropertyExpanded;

                valueArrayProperty.InsertArrayElementAtIndex(_conflictIndex);
                var valueProperty = valueArrayProperty.GetArrayElementAtIndex(_conflictIndex);
                SetPropertyValue(valueProperty, _conflictValue);
                valueProperty.isExpanded = _conflictValuePropertyExpanded;
            }

            var buttonWidth = _buttonStyle.CalcSize(_iconPlus).x;

            var labelPosition = position;
            labelPosition.height = EditorGUIUtility.singleLineHeight;
            if(property.isExpanded)
                labelPosition.xMax -= _buttonStyle.CalcSize(_iconPlus).x;

            EditorGUI.PropertyField(labelPosition, property, label, false);
            if(property.isExpanded) 
            {
                var buttonPosition = position;
                buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
                buttonPosition.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.BeginDisabledGroup(_conflictIndex != -1);
                if(GUI.Button(buttonPosition, _iconPlus, _buttonStyle)) 
                {
                    buttonAction = Action.Add;
                    buttonActionIndex = keyArrayProperty.arraySize;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel++;
                var linePosition = position;
                linePosition.y += EditorGUIUtility.singleLineHeight;

                foreach(var entry in EnumerateEntries(keyArrayProperty, valueArrayProperty)) 
                {
                    var keyProperty = entry.keyProperty;
                    var valueProperty = entry.valueProperty;
                    int i = entry.index;

                    float labelWidth = EditorGUIUtility.labelWidth;

                    float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                    var keyPosition = linePosition;
                    keyPosition.height = keyPropertyHeight;
                    keyPosition.xMin = 0;
                    keyPosition.xMax = labelWidth;
                    EditorGUIUtility.labelWidth = labelWidth * keyPosition.width / linePosition.width;
                    EditorGUI.PropertyField(keyPosition, keyProperty, GUIContent.none, true);

                    float valuePropertyHeight = EditorGUI.GetPropertyHeight(valueProperty);
                    var valuePosition = linePosition;
                    valuePosition.height = valuePropertyHeight;
                    valuePosition.xMin = labelWidth;
                    valuePosition.xMax -= buttonWidth;
                    EditorGUIUtility.labelWidth = labelWidth * valuePosition.width / linePosition.width;
                    EditorGUI.PropertyField(valuePosition, valueProperty, GUIContent.none, true);

                    EditorGUIUtility.labelWidth = labelWidth;

                    buttonPosition = linePosition;
                    buttonPosition.xMin = buttonPosition.xMax - buttonWidth;
                    buttonPosition.height = EditorGUIUtility.singleLineHeight;
                    if(GUI.Button(buttonPosition, _iconMinus, _buttonStyle)) 
                    {
                        buttonAction = Action.Remove;
                        buttonActionIndex = i;
                    }

                    if(i == _conflictIndex && _conflictOtherIndex == -1) 
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = _buttonStyle.CalcSize(_warningIconNull);
                        GUI.Label(iconPosition, _warningIconNull);
                    } 
                    else if(i == _conflictIndex) 
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = _buttonStyle.CalcSize(_warningIconConflict);
                        GUI.Label(iconPosition, _warningIconConflict);
                    } 
                    else if(i == _conflictOtherIndex) 
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = _buttonStyle.CalcSize(_warningIconOther);
                        GUI.Label(iconPosition, _warningIconOther);
                    }

                    float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
                    linePosition.y += lineHeight;
                }

                EditorGUI.indentLevel--;
            }

            if(buttonAction == Action.Add) 
            {
                keyArrayProperty.InsertArrayElementAtIndex(buttonActionIndex);
                valueArrayProperty.InsertArrayElementAtIndex(buttonActionIndex);
            } 
            else if(buttonAction == Action.Remove) 
            {
                DeleteArrayElementAtIndex(keyArrayProperty, buttonActionIndex);
                DeleteArrayElementAtIndex(valueArrayProperty, buttonActionIndex);
            }

            _conflictKey = null;
            _conflictValue = null;
            _conflictIndex = -1;
            _conflictOtherIndex = -1;
            _conflictLineHeight = 0f;
            _conflictKeyPropertyExpanded = false;
            _conflictValuePropertyExpanded = false;

            foreach(var entry1 in EnumerateEntries(keyArrayProperty, valueArrayProperty)) 
            {
                var keyProperty1 = entry1.keyProperty;
                int i = entry1.index;
                object keyProperty1Value = GetPropertyValue(keyProperty1);

                if(keyProperty1Value == null) 
                {
                    var valueProperty1 = entry1.valueProperty;
                    SaveProperty(keyProperty1, valueProperty1, i, -1);
                    DeleteArrayElementAtIndex(valueArrayProperty, i);
                    DeleteArrayElementAtIndex(keyArrayProperty, i);

                    break;
                }


                foreach(var entry2 in EnumerateEntries(keyArrayProperty, valueArrayProperty, i + 1)) 
                {
                    var keyProperty2 = entry2.keyProperty;
                    int j = entry2.index;
                    object keyProperty2Value = GetPropertyValue(keyProperty2);

                    if(object.Equals(keyProperty1Value, keyProperty2Value)) 
                    {
                        var valueProperty2 = entry2.valueProperty;
                        SaveProperty(keyProperty2, valueProperty2, j, i);
                        DeleteArrayElementAtIndex(keyArrayProperty, j);
                        DeleteArrayElementAtIndex(valueArrayProperty, j);

                        goto breakLoops;
                    }
                }
            }
            breakLoops:

            EditorGUI.EndProperty();
        }

        private void SaveProperty(SerializedProperty keyProperty, SerializedProperty valueProperty, int index, int otherIndex) 
        {
            _conflictKey = GetPropertyValue(keyProperty);
            _conflictValue = GetPropertyValue(valueProperty);
            float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
            float valuePropertyHeight = EditorGUI.GetPropertyHeight(valueProperty);
            float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
            _conflictLineHeight = lineHeight;
            _conflictIndex = index;
            _conflictOtherIndex = otherIndex;
            _conflictKeyPropertyExpanded = keyProperty.isExpanded;
            _conflictValuePropertyExpanded = valueProperty.isExpanded;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
        {
            float propertyHeight = EditorGUIUtility.singleLineHeight;

            if(property.isExpanded) 
            {
                var keysProperty = property.FindPropertyRelative(keysFieldName);
                var valuesProperty = property.FindPropertyRelative(valuesFieldName);

                foreach(var entry in EnumerateEntries(keysProperty, valuesProperty)) 
                {
                    var keyProperty = entry.keyProperty;
                    var valueProperty = entry.valueProperty;
                    float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                    float valuePropertyHeight = EditorGUI.GetPropertyHeight(valueProperty);
                    float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
                    propertyHeight += lineHeight;
                }

                if(_conflictIndex != -1) 
                {
                    propertyHeight += _conflictLineHeight;
                }
            }

            return propertyHeight;
        }

        static Dictionary<SerializedPropertyType, PropertyInfo> ms_serializedPropertyValueAccessorsDict;

        static SerializableDictionaryPropertyDrawer() 
        {
            Dictionary<SerializedPropertyType, string> serializedPropertyValueAccessorsNameDict = new Dictionary<SerializedPropertyType, string>() {
            { SerializedPropertyType.Integer, "intValue" },
            { SerializedPropertyType.Boolean, "boolValue" },
            { SerializedPropertyType.Float, "floatValue" },
            { SerializedPropertyType.String, "stringValue" },
            { SerializedPropertyType.Color, "colorValue" },
            { SerializedPropertyType.ObjectReference, "objectReferenceValue" },
            { SerializedPropertyType.LayerMask, "intValue" },
            { SerializedPropertyType.Enum, "intValue" },
            { SerializedPropertyType.Vector2, "vector2Value" },
            { SerializedPropertyType.Vector3, "vector3Value" },
            { SerializedPropertyType.Vector4, "vector4Value" },
            { SerializedPropertyType.Rect, "rectValue" },
            { SerializedPropertyType.ArraySize, "intValue" },
            { SerializedPropertyType.Character, "intValue" },
            { SerializedPropertyType.AnimationCurve, "animationCurveValue" },
            { SerializedPropertyType.Bounds, "boundsValue" },
            { SerializedPropertyType.Quaternion, "quaternionValue" },
        };
            Type serializedPropertyType = typeof(SerializedProperty);

            ms_serializedPropertyValueAccessorsDict = new Dictionary<SerializedPropertyType, PropertyInfo>();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            foreach(var kvp in serializedPropertyValueAccessorsNameDict) 
            {
                PropertyInfo propertyInfo = serializedPropertyType.GetProperty(kvp.Value, flags);
                ms_serializedPropertyValueAccessorsDict.Add(kvp.Key, propertyInfo);
            }
        }

        private static GUIContent IconContent(string name, string tooltip) 
        {
            var builtinIcon = EditorGUIUtility.IconContent(name);
            return new GUIContent(builtinIcon.image, tooltip);
        }

        private static void DeleteArrayElementAtIndex(SerializedProperty arrayProperty, int index) 
        {
            var property = arrayProperty.GetArrayElementAtIndex(index);
            if(property.propertyType == SerializedPropertyType.ObjectReference) 
            {
                property.objectReferenceValue = null;
            }

            arrayProperty.DeleteArrayElementAtIndex(index);
        }

        public static object GetPropertyValue(SerializedProperty p) 
        {
            PropertyInfo propertyInfo;
            if (ms_serializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out propertyInfo)) 
            {
                return propertyInfo.GetValue(p, null);
            } 
            else 
            {
                if (p.isArray) return GetPropertyValueArray(p);
                else return GetPropertyValueGeneric(p);

            }
        }

        private static void SetPropertyValue(SerializedProperty p, object v) 
        {
            PropertyInfo propertyInfo;
            if(ms_serializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out propertyInfo)) 
            {
                propertyInfo.SetValue(p, v, null);
            } 
            else 
            {
                if (p.isArray) SetPropertyValueArray(p, v);
                else SetPropertyValueGeneric(p, v);

            }
        }

        private static object GetPropertyValueArray(SerializedProperty property) 
        {
            object[] array = new object[property.arraySize];
            for(int i = 0; i < property.arraySize; i++) 
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                array[i] = GetPropertyValue(item);
            }
            return array;
        }

        private static object GetPropertyValueGeneric(SerializedProperty property) 
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var iterator = property.Copy();
            if(iterator.Next(true)) 
            {
                var end = property.GetEndProperty();
                do 
                {
                    string name = iterator.name;
                    object value = GetPropertyValue(iterator);
                    dict.Add(name, value);
                } 
                while(iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }
            return dict;
        }

        private static void SetPropertyValueArray(SerializedProperty property, object v) 
        {
            object[] array = (object[])v;
            property.arraySize = array.Length;
            for(int i = 0; i < property.arraySize; i++) 
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                SetPropertyValue(item, array[i]);
            }
        }

        private static void SetPropertyValueGeneric(SerializedProperty property, object v) 
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)v;
            var iterator = property.Copy();
            if(iterator.Next(true)) 
            {
                var end = property.GetEndProperty();
                do 
                {
                    string name = iterator.name;
                    SetPropertyValue(iterator, dict[name]);
                } 
                while(iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }
        }

        private struct EnumerationEntry 
        {
            public SerializedProperty keyProperty;
            public SerializedProperty valueProperty;
            public int index;

            public EnumerationEntry(SerializedProperty keyProperty, SerializedProperty valueProperty, int index) 
            {
                this.keyProperty = keyProperty;
                this.valueProperty = valueProperty;
                this.index = index;
            }
        }

        private static IEnumerable<EnumerationEntry> EnumerateEntries(SerializedProperty keyArrayProperty, SerializedProperty valueArrayProperty, int startIndex = 0) 
        {
            if(keyArrayProperty.arraySize > startIndex) 
            {
                int index = startIndex;
                var keyProperty = keyArrayProperty.GetArrayElementAtIndex(startIndex);
                var valueProperty = valueArrayProperty.GetArrayElementAtIndex(startIndex);
                var endProperty = keyArrayProperty.GetEndProperty();

                do 
                {
                    yield return new EnumerationEntry(keyProperty, valueProperty, index);
                    index++;
                } 
                while(keyProperty.Next(false) && valueProperty.Next(false) && !SerializedProperty.EqualContents(keyProperty, endProperty));
            }
        }
    }
}