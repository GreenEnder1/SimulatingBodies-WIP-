using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class UIElementsEditorWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/UIElementsEditorWindow")]
    public static void ShowExample()
    {
        UIElementsEditorWindow wnd = GetWindow<UIElementsEditorWindow>();
        wnd.titleContent = new GUIContent("UIElementsEditorWindow");
    }

    public void CreateGUI()
    {
        StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("UIElementsStyles.uss");

        VisualElement mainContainer = new VisualElement();

        rootVisualElement.Add(mainContainer);
        rootVisualElement.styleSheets.Add(styleSheet);

        Label posTitle = new Label("Initial Position:");

        mainContainer.Add(posTitle);
        posTitle.AddToClassList("section-header");
        
        VisualElement posContainer = new VisualElement();

        mainContainer.Add(posContainer);

        TextField posX = new TextField();
        TextField posY = new TextField();
        TextField posZ = new TextField();
        Label posXLabel = new Label("X:");
        Label posYLabel = new Label("Y:");
        Label posZLabel = new Label("Z:");

        posX.AddToClassList("dark-textfield");
        posY.AddToClassList("dark-textfield");
        posZ.AddToClassList("dark-textfield");
        
        posContainer.AddToClassList("horizontal-container");

        posContainer.Add(posXLabel);
        posContainer.Add(posX);
        posContainer.Add(posYLabel);
        posContainer.Add(posY);
        posContainer.Add(posZLabel);
        posContainer.Add(posZ);

        Label velTitle = new Label("Initial Velocity:");

        mainContainer.Add(velTitle);
        velTitle.AddToClassList("section-header");
        
        VisualElement velContainer = new VisualElement();

        mainContainer.Add(velContainer);

        TextField velX = new TextField();
        TextField velY = new TextField();
        TextField velZ = new TextField();
        Label velXLabel = new Label("X:");
        Label velYLabel = new Label("Y:");
        Label velZLabel = new Label("Z:");

        velX.AddToClassList("dark-textfield");
        velY.AddToClassList("dark-textfield");
        velZ.AddToClassList("dark-textfield");
        
        velContainer.AddToClassList("horizontal-container");

        velContainer.Add(velXLabel);
        velContainer.Add(velX);
        velContainer.Add(velYLabel);
        velContainer.Add(velY);
        velContainer.Add(velZLabel);
        velContainer.Add(velZ);
        
        Button createObject = new Button() 
        {
            text = "Create Object"
        };
        mainContainer.Add(createObject);
    }
}