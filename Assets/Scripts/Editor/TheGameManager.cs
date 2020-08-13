﻿using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;

public class TheGameManager : OdinMenuEditorWindow
{   
    [OnValueChanged("StateChange")]
    [LabelText("Manager View")]
    [LabelWidth(100f)]
    [EnumToggleButtons] 
    [ShowInInspector]
    private ManagerState managerState;
    private int enumIndex = 0;
    private bool treeRebuild = false;

    // Create field for each type of scriptable object in project to be drawn
    private DrawSelected<EnemyDataSO> drawEnemies = new DrawSelected<EnemyDataSO>();
    private DrawSelected<CardDataSO> drawCards = new DrawSelected<CardDataSO>();

    // Create field for each type of manager object in project to be drawn
    private DrawTestSceneManager drawTestSceneManager = new DrawTestSceneManager();

    // Hard coded file directory paths to specific SO's
    private string enemyPath = "Assets/SO Assets/Enemies";
    private string cardPath = "Assets/SO Assets/Cards";

    [MenuItem("Tools/The Game Manager")]
    public static void OpenWindow()
    {
        GetWindow<TheGameManager>().Show();
    }
    private void StateChange()
    {
        // Listens to changes to the variable 'managerState'
        // via the event listener attribute 'OnPropertyChanged'.
        // clicking on an enum toggle button triggers this function, which
        // signals that the menu tree for that page needs to be rebuilt
        treeRebuild = true;
    }
    protected override void Initialize()
    {
        // Set SO directory folder paths
        drawEnemies.SetPath(enemyPath);
        drawCards.SetPath(cardPath);

        // Find manager objects
        drawTestSceneManager.FindMyObject();
    }
    protected override void OnGUI()
    {
        // Did we toggle to a new page? 
        // Should we rebuild the menu tree?
        if(treeRebuild && Event.current.type == EventType.Layout)
        {
            ForceMenuTreeRebuild();
            treeRebuild = false;
        }

        SirenixEditorGUI.Title("The Game Manager", "Heroes Of Herp Derp", TextAlignment.Center, true);
        EditorGUILayout.Space();

        switch (managerState)
        {
            case ManagerState.enemies:
            case ManagerState.items:
            case ManagerState.cards:
                DrawEditor(enumIndex);
                break;
            default:
                break;
        }

        EditorGUILayout.Space();
        base.OnGUI();
    }
    protected override void DrawEditors()
    {
        // Which target should the window draw?
        // in cases where SO's need to be drawn, do SetSelected();
        // this takes the selected value from the menu tree, then
        // then draws it in the main window for editing

        switch (managerState)
        {
            case ManagerState.testTab:
                DrawEditor(enumIndex);
                break;
            case ManagerState.enemies:
                drawEnemies.SetSelected(MenuTree.Selection.SelectedValue);
                break;
            case ManagerState.items:
                break;
            case ManagerState.cards:
                drawCards.SetSelected(MenuTree.Selection.SelectedValue);
                break;

        }

        // Which editor window should be drawn?
        // just cast the enum value as int to be used as the index
        DrawEditor((int)managerState);
    }
    protected override IEnumerable<object> GetTargets()
    {
        List<object> targets = new List<object>();

        // Targets must be added and drawn in the order
        // that the enum values are in!!
        // allows us to take advantage of the 
        // numerical value behind the enum values

        // Only draw for layouts that need to display scriptable objects
        // Otherwise, just add a null for managers
        targets.Add(drawTestSceneManager);
        targets.Add(drawEnemies);
        targets.Add(null);
        targets.Add(drawCards);
        targets.Add(base.GetTarget());

        enumIndex = targets.Count - 1;


        return targets;
    }
    protected override void DrawMenu()
    {
        switch (managerState)
        {
            case ManagerState.enemies:
            case ManagerState.items:
            case ManagerState.cards:
                base.DrawMenu();
                break;
            default:
                break;
        }
    }
    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();

        switch (managerState)
        {
            case ManagerState.cards:
                tree.AddAllAssetsAtPath("Card Data", cardPath, typeof(CardDataSO));
                break;
            case ManagerState.enemies:
                tree.AddAllAssetsAtPath("Enemy Data", enemyPath, typeof(EnemyDataSO));
                break;
        }
        return tree;
    }
    public enum ManagerState
    {
        testTab,
        enemies,
        items,
        cards,
    }


}

public class DrawSelected<T> where T: ScriptableObject
{
    // The SO selected in the menu tree
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public T selected;

    [LabelWidth(100)]
    [PropertyOrder(-1)]
    [ColorFoldoutGroup("CreateNew", 1f, 1f, 1f)]
    [HorizontalGroup("CreateNew/Horizontal")]
    public string nameForNew;

    // Directory path for saving the new SO
    private string path;

