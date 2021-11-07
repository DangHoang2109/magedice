#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Cosina.Components;
using UnityEditor;
using UnityEngine;


public class CoLogEditor : EditorWindow
{
    private static CoLogEditor _instance;
    private static Color COLOR_BLACK = Color.black;
    private static Color COLOR_WHITE = Color.white;
    private static Color COLOR_DARK_GREY = new Color(0.2f, 0.2f, 0.2f, 1f);
    private static Color COLOR_LESS_DARK_GREY = new Color(0.3f, 0.3f, 0.3f, 1f);
    private static Color COLOR_SLIGHT_DARK_GREY = new Color(0.4f, 0.4f, 0.4f, 1f);
    private static Color COLOR_LIGHT_GREY = new Color(0.8f, 0.8f, 0.8f, 1f);

    private List<string> _s;
    private string sFilterer;
    private List<string> sFiltered;
    private List<string> sDisplaying;

    private string sInspect;
    private Vector2 _scroll;
    private Vector2 _scrollInspect;
    
    //private GUIStyle _richText1;
    private GUIStyle _styleText;
    private GUIStyle _styleInspect;
    private GUIStyle _styleStatus;

    private int _countOperation = 50;

    private string sStatus = "Just opened the dialog";
    
    [MenuItem("Tools/Show CoLog")]
    public static void OnShow()
    {
        CoLogEditor wnd = GetWindow<CoLogEditor>();
        wnd.titleContent = new GUIContent("CoLog");
    }
    
    // enable 2 of this in the case the editor window error (not shown)
    // [MenuItem("Tools/Close Custom Log")]
    // public static void OnClose()
    // {
    //     if(_instance != null)
    //         _instance.Close();
    // }
    //
    // private void OnEnable()
    // {
    //     _instance = this;
    // }

    private void OnFirstRun()
    {
        // _richText1 = new GUIStyle(GUI.skin.label);
        // _richText1.richText = true;
        // _richText1.normal.background = CosinaTools.CreateTextureWithColor(COLOR_BLACK);
        // _richText1.normal.textColor = COLOR_WHITE;

        _styleText = new GUIStyle(GUI.skin.button);
        _styleText.richText = true;
        _styleText.alignment = TextAnchor.UpperLeft;
        _styleText.normal.background = CosinaTools.CreateTextureWithColor(COLOR_BLACK);
        _styleText.normal.textColor = COLOR_LIGHT_GREY;
        _styleText.active.textColor = Color.red;
        _styleText.hover.textColor = Color.magenta;
        _styleText.wordWrap = false;
        _styleText.fixedHeight = _styleText.lineHeight * 1.2f;
        
        _styleText.clipping = TextClipping.Clip;
        //_richText2.hover.background = CosinaTools.CreateTextureWithColor(Color.blue);

        _styleInspect = new GUIStyle(GUI.skin.button);
        _styleInspect.richText = true;
        _styleInspect.alignment = TextAnchor.UpperLeft;
        
        _styleStatus = new GUIStyle(GUI.skin.label);
        _styleStatus.normal.background = CosinaTools.CreateTextureWithColor(COLOR_DARK_GREY);
        _styleStatus.normal.textColor = COLOR_LIGHT_GREY;

        _scroll = Vector2.zero;
        
        
    }

    private void OnGUI()
    {
        if (_styleText == null)
        {
            this.OnFirstRun();
        }

        GUIStyle style;
        style = _styleText;

        _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.MaxHeight(1000f));
        if (sFiltered == null)
        {
            sFiltered = new List<string>(0);
            sDisplaying = new List<string>(0);
        }
        else if (sDisplaying == null || sDisplaying.Count == 0)
        {
            sDisplaying = sFiltered.Select(x => x.Substring(0, x.IndexOf('\n'))).ToList();
        }
        
