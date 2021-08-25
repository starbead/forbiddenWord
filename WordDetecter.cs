using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class TypeBanWord
{
    public string type;
    public string word;
}

public class WordDetecter : MonoBehaviour
{

    //public Dictionary<string, TypeBanWord> BanWord = new Dictionary<string, TypeBanWord>();
    List<string> BanWord = new List<string>();

    private void Start()
    {
        //Test msg
        //Debug.LogError(CheckBanChatting("신발 신고 다니지마라"));
    }
    public string CheckBanChatting(string msg)
    {
        //TypeBanWord banword = CheckcurLanguage();
        //InsertForbbidenWord(banword.type);
        InsertForbbidenWord("sample");

        foreach (var word in BanWord)
        {
            if (msg.Contains(word))
            {
                string locksignal = null;
                for(int i = 0; i < word.Length; i++)
                {
                    locksignal += "*";
                }
                msg = msg.Replace(word, locksignal);
            }
        }
        return msg;
    }

    public void CheckOverlapChatting(string msg)
    {

    }

    public TypeBanWord CheckcurLanguage()
    {
        string curlan = GameOptionManager.Instance.curLanguageCode;

        TypeBanWord banword = new TypeBanWord();

        switch (curlan)
        {
            case "en":
                banword.type = "English";
                break;

            case "kr":
                banword.type = "Korean";
                break;

            case "ru":
                banword.type = "Russian";
                break;

            case "fr":
                banword.type = "French";
                break;

            case "de":
                banword.type = "Germany";
                break;

            case "fa":
                banword.type = "Farsi";
                break;
        }
        return banword;
    }
    public void InsertForbbidenWord(string lan)
    {
        string templan = lan;
        templan += ".txt";

        string filepath = "Assets/Resources/ForbiddenText/kr.txt";
#if UNITY_EDITOR
        
#elif UNITY_IOS

#elif UNITY_ANDROID

#endif
        //filepath = Path.Combine(Application.streamingAssetsPath, templan);
        
        ReadTxt(filepath);
    }

    public void ReadTxt(string filepath)
    {
        string[] forbiddenWord = null;

        if(File.Exists(filepath))
        {
            forbiddenWord = File.ReadAllLines(filepath);
            for(int i = 0; i < forbiddenWord.Length; i++)
            {
                BanWord.Add(forbiddenWord[i]);
            }
        }
    }
}
