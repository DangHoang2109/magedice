#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

using System.IO;

[CustomEditor(typeof(ShopStatConfigs))]
public class CueConfigsEditor : Editor
{
    private enum TabType
    {
        None = 0,
        Stat = 5,
        Utility = 10
    }

    private enum UtilityType
    {
        None = 0,
        NewStat = 5,
        Other = 10,
    }

    private class TabData
    {
        public Vector2 _scrollPos;
        public List<ShopStatConfig> configs;
        public List<bool> isItemExpandeds;

        public TabData()
        {
            _scrollPos = Vector2.zero;
            configs = new List<ShopStatConfig>();
            isItemExpandeds = new List<bool>();
        }
    }

    private TabType upperTabType;
    private StatManager.Tier tierTabType;
    private UtilityType utilityTabType;

    private Dictionary<StatManager.Tier, TabData> dicTabData;


    private static List<StatManager.Tier> _tiers = null;
    private static List<StatManager.Tier> _Tiers
    {
        get
        {
            if(_tiers == null)
            {
                StatManager.Tier[] ttt = (StatManager.Tier[])System.Enum.GetValues(typeof(StatManager.Tier));
                _tiers = new List<StatManager.Tier>(ttt);
            }
            return _tiers;
        }
    }

    SerializedProperty configs;

    private int size = 0;


    private string _strCloneSource = string.Empty;
    private string _strCloneTargets = string.Empty;



    private Color[] cols;

    private GUIStyle styleBtnActive;

    private GUIStyle styleIconCue;

    private GUIStyle styleTextBold;
    private GUIStyle styleTextBoldRed;

    private GUIStyle styleBtnRed;
    private GUIStyle styleBtnMangeta;

    private GUIStyle styleBtnSquare;
    private GUIStyle styleBtnSquareRed;

    private GUIStyle styleIndent;


    private Dictionary<int, Texture2D> dictTex;

    private GUISkin customSkin;

    Texture2D texBlue;
    Color navyColor;
    Texture2D texDarkBlue;
    Color darkRed;

    void OnEnable()
    {
        configs = serializedObject.FindProperty("configs");
        customSkin = (GUISkin)Resources.Load("CustomSkin");
        size = configs.arraySize;

        cols = new Color[]
        {
            Color.yellow,
            new Color(0.8f, 1f, 1f),
            new Color(0.95f, 0.8f, 1f),
            new Color(1f, 0.9f, 0.9f),
            new Color(0.8f, 1f, 0.8f),
        };

    }

    private object __check;

    private void FirstRun()
    {
        __check = new object();

        darkRed = new Color(0.8f, 0f, 0f);
        texBlue = this.MakeTex(Color.blue);
        navyColor = new Color(0f, 0f, 0.4f);
        texDarkBlue = this.MakeTex(navyColor);

        styleBtnActive = new GUIStyle(GUI.skin.button);

        styleBtnActive.normal = customSkin.button.normal;
        styleBtnActive.active = customSkin.button.active;

        styleIconCue = new GUIStyle(GUI.skin.label);
        styleIconCue.fixedHeight = 50f;
        styleIconCue.fixedWidth = 300f;
        styleIconCue.padding = new RectOffset(0, 0, -5, -5);

        styleTextBold = new GUIStyle(GUI.skin.label);
        styleTextBold.fontStyle = FontStyle.Bold;
        styleTextBold.normal.background = this.MakeTex(Color.white);

        styleTextBoldRed = new GUIStyle(GUI.skin.label);
        styleTextBoldRed.fontStyle = FontStyle.Bold;
        styleTextBoldRed.normal.background = this.MakeTex(Color.white);
        styleTextBoldRed.normal.textColor = Color.red;

        styleBtnRed = new GUIStyle(GUI.skin.button);
        styleBtnRed.normal.textColor = Color.red;
        styleBtnRed.fontStyle = FontStyle.Bold;
        styleBtnMangeta = new GUIStyle(GUI.skin.button);
        styleBtnMangeta.normal.textColor = Color.magenta;
        styleBtnMangeta.fontStyle = FontStyle.Bold;


        styleBtnSquare = new GUIStyle(GUI.skin.button);
        styleBtnSquare.fixedWidth = 24f;
        styleBtnSquare.fontStyle = FontStyle.Bold;

        styleBtnSquareRed = new GUIStyle(GUI.skin.button);
        styleBtnSquareRed.normal.textColor = Color.red;
        styleBtnSquareRed.fixedWidth = 20f;
        styleBtnSquareRed.fontStyle = FontStyle.Bold;

        styleIndent = new GUIStyle(GUI.skin.box);
        styleIndent.fixedWidth = 30f;
        styleIndent.stretchHeight = true;
        styleIndent.normal.background = texDarkBlue;
    }

