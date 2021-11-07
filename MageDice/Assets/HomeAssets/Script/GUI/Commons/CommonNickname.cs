using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Games/Nicknames")]
public class CommonNickname : ScriptableObject
{
    public NicknameData nickname;
    private static CommonNickname _isntance;
    public static CommonNickname Instance
    {
        get
        {
            if (_isntance == null)
            {

                _isntance = LoaderUtility.Instance.GetAsset<CommonNickname>("Home/Configs/Users/Nicknames");

            }
            return _isntance;
        }

    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        string path = "Assets/LocalDatas/Nicknames/names";
        TextAsset textAsset = GameUtils.LoadAssetInFolder<TextAsset>(path, ".json"); //UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        if (textAsset != null)
        {
            if (!textAsset.text.Equals(""))
            {
                this.nickname = JsonUtility.FromJson<NicknameData>(textAsset.text);

            }
        }
    }
#endif

    public string GetRandomNickname()
    {
        if (this.nickname != null)
        {
            if (this.nickname.names.Count > 0)
            {
                int index = Random.Range(0, this.nickname.names.Count);
                return this.nickname.names[index];
            }
            else
            {
                Debug.LogError("KHONG CO NICKNAME NAO");
            }
        }
        else
        {
            Debug.LogError("KHONG TIM THAY NICKNAME");
        }
        return string.Empty;
    }
}
[System.Serializable]
public class NicknameData
{
    public List<string> names;
}