    [HorizontalGroup("CreateNew/Horizontal")]
    [GUIColor(0.5f, 1f, 0.5f)]
    [Button]
    public void CreateNew()
    {
        // Prevent creation of unnamed scriptable objects
        if(nameForNew == "")
        {
            return;
        }

        // Create SO and assign its name
        T newItem = ScriptableObject.CreateInstance<T>();
        newItem.name = "New " + typeof(T).ToString();

        // if the path was empty, just place the new SO
        // in the root asset folder
        if(path == "")
        {
            path = "Assets/";
        }

        // Create the new asset and save
        AssetDatabase.CreateAsset(newItem, path + "\\" + nameForNew + ".asset");
        AssetDatabase.SaveAssets();

        // Reset the name field: sets up the window for the next asset creation
        nameForNew = "";
    }

    [HorizontalGroup("CreateNew/Horizontal")]
    [GUIColor(1f, 0f, 0f)]
    [Button]
    public void DeleteSelected()
    {
        // make sure something is actually selected before 
        // trying to delete
        if(selected != null)
        {
            string _path = AssetDatabase.GetAssetPath(selected);
            AssetDatabase.DeleteAsset(_path);
            AssetDatabase.SaveAssets();
        }
    }

    public void SetSelected(object item)
    {
        // Check if object type is correct first because
        // Menu tree may not always have the correct type of object in it
        // (this can occur when using the enum toggle buttons to
        // change what objects are displayed)

        var attempt = item as T;
        if(attempt != null)
        {
            selected = attempt;
        }
    }

    // Used for setting up the directory path
    // that an SO is saved to.
    public void SetPath(string _path)
    {
        path = _path;
    }
}

public class ColorFoldoutGroupAttribute: PropertyGroupAttribute
{
    public float R, G, B, A;

    public ColorFoldoutGroupAttribute(string path) : base(path)
    {

    }

    public ColorFoldoutGroupAttribute(string path, float r, float g, float b, float a =1f): base(path)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    protected override void CombineValuesWith(PropertyGroupAttribute other)
    {
        var otherAttr = (ColorFoldoutGroupAttribute)other;

        R = Mathf.Max(otherAttr.R, R);
        G = Mathf.Max(otherAttr.G, G);
        B = Mathf.Max(otherAttr.B, B);
        A = Mathf.Max(otherAttr.A, A);
    }

}

public class ColorFoldoutGroupAttributeDrawer: OdinGroupDrawer<ColorFoldoutGroupAttribute>
{
    private LocalPersistentContext<bool> isExpanded;

    protected override void Initialize()
    {
        this.isExpanded = this.GetPersistentValue<bool>("ColorFoldoutGroupAttributeDrawer.isExpaned",
            GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
    }

    protected override void DrawPropertyLayout(GUIContent label)
    {
        GUIHelper.PushColor(new Color(Attribute.R, Attribute.G, Attribute.B, Attribute.A));
        SirenixEditorGUI.BeginBox();
        SirenixEditorGUI.BeginBoxHeader();
        GUIHelper.PopColor();

        isExpanded.Value = SirenixEditorGUI.Foldout(isExpanded.Value, label);
        SirenixEditorGUI.EndBoxHeader();


        if (SirenixEditorGUI.BeginFadeGroup(this, isExpanded.Value))
        {
            for (int i = 0; i < Property.Children.Count; i++)
            {
                Property.Children[i].Draw();
            }
        }
        SirenixEditorGUI.EndFadeGroup();
        SirenixEditorGUI.EndBox();
       
    }
}

public class DrawSceneObject<T> where T : MonoBehaviour
{
    // InlineEditor force window to draw the full class
    // Holds the reference the specific manager game object
    [Title("Universe Creator")]
    [ShowIf("@myObject != null")]    
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]    
    public T myObject;

    public void FindMyObject()
    {
        // Finds the manager game object in the scene
        if(myObject == null)
        {
            myObject = Object.FindObjectOfType<T>();
        }
    }

    // Button group forces manager specific buttons to be grouped together
    // at the top of the editor window
    [ShowIf("@myObject != null")]
    [GUIColor(0.7f, 1f, 0.7f)]
    [ButtonGroup("Top Button", -1000)]
    private void SelectSceneObject()
    {
        if(myObject != null)
        {
            Selection.activeObject = myObject.gameObject;
        }
    }

    [ShowIf("@myObject == null")]
    [Button]
    private void CreateManagerObject()
    {
        GameObject newManager = new GameObject();
        newManager.name = "New " + typeof(T).ToString();
        myObject = newManager.AddComponent<T>();
    }
}

public class DrawTestSceneManager: DrawSceneObject<CombatTestSceneController>
{
    // This class specifically draws a editor window
    // for the manager class 'CombatTestSceneController'

    [ShowIf("@myObject != null")]
    [GUIColor(0.7f, 1f, 0.7f)]
    [ButtonGroup("Top Button")]
    private void SomeFunction()
    {

    }
}