    private Texture2D MakeTex(Color col)
    {
        Color[] pix = new Color[] { col };

        Texture2D result = new Texture2D(1, 1);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    public override void OnInspectorGUI()
    {
        if (__check == null)
        {
            this.FirstRun();
        }

        switch (this.upperTabType)
        {
            case TabType.None:
                this.upperTabType = TabType.Stat;
                goto case TabType.Stat;
            case TabType.Stat:
                GUILayout.BeginHorizontal();
                GUILayout.Button("Cues", this.styleBtnActive);
                if(GUILayout.Button("Utility"))
                {
                    this.upperTabType = TabType.Utility;
                }
                GUILayout.EndHorizontal();

                this.RenderTabCues();
                break;

            case TabType.Utility:
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Cues"))
                {
                    this.upperTabType = TabType.Stat;
                }
                GUILayout.Button("Utility", this.styleBtnActive);
                GUILayout.EndHorizontal();

                this.RenderTabUtility();
                break;
        }
    }


    private void RenderTabCues()
    {
        // ============== render tier tabs ============
        GUILayout.BeginHorizontal();
        int count = _Tiers.Count;
        for(int i4 = 0; i4 < count; ++i4)
        {
            if(this.tierTabType == _tiers[i4])
            {
                GUILayout.Button(_tiers[i4].ToString(), this.styleBtnActive);
            }
            else
            {
                if(GUILayout.Button(_tiers[i4].ToString()))
                {
                    this.tierTabType = _tiers[i4];
                }
            }
        }
        GUILayout.EndHorizontal();


        // ============== render content inside the tier ==============
        if (this.dicTabData != null && this.dicTabData.ContainsKey(this.tierTabType))
        {

            TabData activeData = this.dicTabData[this.tierTabType];
            count = activeData.configs.Count;
            GUILayout.Label(string.Format("count: {0}", count));
            this.GuiHorizontalLine(this.darkRed, 2);

            activeData._scrollPos = GUILayout.BeginScrollView(
                activeData._scrollPos, GUILayout.MaxHeight(900f));
            for (int i5 = 0; i5 < count; ++i5)
            {
                if (i5 >= activeData.configs.Count)
                    break; // during refreshing
                this.RenderEachCue(i5, activeData.isItemExpandeds, activeData.configs[i5]);
            }
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No data loaded, button \"Load\" at the bottom");
            this.GuiHorizontalLine(this.darkRed, 2);
            GUILayout.BeginScrollView(Vector2.zero, GUILayout.MaxHeight(900f));
            GUILayout.EndScrollView();
        }



        // ========== render Load Save buttons ===========
        this.GuiHorizontalLine(this.darkRed, 2);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load", styleBtnMangeta))
        {
            serializedObject.Update();
            this.CloneOriginConfigs();
        }
        if (GUILayout.Button("Save", styleBtnRed))
        {
            serializedObject.Update();
            this.SaveData();
        }
        GUILayout.EndHorizontal();
    }

