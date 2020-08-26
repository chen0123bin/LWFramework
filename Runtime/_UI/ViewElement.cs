using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ViewElement : MonoBehaviour
{
    public GameObject[] gameObjects;
    public virtual string ComponentName
    {
        get
        {
            if (null != GetComponent<ScrollRect>())
                return "ScrollRect";
            if (null != GetComponent<InputField>())
                return "InputField";
            if (null != GetComponent<Text>())
                return "Text";
            if (null != GetComponent<Button>())
                return "Button";
            if (null != GetComponent<RawImage>())
                return "RawImage";
            if (null != GetComponent<Toggle>())
                return "Toggle";
            if (null != GetComponent<Slider>())
                return "Slider";
            if (null != GetComponent<Scrollbar>())
                return "Scrollbar";
            if (null != GetComponent<Image>())
                return "Image";
            if (null != GetComponent<ToggleGroup>())
                return "ToggleGroup";
            if (null != GetComponent<Animator>())
                return "Animator";
            if (null != GetComponent<Canvas>())
                return "Canvas";
            if (null != GetComponent("Empty4Raycast"))
                return "Empty4Raycast";
            if (null != GetComponent<RectTransform>())
                return "RectTransform";

            return "Transform";


        }
    }
    [FolderPath]
    public string savePath = "Scripts";
    [Button("创建View脚本")]
    public void CreateScriptBtn()
    {
        string viewName = gameObject.name;
        string generateFilePath = savePath + "/" + viewName + ".cs";
        var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
        var strBuilder = new StringBuilder();

        strBuilder.AppendLine("using LWFramework.UI;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("using UnityEngine;");
        strBuilder.AppendLine();
        strBuilder.AppendFormat("[UIViewData(\"\",FindType.Name,\"\")]");
        strBuilder.AppendLine();
        strBuilder.AppendFormat("public class {0} : BaseUIView ", viewName);
        strBuilder.AppendLine();
        strBuilder.AppendLine("{");
        strBuilder.AppendLine();
        //获取view上的组建
        ViewElement viewElement = gameObject.GetComponent<ViewElement>();
        //定义控件属性
        if (viewElement)
        {
            foreach (var item in viewElement.gameObjects)
            {
                string childName = item.name;
                string componentName = GetComponetName(item);
                strBuilder.AppendFormat("\t[UIElement(\"{0}\")]", GetParentPath(gameObject, item, ""));
                strBuilder.AppendLine();
                strBuilder.AppendFormat("\tpublic {0} _{1};", componentName, childName);
                strBuilder.AppendLine();
            }
        }
        strBuilder.AppendLine("\tpublic override  void OnCreateView()");
        strBuilder.AppendLine("\t{");
        List<string> buttons = new List<string>();
        //获取ui控件
        if (viewElement)
        {
            foreach (var item in viewElement.gameObjects)
            {
                string childName = item.name;
                string componentName = GetComponetName(item);

                //添加按钮点击事件监听
                if (componentName == "Button")
                {
                    strBuilder.AppendFormat("\t\t_{0}.onClick.AddListener(() => ", childName);
                    strBuilder.AppendLine("\t\t{");
                    strBuilder.AppendLine();
                    strBuilder.AppendLine("\t\t});");
                    strBuilder.AppendLine();
                    buttons.Add(childName);
                }
            }
        }

        strBuilder.AppendLine("\t}");


        strBuilder.AppendLine("}");
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    [Button("创建ViewLogic脚本")]
    public void CreateViewLogicScriptBtn()
    {
        string viewName = gameObject.name;
        string logicName = viewName + "Logic";
        CreateView(viewName, logicName);
        CreateLogic(viewName, logicName);
    }
    [Button("创建Node脚本")]
    public void CreateNodeScriptBtn()
    {
        string behaviourName = gameObject.name;
        string generateFilePath = savePath + "/" + behaviourName + ".cs";
        var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
        var strBuilder = new StringBuilder();

        strBuilder.AppendLine("using LWFramework.UI;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("using UnityEngine;");
        strBuilder.AppendLine();
        strBuilder.AppendFormat("public class {0} : BaseUINode ", behaviourName);
        strBuilder.AppendLine();
        strBuilder.AppendLine("{");
        strBuilder.AppendLine();
        //获取view上的组建
        ViewElement viewElement = gameObject.GetComponent<ViewElement>();
        //定义控件属性
        if (viewElement)
        {
            foreach (var item in viewElement.gameObjects)
            {
                string childName = item.name;
                string componentName = GetComponetName(item);
                strBuilder.AppendFormat("\t[UIElement(\"{0}\")]", GetParentPath(gameObject, item, ""));
                strBuilder.AppendLine();
                strBuilder.AppendFormat("\tpublic {0} _{1};", componentName, childName);
                strBuilder.AppendLine();
            }
        }
        strBuilder.AppendLine("\tpublic override  void Create(GameObject gameObject)");
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("\t\tbase.Create(gameObject);");
        List<string> buttons = new List<string>();
        //获取ui控件
        if (viewElement)
        {
            foreach (var item in viewElement.gameObjects)
            {
                string childName = item.name;
                string componentName = GetComponetName(item);

                //添加按钮点击事件监听
                if (componentName == "Button")
                {
                    strBuilder.AppendFormat("\t\t_{0}.onClick.AddListener(() => ", childName);
                    strBuilder.AppendLine("\t\t{");
                    strBuilder.AppendLine();
                    strBuilder.AppendLine("\t\t});");
                    strBuilder.AppendLine();
                    buttons.Add(childName);
                }
            }
        }

        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("\tpublic override void OnUnSpawn()");
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("\tpublic override void Release()");
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("\t\tbase.Release();");
        strBuilder.AppendLine("\t}");

        strBuilder.AppendLine("}");
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    [LabelText("缩放系数")]
    public string size = "";
    [Button("重设大小")]
    public void ResetSize()
    {
        float sizeValue = float.Parse(size);
        RectTransform rectTransform = transform as RectTransform;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * sizeValue, rectTransform.sizeDelta.y * sizeValue);
    }

    public string GetComponetName(GameObject gameObject)
    {
        if (null != gameObject.GetComponent<ScrollRect>())
            return "ScrollRect";
        else if (null != gameObject.GetComponent<InputField>())
            return "InputField";
        else if (null != gameObject.GetComponent<Button>())
            return "Button";
        else if (null != gameObject.GetComponent<Image>())
            return "Image";
        else if (null != gameObject.GetComponent<Text>())
            return "Text";
        else if (null != gameObject.GetComponent<Toggle>())
            return "Toggle";
        return "Transform";
    }
    string GetParentPath(GameObject gameObject, GameObject child, string str)
    {
        if (child.transform.parent == gameObject.transform)
        {
            str = child.name + str;
            return str;
        }
        else
        {
            str = "/" + child.name + str;
            return GetParentPath(gameObject, child.transform.parent.gameObject, str);
        }

    }
    void CreateView(string viewName, string logicName)
    {
        string generateFilePath = savePath + "/" + viewName + ".cs";
        var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("using LWFramework.UI;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("using UnityEngine;");
        strBuilder.AppendLine();
        strBuilder.AppendFormat("[UIViewData(\"\",FindType.Name,\"\")]");
        strBuilder.AppendLine();
        strBuilder.AppendFormat("public class {0} : BaseLogicUIView<{1}>  ", viewName, logicName);
        strBuilder.AppendLine();
        strBuilder.AppendLine("{");
        strBuilder.AppendLine();
        //获取view上的组建
        ViewElement viewElement = gameObject.GetComponent<ViewElement>();
        //定义控件属性
        if (viewElement)
        {
            foreach (var item in viewElement.gameObjects)
            {
                string childName = item.name;
                string componentName = GetComponetName(item);
                strBuilder.AppendFormat("\t[UIElement(\"{0}\")]", GetParentPath(gameObject, item, ""));
                strBuilder.AppendLine();
                strBuilder.AppendFormat("\tpublic {0} _{1};", componentName, childName);
                strBuilder.AppendLine();
            }
        }
        strBuilder.AppendLine("\tpublic override  void OnCreateView()");
        strBuilder.AppendLine("\t{");
        List<string> buttons = new List<string>();
        //获取ui控件
        if (viewElement)
        {
            foreach (var item in viewElement.gameObjects)
            {
                string childName = item.name;
                string componentName = GetComponetName(item);

                //添加按钮点击事件监听
                if (componentName == "Button")
                {
                    strBuilder.AppendFormat("\t\t_{0}.onClick.AddListener(() => ", childName);
                    strBuilder.AppendLine("\t\t{");
                    strBuilder.AppendLine();
                    strBuilder.AppendLine("\t\t});");
                    strBuilder.AppendLine();
                    buttons.Add(childName);
                }
            }
        }

        strBuilder.AppendLine("\t}");


        strBuilder.AppendLine("}");
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    void CreateLogic(string viewName, string logicName)
    {
        string generateFilePath = savePath + "/" + logicName + ".cs";
        var sw = new StreamWriter(generateFilePath, false, Encoding.UTF8);
        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("using LWFramework.UI;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("using UnityEngine;");
        strBuilder.AppendLine();
        strBuilder.AppendFormat("public class {0} : BaseUILogic<{1}>  ", logicName, viewName);
        strBuilder.AppendLine();
        strBuilder.AppendLine("{");
        strBuilder.AppendLine();

        strBuilder.AppendFormat("\tpublic {0}({1} view): base(view)", logicName, viewName);
        strBuilder.AppendLine();
        strBuilder.AppendLine("\t{");
        //strBuilder.AppendLine("\t\tbase.CreateView(gameObject);");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}