        GUI.backgroundColor = COLOR_LESS_DARK_GREY;
        int i = 0;
        while (true)
        {
            switch (i % 3)
            {
                case 0:
                    //GuiLine(COLOR_DARK_GREY, 1);
                    GUI.backgroundColor = COLOR_DARK_GREY;
                    break;
                case 1:
                    //GuiLine(COLOR_BLACK, 1);
                    GUI.backgroundColor = COLOR_BLACK;
                    break;
                case 2:
                    //GuiLine(COLOR_LESS_DARK_GREY, 1);
                    GUI.backgroundColor = COLOR_SLIGHT_DARK_GREY;
                    break;
            }
            
            if (sFiltered == null || sFiltered.Count <= i)
                break;
            
            EditorGUILayout.SelectableLabel(sFiltered[i], style);
            //if (GUILayout.Button(sDisplaying[i], style))
            //if (GUILayout.Button(sFiltered[i].Substring(0, sFiltered[i].IndexOf('\n')), style))
            //if (GUILayout.Button(sFiltered[i], style))
            // {
            //     if (sFiltered != null && sFiltered.Count > i)
            //         this.Analysis(sFiltered[i]);
            // }
            ++i;
        }
        GUILayout.EndScrollView();
        
        
        GuiLine(Color.green, 1);
        GuiLine(Color.blue, 1);
        _scrollInspect = GUILayout.BeginScrollView(_scrollInspect, GUILayout.Height(200f));
        GUILayout.TextField(sInspect, _styleInspect);
        GUILayout.EndScrollView();
        
        GuiLine(Color.green, 1);
        GuiLine(Color.blue, 1);
        GUI.backgroundColor = COLOR_LESS_DARK_GREY;
        GUILayout.Label(this.sStatus, _styleStatus);

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = COLOR_WHITE;
        if (GUILayout.Button("Filter"))
        {
            this.Filter();
        }
        this.sFilterer = GUILayout.TextField(this.sFilterer);
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = COLOR_WHITE;
        if (GUILayout.Button("Fetch All"))
        {
            this.FetchCustomLog();
        }
        if (GUILayout.Button("Clear CoLog Manager's logs"))
        {
            this.ClearCustomLog();
        }
        GUILayout.EndHorizontal();
        
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = COLOR_WHITE;
        this._countOperation = EditorGUILayout.IntField(_countOperation);
        if (GUILayout.Button("Fetch Lasts"))
        {
            this.FetchLastCount();
        }
        if (GUILayout.Button("Exclude Lasts"))
        {
            this.ExcludeLastCount();
        }
        GUILayout.EndHorizontal();
    }
    private void Filter()
    {
        this.UpdateStatus("Filter start...");
        sFiltered = string.IsNullOrEmpty(sFilterer) ?
            _s : _s.Where(this.FilterEachLine).ToList();
        sDisplaying = sFiltered.Select(x => x.Substring(0, x.IndexOf('\n'))).ToList();
            
        
        
        this.UpdateStatus($"Filter completed. Found {sFiltered?.Count.ToString()??"0"} results.");
    }

    private bool FilterEachLine(string s)
    {
        return s.StripHTML().Contains(this.sFilterer);
    }
    
    private void FetchCustomLog()
    {
        _s = CoLogManager.logs;
        this.Filter();
        this.UpdateStatus($"Fetch All CoLog's completed. Found {_s?.Count.ToString()??"0"} logs.");
    }

    private void FetchLastCount()
    {
        _s = CoLogManager.logs.TakeLast(_countOperation).ToList();
        this.Filter();
        this.UpdateStatus($"Fetch last {_countOperation} CoLog's completed. Found {_s?.Count.ToString()??"0"} logs.");
    }
    private void ExcludeLastCount()
    {
        _s.RemoveLast(_countOperation);
        this.Filter();
        this.UpdateStatus($"Exclude last {_countOperation} CoLog's completed. Found {_s?.Count.ToString()??"0"} logs.");
    }
    
    private void ClearCustomLog()
    {
        CoLogManager.Clear();
        this.UpdateStatus("Clear CoLog's completed.");
    }
    
    private void GuiLine(in Color col, in int height)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height );
        rect.height = height;
        EditorGUI.DrawRect(rect, col);
    }

    private void UpdateStatus(string newStatus)
    {
        this.sStatus = newStatus;
        _styleStatus.normal.textColor = CosinaTools.RandomColor(true);
    }
    
}
#endif