    private void RenderEachCue(int index, List<bool> isItemExpandeds, ShopStatConfig config)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("{0} - {1}   --- {2}", config.id, config.statName, config.unlockType),
            this.styleTextBoldRed);
        GUILayout.FlexibleSpace();
        
        if(GUILayout.Button("X", this.styleBtnSquareRed))
        {
            this.RemoveElement(index);
            if (index >= isItemExpandeds.Count)
                return;// during refreshing
        }
        if(GUILayout.Button("+", this.styleBtnSquare))
        {
            this.DuplicateElement(index);
        }
        GUILayout.EndHorizontal();
        if (config.sprStatItem != null)
        {
            GUILayout.Button(config.sprStatItem.texture, this.styleIconCue);
        }
        else
        {
            GUILayout.Button("-- cue --", styleIconCue);
        }

        StatItemStats[] stats = config.statsPerLevels;
        string txtBtnExpand;
        if(stats == null || stats.Length == 0)
        {
            txtBtnExpand = "--- no stat ---";
        }
        else
        {
            txtBtnExpand = string.Format("lvls: {0}        base: {1} {2} {3} {4}",
                stats.Length, stats[0].damageStrength, stats[0].rangeStrength, stats[0].speedStrength, stats[0].timeEffectStrength);

        }

        if(isItemExpandeds[index])
        {
            if(GUILayout.Button(txtBtnExpand, this.styleBtnActive))
            {
                isItemExpandeds[index] = !isItemExpandeds[index];
            }
            this.RenderChildStats(config);
        }
        else
        {
            if (GUILayout.Button(txtBtnExpand))
            {
                isItemExpandeds[index] = !isItemExpandeds[index];
            }
        }

        this.GuiHorizontalLine(Color.clear, 20);
        this.GuiHorizontalLine(Color.white, 5);
    }

    private void RenderChildStats(ShopStatConfig config)
    {
        EditorGUI.indentLevel++;
        config.id = (DiceID)EditorGUILayout.EnumPopup("id", config.id);
        config.statName = EditorGUILayout.TextField("statName", config.statName);
        config.skillDescription = EditorGUILayout.TextField("skillDescription", config.skillDescription);

        config.sprStatItem = (Sprite)EditorGUILayout.ObjectField(
            obj: config.sprStatItem,
            objType: typeof(Sprite), allowSceneObjects: false);
        config.tier = (StatManager.Tier)EditorGUILayout.EnumPopup("tier", config.tier);
        config.unlockType = (StatManager.UnlockType)EditorGUILayout.EnumPopup("unlockType", config.unlockType);
        config.upgradeType = (StatManager.UnlockType)EditorGUILayout.EnumPopup("upgradeType", config.upgradeType);

        EditorGUILayout.BeginHorizontal();
        config.isHide = EditorGUILayout.Toggle("isHide", config.isHide);
        config.appearTier = EditorGUILayout.IntField("appearTier", config.appearTier);
        EditorGUILayout.EndHorizontal();


        config.unlockLinkedId = EditorGUILayout.IntField("unlockLinkedId", config.unlockLinkedId);
        config.unlockText = EditorGUILayout.TextField("unlockText", config.unlockText);

        config.rateRandomUnlock = EditorGUILayout.FloatField("rate Unlock", config.rateRandomUnlock);

        GUILayout.BeginHorizontal();
        GUILayout.Box(text: null, this.styleIndent);
        GUILayout.BeginVertical();
        //EditorGUI.indentLevel++;
        if (config.statsPerLevels == null || config.statsPerLevels.Length == 0)
        {
            int levelYouWant = 5;
            levelYouWant = EditorGUILayout.IntField("Levels you want:", levelYouWant);
            if (GUILayout.Button("Create stat"))
            {
                if (levelYouWant < 0)
                {
                    Debug.LogError("levels can be negative!");
                    return;
                }
                config.statsPerLevels = new StatItemStats[levelYouWant];
                for(int ite = 0; ite < levelYouWant; ++ite)
                {
                    config.statsPerLevels[ite] = StatItemStats.CreateZero();
                }
            }
        }
        else
        {
            this.RenderChildStat(config);
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        //EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }

    private void RenderChildStat(ShopStatConfig config)
    {
        StatItemStats[] statPerLevel = config.statsPerLevels;

        for (int iss = 0; iss < statPerLevel.Length; ++iss)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("---- level: {0} ----", iss + 1), this.styleTextBold);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", this.styleBtnSquareRed))
            {// remove this level
                var l = config.statsPerLevels.ToList();
                l.RemoveAt(iss);
                statPerLevel = config.statsPerLevels = l.ToArray();

                if (iss >= statPerLevel.Length)
                    break;
            }
            if (GUILayout.Button("+", this.styleBtnSquare))
            {// duplicate this level
                var l = config.statsPerLevels.ToList();

                StatItemStats newStat = StatItemStats.Clone(statPerLevel[iss]);
                newStat.price = statPerLevel[iss].price;
                newStat.cardsRequired = statPerLevel[iss].cardsRequired;
                l.Insert(iss + 1, newStat);

                statPerLevel = config.statsPerLevels = l.ToArray();
            }
            GUILayout.EndHorizontal();

            statPerLevel[iss].price = EditorGUILayout.LongField("price", statPerLevel[iss].price);
            statPerLevel[iss].cardsRequired = EditorGUILayout.IntField("cardsRequired", statPerLevel[iss].cardsRequired);

            statPerLevel[iss].damageStrength = EditorGUILayout.FloatField("damageStrength", statPerLevel[iss].damageStrength);
            statPerLevel[iss].rangeStrength = EditorGUILayout.FloatField("rangeStrength", statPerLevel[iss].rangeStrength);
            statPerLevel[iss].speedStrength = EditorGUILayout.FloatField("speedStrength", statPerLevel[iss].speedStrength);
            statPerLevel[iss].timeEffectStrength = EditorGUILayout.FloatField("timeEffectStrength", statPerLevel[iss].timeEffectStrength);
        }
    }
    void GuiHorizontalLine(Color col, int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, col);
    }

    private void RemoveElement(int index)
    {
        if (this.dicTabData != null && this.dicTabData.ContainsKey(this.tierTabType))
        {
            TabData activeData = this.dicTabData[this.tierTabType];
            if (index >= activeData.configs.Count)
            {
                Debug.LogError("Remove Element: index too high!");
                return;// during refreshing
            }
                

            activeData.configs.RemoveAt(index);
            activeData.isItemExpandeds.RemoveAt(index);
        }
    }
    
    private void DuplicateElement(int index)
    {
        if(this.dicTabData != null && this.dicTabData.ContainsKey(this.tierTabType))
        {
            TabData activeData = this.dicTabData[this.tierTabType];
            if (index >= activeData.configs.Count)
            {
                Debug.LogError("Duplicate Element: index too high!");
                return;// during refreshing
            }

            ShopStatConfig current = activeData.configs[index];

            ShopStatConfig newConfig = ShopStatConfig.CloneConfig(current);
            activeData.configs.Insert(index + 1, newConfig);
            activeData.isItemExpandeds.Insert(index + 1, false);
        }
    }

    // ====== load data ======
    // -----------------------
    private void CloneOriginConfigs()
    {
        if (this.dicTabData != null && this.dicTabData.Count != 0)
        {
            bool check = EditorUtility.DisplayDialog("Load data?",
                   "Overwrite your working data", "OK", "Cancel cancel cancel");
            if (!check)
                return;
        }

        this.configs = serializedObject.FindProperty("configs");


        this.dicTabData = new Dictionary<StatManager.Tier, TabData>(_Tiers.Count);
        for (int i1 = 0; i1 < _Tiers.Count; ++i1)
        {
            this.dicTabData.Add(_Tiers[i1], new TabData());
        }

        int n = configs.arraySize;
        for (int i2 = 0; i2 < n; ++i2)
        {
            this.CloneOriginConfig(i2, configs.GetArrayElementAtIndex(i2));
        }
    }

    private void CloneOriginConfig(int index, SerializedProperty child)
    {
        if (index >= configs.arraySize)
            return;// refreshing

        ShopStatConfig newConfig = new ShopStatConfig()
        {
            id = (DiceID)child.FindPropertyRelative("id").intValue,
            statName = child.FindPropertyRelative("statName").stringValue,
            skillDescription = child.FindPropertyRelative("skillDescription").stringValue,
            sprStatItem = child.FindPropertyRelative("sprStatItem").objectReferenceValue as Sprite,
            tier = (StatManager.Tier)child.FindPropertyRelative("tier").intValue,
            unlockType = (StatManager.UnlockType)child.FindPropertyRelative("unlockType").intValue,
            upgradeType = (StatManager.UnlockType)child.FindPropertyRelative("upgradeType").intValue,
            unlockLinkedId = child.FindPropertyRelative("unlockLinkedId").intValue,
            unlockText = child.FindPropertyRelative("unlockText").stringValue,
            rateRandomUnlock = child.FindPropertyRelative("rateRandomUnlock").floatValue,
            isHide = child.FindPropertyRelative("isHide").boolValue,
            appearTier = child.FindPropertyRelative("appearTier").intValue
        };

        SerializedProperty stats = child.FindPropertyRelative("statsPerLevels");
        if (stats != null)
        {
            this.CloneOriginConfigStats(newConfig, stats);
        }

        if(!this.dicTabData.ContainsKey(newConfig.tier))
        {
            Debug.LogError("dict not contain key: " + newConfig.tier);
        }
        else
        {
            this.dicTabData[newConfig.tier].configs.Add(newConfig);
            this.dicTabData[newConfig.tier].isItemExpandeds.Add(false);
        }
    }

    private void CloneOriginConfigStats(ShopStatConfig newConfig, SerializedProperty childStats)
    {
        newConfig.statsPerLevels = new StatItemStats[childStats.arraySize];
        if (newConfig.statsPerLevels.Length == 0)
            return;

        for (int i3 = 0; i3 < newConfig.statsPerLevels.Length; ++i3)
        {
            this.CloneOriginConfigStat(i3, newConfig.statsPerLevels,
                childStats.GetArrayElementAtIndex(i3));
        }
    }

    private void CloneOriginConfigStat(int index, StatItemStats[] stats, SerializedProperty childStatChild)
    {
        stats[index] = StatItemStats.CreateZero();
        stats[index].price = childStatChild.FindPropertyRelative("price").longValue;
        stats[index].cardsRequired = childStatChild.FindPropertyRelative("cardsRequired").intValue;

        stats[index].damageStrength = childStatChild.FindPropertyRelative("damageStrength").floatValue;
        stats[index].rangeStrength = childStatChild.FindPropertyRelative("rangeStrength").floatValue;
        stats[index].speedStrength = childStatChild.FindPropertyRelative("speedStrength").floatValue;
        stats[index].timeEffectStrength = childStatChild.FindPropertyRelative("timeEffectStrength").floatValue;
    }
    // ---- load data ----




    // ====== save data ======
    // -----------------------
    /// <summary>
    /// Serialize data back to disk
    /// </summary>
    private void SaveData()
    {
        bool check = EditorUtility.DisplayDialog("Save data?",
            "Overwrite disk data by your working data", "OK", "Cancel cancel cancel");
        if (!check)
            return;

        if(this.dicTabData == null || this.dicTabData.Count == 0)
        {
            Debug.LogError("No data to save!");
            return;
        }

        //this.CorrectUpgradeType();


        List<ShopStatConfig> l;

        int totalCount = 0;
        for(int i = 0; i < _Tiers.Count; ++i)
        {
            if (!this.dicTabData.ContainsKey(_tiers[i]))
                continue;

            l = this.dicTabData[_tiers[i]].configs;
            if (l == null || l.Count == 0)
                continue;

            totalCount += l.Count;
        }

        this.configs = serializedObject.FindProperty("configs");
        this.configs.arraySize = totalCount;

        int index = 0;
        for (int i = 0; i < _Tiers.Count; ++i)
        {
            if (!this.dicTabData.ContainsKey(_tiers[i]))
                continue;

            l = this.dicTabData[_tiers[i]].configs;
            if (l == null || l.Count == 0)
                continue;

            for(int j = 0; j < l.Count; ++j)
            {
                this.SaveDataChild(l[j], this.configs.GetArrayElementAtIndex(index));
                
                ++index;
            }
        }

        serializedObject.ApplyModifiedProperties();// apply array size?
    }

    private void CorrectUpgradeType()
    {
        Debug.LogError("Warning! You are using Correct Upgrade type!");

        foreach (TabData tabData in this.dicTabData.Values)
        {
            if (tabData.configs == null || tabData.configs.Count == 0)
                continue;

            foreach (ShopStatConfig cf in tabData.configs)
            {
                if (cf.unlockType == StatManager.UnlockType.Coin || cf.unlockType == StatManager.UnlockType.Cash)
                {
                    cf.upgradeType = StatManager.UnlockType.Coin;
                }
            }
        }
    }

    private void SaveDataChild(ShopStatConfig cf, SerializedProperty child)
    {
        child.FindPropertyRelative("id").intValue = (int)cf.id;
        child.FindPropertyRelative("statName").stringValue = cf.statName;
        child.FindPropertyRelative("skillDescription").stringValue = cf.skillDescription;
        child.FindPropertyRelative("sprStatItem").objectReferenceValue = cf.sprStatItem;
        child.FindPropertyRelative("tier").intValue = (int)cf.tier;
        child.FindPropertyRelative("unlockType").intValue = (int)cf.unlockType;
        child.FindPropertyRelative("upgradeType").intValue = (int)cf.upgradeType;
        child.FindPropertyRelative("unlockLinkedId").intValue = cf.unlockLinkedId;
        child.FindPropertyRelative("unlockText").stringValue = cf.unlockText;


        child.FindPropertyRelative("isHide").boolValue = cf.isHide;
        child.FindPropertyRelative("appearTier").intValue = cf.appearTier;

        SerializedProperty stats = child.FindPropertyRelative("statsPerLevels");
        if(cf.statsPerLevels == null || cf.statsPerLevels.Length == 0)
        {
            stats.arraySize = 0;
        }
        else
        {
            stats.arraySize = cf.statsPerLevels.Length;

            SerializedProperty s; StatItemStats cS;
            for (int ic = 0; ic < cf.statsPerLevels.Length; ++ic)
            {
                s = stats.GetArrayElementAtIndex(ic);
                cS = cf.statsPerLevels[ic];

                s.FindPropertyRelative("price").longValue = cS.price;
                s.FindPropertyRelative("cardsRequired").intValue = cS.cardsRequired;

                s.FindPropertyRelative("damageStrength").floatValue = cS.damageStrength;
                s.FindPropertyRelative("rangeStrength").floatValue = cS.rangeStrength;
                s.FindPropertyRelative("speedStrength").floatValue = cS.speedStrength;
                s.FindPropertyRelative("timeEffectStrength").floatValue = cS.timeEffectStrength;
            }
        }
    }

    // --- save data ---






    // ============================= UTILITY ===============================
    // ---------------------------------------------------------------------

    private void RenderTabUtility()
    {
        switch (this.utilityTabType)
        {
            case UtilityType.None:
                this.utilityTabType = UtilityType.Other;
                goto case UtilityType.Other;
            case UtilityType.NewStat:

                GUILayout.BeginHorizontal();
                GUILayout.Button("List new cues", this.styleBtnActive);
                if (GUILayout.Button("Others"))
                {
                    this.utilityTabType = UtilityType.Other;
                }
                GUILayout.EndHorizontal();

                this.RenderSubTabManageNewCue();

                break;
            case UtilityType.Other:

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("List new cues"))
                {
                    this.utilityTabType = UtilityType.NewStat;
                }
                GUILayout.Button("Others", this.styleBtnActive);
                GUILayout.EndHorizontal();

                this.RenderSubTabOtherUtility();

                break;
        }


    }

    // ====== new cues =======
    // -----------------------

    private NewCuePack newCuePack;
    private Vector2 scrollNewCues = Vector2.zero;
    private List<ShopStatConfig> cachedConfigNewCues;


    private void RenderSubTabManageNewCue()
    {
        //this.GuiHorizontalLine(this.navyColor, 10);



        // ============== render content inside the tier ==============
        if (this.newCuePack != null && this.newCuePack.newIds != null && this.newCuePack.newIds.Count != 0)
        {

            GUILayout.Label(string.Format("count: {0}", newCuePack.newIds.Count));
            this.GuiHorizontalLine(this.navyColor, 2);

            this.scrollNewCues = GUILayout.BeginScrollView(
                this.scrollNewCues, GUILayout.MaxHeight(900f));

            for (int i6 = 0; i6 < newCuePack.newIds.Count; ++i6)
            {
                //this.RenderEachNewCue(i6, newCuePack.newIds);
            }
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No data loaded");
            this.GuiHorizontalLine(this.navyColor, 2);
            GUILayout.BeginScrollView(Vector2.zero, GUILayout.MaxHeight(900f));
            GUILayout.EndScrollView();
        }



        // ========== render Load Save buttons ===========
        this.GuiHorizontalLine(this.darkRed, 2);
        if (GUILayout.Button("Synch new cues's configs"))
        {
            this.SynchNewCueConfigs();
        }
        this.GuiHorizontalLine(this.darkRed, 2);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load list new cues", styleBtnMangeta))
        {
            this.LoadNewCues();
        }
        if (GUILayout.Button("Save list new cues", styleBtnRed))
        {
            this.SaveNewCues();
        }
        GUILayout.EndHorizontal();
    }

    private void RenderEachNewCue(int index, List<string> newIds)
    {

        if (this.cachedConfigNewCues[index] == null)
        {
            GUILayout.BeginHorizontal();
            newIds[index] = EditorGUILayout.TextField(newIds[index], GUILayout.Width(100f));
            GUILayout.Label("id not found", this.styleTextBoldRed);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", this.styleBtnSquareRed))
            {
                newIds.RemoveAt(index);
                this.cachedConfigNewCues.RemoveAt(index);

                if (index >= newIds.Count)
                    return;// during refreshing
            }
            if (GUILayout.Button("+", this.styleBtnSquare))
            {
                newIds.Insert(index + 1, newIds[index]);
                this.cachedConfigNewCues.Insert(index + 1, this.cachedConfigNewCues[index]);
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            ShopStatConfig cf = this.cachedConfigNewCues[index];

            GUILayout.BeginHorizontal();
            newIds[index] = EditorGUILayout.TextField(newIds[index], GUILayout.Width(100f));

            GUILayout.Label(string.Format("{0}", cf.statName),
                this.styleTextBoldRed);

            GUILayout.Label(string.Format("{0}", cf.skillDescription),
                this.styleTextBoldRed);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", this.styleBtnSquareRed))
            {
                newIds.RemoveAt(index);
                this.cachedConfigNewCues.RemoveAt(index);

                if (index >= newIds.Count)
                    return;// during refreshing
            }
            if (GUILayout.Button("+", this.styleBtnSquare))
            {
                newIds.Insert(index + 1, newIds[index]);
                this.cachedConfigNewCues.Insert(index + 1, this.cachedConfigNewCues[index]);
            }
            GUILayout.EndHorizontal();
            if (cf.sprStatItem != null)
            {
                GUILayout.Button(cf.sprStatItem.texture, this.styleIconCue);
            }
            else
            {
                GUILayout.Button("-- cue --", styleIconCue);
            }
        }

        this.GuiHorizontalLine(Color.clear, 10);
        this.GuiHorizontalLine(Color.white, 5);
    }

    private void LoadNewCues()
    {
        if(this.dicTabData == null)
        {
            bool check =  EditorUtility.DisplayDialog("Load list new cues failed",
                "You must load config data first!", "Load config data", "Cancel");
            if(check)
            {
                this.CloneOriginConfigs();
            }
            else
            {
                return;
            }
        }
        if (this.dicTabData.Count == 0)
        {
            EditorUtility.DisplayDialog("Load list new cues failed",
                "Config data empty!", "OK");
            return;
        }

        string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/Resources/Games/Configs/NewCues.json").Replace('\\', '/');
        if (!File.Exists(path))
        {
            Debug.LogError("No file in the path to read:\n" + path);
            return;
        }

        string t = File.ReadAllText(path);
        this.newCuePack = JsonUtility.FromJson<NewCuePack>(t);

        this.SynchNewCueConfigs();
    }

    private void SynchNewCueConfigs()
    {
        if (this.newCuePack != null && this.newCuePack.newIds != null)
        {
            this.cachedConfigNewCues = new List<ShopStatConfig>(this.newCuePack.newIds.Count);
            for (int i7 = 0; i7 < this.cachedConfigNewCues.Capacity; ++i7)
            {
                this.cachedConfigNewCues.Add(this.FindMatchedConfig(this.newCuePack.newIds[i7]));
            }
        }
    }

    private ShopStatConfig FindMatchedConfig(DiceID lookingId)
    {
        ShopStatConfig c = null;
        foreach (TabData tabData in this.dicTabData.Values)
        {
            if (tabData.configs == null || tabData.configs.Count == 0)
                continue;

            c = tabData.configs.Find(x => x.id == lookingId);
            if (c != null)
                return c;
        }
        return null;
    }
    private void SaveNewCues()
    {
        if (this.newCuePack == null)
            return;

        string text = JsonUtility.ToJson(this.newCuePack, false);

        string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Assets/Resources/Games/Configs/NewCues.json").Replace('\\', '/');
        string folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(path, text);
    }


    private void RenderSubTabOtherUtility()
    {

        this.GuiHorizontalLine(Color.clear, 10);
        this.GuiHorizontalLine(Color.red, 2);
        GUILayout.Label("Clone stats from a cue to others", this.styleTextBold);
        GUILayout.BeginHorizontal();
        this._strCloneSource = EditorGUILayout.TextField("Source ID: ", this._strCloneSource);
        if (GUILayout.Button("Clone stats"))
        {
            this.CloneStat();
            serializedObject.ApplyModifiedProperties();
        }
        GUILayout.EndHorizontal();
        this._strCloneTargets = EditorGUILayout.TextField("Target IDs by ; ", this._strCloneTargets);



        //GUILayout.BeginHorizontal();
        //this._tempSize = EditorGUILayout.IntField("count of configs:", _tempSize);
        //if (GUILayout.Button("Update"))
        //{
        //    this.Update();
        //}
        //GUILayout.EndHorizontal();


        this.GuiHorizontalLine(Color.clear, 10);
        this.GuiHorizontalLine(Color.red, 2);
        GUILayout.Label("Export/Import entire the assets (backup only)", this.styleTextBold);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("To Json"))
        {
            this.ToJson();
        }
        if (GUILayout.Button("From Json"))
        {
            this.FromJson();
        }
        GUILayout.EndHorizontal();

        this.GuiHorizontalLine(Color.clear, 10);
        this.GuiHorizontalLine(Color.red, 2);

        GUILayout.Label("Import all cues's stats (this feature not auto save)", this.styleTextBold);
        if(GUILayout.Button("Import cues's stats"))
        {
            this.ImportStatItemStats();
        }

        this.GuiHorizontalLine(Color.clear, 10);
        this.GuiHorizontalLine(Color.green, 2);
    }


    //private void Update()
    //{
    //    this.size = _tempSize;

    //    this.dictTex.Clear();
    //}

    private void ToJson()
    {
        string text = JsonUtility.ToJson(this.target, true);

        string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "extracted/cues.txt").Replace('\\', '/');
        string folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }


        File.WriteAllText(path, text);

        Application.OpenURL(folder);
    }

    private void FromJson()
    {
        string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "extracted/cues.txt").Replace('\\', '/');
        if (!File.Exists(path))
        {
            EditorUtility.DisplayDialog("Error!", "No file in the path to read:\n" + path, "OK");
            return;
        }

        string t = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(t, this.target);
    }

    [System.Serializable]
    public class CuesStatsImportingPack
    {
        public List<StatItemStatsImportingItem> array;
    }
    [System.Serializable]
    public class StatItemStatsImportingItem
    {
        public string ID;
        public string Level;
        public string Card_Require;
        public string Price;
        public string apw;
        public string aspeedStrength;
        public string aaim;
        public string atime;
        public string pw;
        public string speedStrength;
        public string aim;
        public string time;
    }


    private void ImportStatItemStats()
    {
        if (this.dicTabData == null)
        {
            bool check = EditorUtility.DisplayDialog("Import cues's stats failed",
                "You must load config data first!", "Load config data", "Cancel");
            if (check)
            {
                this.CloneOriginConfigs();
            }
            else
            {
                return;
            }
        }
        if (this.dicTabData.Count == 0)
        {
            EditorUtility.DisplayDialog("Import cues's stats failed",
                "Config data empty!", "OK");
            return;
        }

        string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "import/cues_stats.json").Replace('\\', '/');

        if (!File.Exists(path))
        {
            EditorUtility.DisplayDialog("Error!", "No file in the path to read:\n" + path, "OK");

            return;
        }

        string tt = File.ReadAllText(path);
        CuesStatsImportingPack pack = JsonUtility.FromJson<CuesStatsImportingPack>(tt);
        pack.array.RemoveAll(this.FilterEmptyImportingItemId);

        this.MapData(pack);

        EditorUtility.DisplayDialog("Import complete!",
                "Import complete!\nCheck the imported data before saving them!", "OK");
    }

    private bool FilterEmptyImportingItemId(StatItemStatsImportingItem item)
    {
        return string.IsNullOrEmpty(item.ID);
    }

    private void MapData(CuesStatsImportingPack pack)
    {
        // id, level
        Dictionary<string, Dictionary<int, StatItemStatsImportingItem>> mapper
            = new Dictionary<string, Dictionary<int, StatItemStatsImportingItem>>();

        var ar = pack.array;
        int n = ar.Count;
        int parsedLevel;
        Dictionary<int, StatItemStatsImportingItem> subDict;
        for (int i = 0; i < n; ++i)
        {
            if (!mapper.ContainsKey(ar[i].ID))
                mapper.Add(ar[i].ID, new Dictionary<int, StatItemStatsImportingItem>());

            if (!int.TryParse(ar[i].Level, out parsedLevel))
                continue;

            subDict = mapper[ar[i].ID];
            if (!subDict.ContainsKey(parsedLevel))
                subDict.Add(parsedLevel, ar[i]);
            else
            {
                Debug.LogError(string.Format("ImportStatItemStats MapData dupplicate" +
                    " item ID: {0} level: {1}", ar[i].ID, parsedLevel));
            }
        }

        StatItemStatsImportingItem tempItem;
        foreach (TabData tabData in this.dicTabData.Values)
        {
            if (tabData.configs == null || tabData.configs.Count == 0)
                continue;

            foreach(ShopStatConfig cf in tabData.configs)
            {
                if (!mapper.ContainsKey(cf.id.ToString()))
                    continue;
                subDict = mapper[cf.id.ToString()];
                cf.statsPerLevels = new StatItemStats[subDict.Keys.Count];
                for(int iz = 0; iz < cf.statsPerLevels.Length; ++iz)
                {
                    cf.statsPerLevels[iz] = StatItemStats.CreateZero();
                    if (!subDict.ContainsKey(iz + 1))
                        continue;

                    tempItem = subDict[iz + 1];

                    int.TryParse(tempItem.Card_Require, out cf.statsPerLevels[iz].cardsRequired);
                    tempItem.Price = tempItem.Price.Replace(",", "").Replace(".", "");
                    long.TryParse(tempItem.Price, out cf.statsPerLevels[iz].price);

                    float.TryParse(tempItem.apw, out cf.statsPerLevels[iz].damageStrength);
                    float.TryParse(tempItem.aaim, out cf.statsPerLevels[iz].rangeStrength);
                    float.TryParse(tempItem.aspeedStrength, out cf.statsPerLevels[iz].speedStrength);
                    float.TryParse(tempItem.atime, out cf.statsPerLevels[iz].timeEffectStrength);
                    
                }
            }
        }


    }



    //private void DisplayChilren(int index, SerializedProperty child)
    //{
    //    if (index >= this.configs.arraySize)
    //        return;


    //    GUI.backgroundColor = Color.white;


    //    GUILayout.BeginHorizontal();

    //    Object ojSpr = child.FindPropertyRelative("sprCard").objectReferenceValue;

    //    if (ojSpr != null)
    //    {
    //        Sprite spr = ojSpr as Sprite;
    //        GUILayout.Button(spr.texture, iconCardStyle);
    //    }
    //    else
    //    {
    //        GUILayout.Button("ca", iconCardStyle);
    //    }

    //    ojSpr = child.FindPropertyRelative("sprStatItem").objectReferenceValue;

    //    if (ojSpr != null)
    //    {
    //        Sprite spr = ojSpr as Sprite;
    //        GUILayout.Button(/*this.GetCacheCropedImage(index,*/ spr.texture/*)*/, styleIconCue);
    //    }
    //    else
    //    {
    //        GUILayout.Button("-- cue --", styleIconCue);
    //    }
    //    GUILayout.EndHorizontal();

    //    GUI.backgroundColor = this.cols[index % this.cols.Length];
    //    EditorGUILayout.PropertyField(child, includeChildren: true);

    //    if (child.isExpanded)
    //    {

    //        EditorGUI.indentLevel++;
    //        GUI.backgroundColor = Color.white;
    //        if (GUILayout.Button("____ Multiplier ____ | Auto Fill()"))
    //        {
    //            this.AutoFillData(index, child);
    //        }
    //        EditorGUI.indentLevel++;

    //        additionsEachLevel[index].damageStrength = EditorGUILayout.FloatField(
    //            "strength", additionsEachLevel[index].damageStrength);
    //        additionsEachLevel[index].rangeStrength = EditorGUILayout.FloatField(
    //            "aim", additionsEachLevel[index].rangeStrength);
    //        additionsEachLevel[index].speedStrength = EditorGUILayout.FloatField(
    //            "speedStrength", additionsEachLevel[index].speedStrength);
    //        additionsEachLevel[index].timeEffectStrength = EditorGUILayout.FloatField(
    //            "timeEffectStrength", additionsEachLevel[index].timeEffectStrength);
    //        additionsEachLevel[index].price = EditorGUILayout.LongField(
    //            "price", additionsEachLevel[index].price);

    //        EditorGUI.indentLevel--;
    //        EditorGUI.indentLevel--;
    //    }
    //    GUI.backgroundColor = Color.red;
    //    EditorGUILayout.TextArea(string.Empty, GUILayout.Height(2));
    //}


    //private void AutoFillData(int index, SerializedProperty child)
    //{
    //    SerializedProperty c = child.FindPropertyRelative("statsPerLevels");
    //    int n = c.arraySize;
    //    if (n == 0 || n == 1)
    //        return;

    //    SerializedProperty c0 = c.GetArrayElementAtIndex(0),
    //        ci;

    //    float mPower = c0.FindPropertyRelative("damageStrength").floatValue;
    //    float mAim = c0.FindPropertyRelative("rangeStrength").floatValue;
    //    float mspeedStrength = c0.FindPropertyRelative("speedStrength").floatValue;
    //    float mTime = c0.FindPropertyRelative("timeEffectStrength").floatValue;
    //    long mPrice = c0.FindPropertyRelative("price").longValue;
    //    int mCardsRequired = c0.FindPropertyRelative("cardsRequired").intValue;
    //    for (int i = 1; i < n; ++i)
    //    {
    //        ci = c.GetArrayElementAtIndex(i);
    //        ci.FindPropertyRelative("damageStrength").floatValue = (float)System.Math.Round(Mathf.Clamp(additionsEachLevel[index].damageStrength * i + mPower
    //            , 0f, 10f), System.MidpointRounding.AwayFromZero);
    //        ci.FindPropertyRelative("rangeStrength").floatValue = (float)System.Math.Round(Mathf.Clamp(additionsEachLevel[index].rangeStrength * i + mAim
    //            , 0f, 10f), System.MidpointRounding.AwayFromZero);
    //        ci.FindPropertyRelative("speedStrength").floatValue = (float)System.Math.Round(Mathf.Clamp(additionsEachLevel[index].speedStrength * i + mspeedStrength
    //            , 0f, 10f), System.MidpointRounding.AwayFromZero);
    //        ci.FindPropertyRelative("timeEffectStrength").floatValue = (float)System.Math.Round(Mathf.Clamp(additionsEachLevel[index].timeEffectStrength * i + mTime
    //            , 0f, 10f), System.MidpointRounding.AwayFromZero);

    //        ci.FindPropertyRelative("price").longValue = additionsEachLevel[index].price * i + mPrice;
    //        ci.FindPropertyRelative("cardsRequired").intValue = additionsEachLevel[index].cardsRequired * i + mCardsRequired;


    //    }
    //}


    private void CloneStat()
    {
        if (string.IsNullOrEmpty(this._strCloneSource) || string.IsNullOrEmpty(this._strCloneTargets))
        {
            Debug.LogError("Clone stats failed: empty input or output!");
            return;
        }

        string[] targetIds = this._strCloneTargets.Split(';');

        if (targetIds == null || targetIds.Length == 0)
            return;

        SerializedProperty source = this.FindElementWithId(this._strCloneSource);
        if (source == null)
        {
            Debug.LogError("Clone Stats failed: source not found: " + this._strCloneSource);
            return;
        }

        foreach (SerializedProperty target in this.FindElementWithIds(targetIds))
        {
            this.CloneConfigStatsAllLevel(source, target);
        }
    }

    private SerializedProperty FindElementWithId(string id)
    {
        SerializedProperty tmp;
        for (int i = 0; i < size; ++i)
        {
            tmp = configs.GetArrayElementAtIndex(i);
            if (tmp.FindPropertyRelative("id").stringValue == id)
                return tmp;
        }

        return null;
    }

    private IEnumerable<SerializedProperty> FindElementWithIds(params string[] ids)
    {
        List<string> convertedIds = ids.ToList();
        int found = 0;

        SerializedProperty tmp;
        for (int j = 0; j < size; ++j)
        {
            tmp = configs.GetArrayElementAtIndex(j);
            if (convertedIds.Contains(tmp.FindPropertyRelative("id").stringValue))
            {
                yield return tmp;

                if ((++found) >= ids.Length)
                    yield break;
            }
        }
    }

    private void CloneConfigStatsAllLevel(SerializedProperty source, SerializedProperty target)
    {

        SerializedProperty sourceC = source.FindPropertyRelative("statsPerLevels");
        SerializedProperty targetC = target.FindPropertyRelative("statsPerLevels");

        int arraySize
            = target.FindPropertyRelative("statsPerLevels").arraySize
                = source.FindPropertyRelative("statsPerLevels").arraySize;

        for (int i = 0; i < arraySize; ++i)
        {
            this.CloneConfigStatsEachLevel(sourceC.GetArrayElementAtIndex(i)
                , targetC.GetArrayElementAtIndex(i));
        }

    }

    private void CloneConfigStatsEachLevel(SerializedProperty source, SerializedProperty target)
    {
        target.FindPropertyRelative("damageStrength").floatValue
            = source.FindPropertyRelative("damageStrength").floatValue;
        target.FindPropertyRelative("rangeStrength").floatValue
            = source.FindPropertyRelative("rangeStrength").floatValue;
        target.FindPropertyRelative("speedStrength").floatValue
            = source.FindPropertyRelative("speedStrength").floatValue;
        target.FindPropertyRelative("timeEffectStrength").floatValue
            = source.FindPropertyRelative("timeEffectStrength").floatValue;

        target.FindPropertyRelative("price").longValue
            = source.FindPropertyRelative("price").longValue;
        target.FindPropertyRelative("cardsRequired").intValue
            = source.FindPropertyRelative("cardsRequired").intValue;
    }





    // ======= load "new cues" list =======
    // ------------------------------------


}

#endif