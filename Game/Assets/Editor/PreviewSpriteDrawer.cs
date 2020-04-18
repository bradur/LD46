using UnityEngine;
using System.Collections.Generic;

namespace UnityEditor
{

    [CustomPropertyDrawer(typeof(PreviewSpriteAttribute))]
    public class PreviewSpriteDrawer : PropertyDrawer
    {
        const float imageHeight = 100;
        const int size = 5;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference &&
                (property.objectReferenceValue as Sprite) != null)
            {
                return EditorGUI.GetPropertyHeight(property, label, true) + imageHeight + 10;
            }
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        static string GetPath(SerializedProperty property)
        {
            string path = property.propertyPath;
            int index = path.LastIndexOf(".");
            return path.Substring(0, index + 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Draw the normal property field
            EditorGUI.PropertyField(position, property, label, true);
            float defaultPropertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var sprite = property.objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    position.y += defaultPropertyHeight + 5;
                    position.height = imageHeight;

                    DrawTexturePreview(position, sprite);
                }
            }
            else if (property.isArray)
            {
                SerializedProperty sp = property.Copy();
                int arrayLength = 0;

                sp.Next(true); // skip generic field
                sp.Next(true); // advance to array size field

                // Get the array size
                arrayLength = sp.intValue;

                sp.Next(true); // advance to first array index

                // Write values to list
                //List<Sprite> sprites = new List<Sprite>(arrayLength);
                int lastIndex = arrayLength - 1;
                for (int i = 0; i < arrayLength; i++)
                {
                    var sprite = property.objectReferenceValue as Sprite;
                    if (sprite != null)
                    {
                        float yPos = i % size * imageHeight;
                        float xPos = i * imageHeight;
                        position.x = xPos;
                        position.y += yPos + defaultPropertyHeight;
                        position.height = imageHeight;

                        DrawTexturePreview(position, sprite);
                    }
                    if (i < lastIndex) sp.Next(false); // advance without drilling into children
                }

            }
        }

        private void DrawTexturePreview(Rect position, Sprite sprite)
        {
            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
    }
}