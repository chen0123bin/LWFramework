using UnityEditor;
using UnityEditor.UI;
[CustomEditor(typeof(RoundedImage), true)]
//[CanEditMultipleObjects]
public class RoundedImageEditor : ImageEditor
{

    SerializedProperty m_Radius;   
    SerializedProperty m_TriangleNum;
    SerializedProperty m_Sprite;
    SerializedProperty m_TopLeft;
    SerializedProperty m_TopRight;
    SerializedProperty m_BottomLeft;
    SerializedProperty m_BottomRight;
    protected override void OnEnable()
    {
        base.OnEnable();

        m_Sprite = serializedObject.FindProperty("m_Sprite");
        m_Radius = serializedObject.FindProperty("Radius");
        m_TriangleNum = serializedObject.FindProperty("TriangleNum");
        m_TopLeft = serializedObject.FindProperty("topLeft");
        m_TopRight = serializedObject.FindProperty("topRight");
        m_BottomLeft = serializedObject.FindProperty("bottomLeft");
        m_BottomRight = serializedObject.FindProperty("bottomRight");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SpriteGUI();
        AppearanceControlsGUI();
        RaycastControlsGUI();
        bool showNativeSize = m_Sprite.objectReferenceValue != null;
        m_ShowNativeSize.target = showNativeSize;
        NativeSizeButtonGUI();
        EditorGUILayout.PropertyField(m_Radius);
        EditorGUILayout.PropertyField(m_TriangleNum);
        EditorGUILayout.PropertyField(m_TopLeft);
        EditorGUILayout.PropertyField(m_TopRight);
        EditorGUILayout.PropertyField(m_BottomLeft);
        EditorGUILayout.PropertyField(m_BottomRight);
        this.serializedObject.ApplyModifiedProperties();
    }
}
