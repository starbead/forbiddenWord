using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using Qitz.DataUtil;
using LitJson;

public class BanWordList
{
    public string[] KRword;
    public string[] ENword;
}

public class Langauge
{
    public string KR;
    public string EN;
    //public string RU;
    //public string FR;
    //public string DE;
    //public string FA;
}

public class WordDetecter : MonoBehaviour
{
    public const string version_url = "https://docs.google.com/spreadsheets/d/1qpYs5XfAOIEOoQ8zohFnSkSxCWuMiYrmw3KcL-_8TD8/edit#gid=0";
    
    public Dictionary<string, List<string>> BanWord = new Dictionary<string, List<string>>();
    BanWordList banwordlist = new BanWordList();
    List<string> RecordMsg = new List<string>();

    List<string> KRwordlist = new List<string>();
    List<string> ENwordlist = new List<string>();

    private void Awake()
    {
        //시작할때 미리 금지어 리스트를 로드
        //TypeBanWord banword = CheckcurLanguage();
        //InsertForbbidenWord(banword.type);
    }
    private void Start()
    {
        Application.runInBackground = true;

        StartCoroutine(SheetLoadJson(version_url));
    }
    public IEnumerator SheetLoadJson(string url)
    {
        yield return JsonFromGoogleSpreadSheet.GetJsonArrayFromGoogleSpreadSheetUrl(url, (jsonArray) =>
        {
            foreach (var json in jsonArray)
            {
                Langauge severModel = JsonMapper.ToObject<Langauge>(json.ToString());

                KRwordlist.Add(severModel.KR);
                ENwordlist.Add(severModel.EN);

            }
        });

        BanWord.Add("ko", KRwordlist);
        BanWord.Add("en", ENwordlist);
        //BanWord.Add("ru", banwordlist.RUword);
        //BanWord.Add("fr", banwordlist.FRword);
        //BanWord.Add("de", banwordlist.DEword);
        //BanWord.Add("fa", banwordlist.FAword);
        //Debug.LogError(CheckBanChatting("신발 좀 벗고 다니세요."));
        yield return null;
    }
    public string CheckBanChatting(string msg)
    {
        string curlan = GameOptionManager.Instance.curLanguageCode;
        //InsertForbbidenWord("sample");

        foreach (var word in BanWord[curlan])
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
    public void UpdateRecordChatting(string msg, int limit)
    {
        int lastindex = RecordMsg.Count - 1;

        if(RecordMsg.Count < limit + 1)
        {
            RecordMsg.Add(msg);

            if (IsOverlapChatting(limit))
            {
                Debug.LogError("채팅 중복가능 횟수 초과");
                return;
            }
        }
        else
        {
            List<string> tempList = new List<string>();

            tempList.Add(RecordMsg[lastindex]);

            for(int i = 1; i < limit + 1; i++)
            {
                if (RecordMsg[lastindex].Equals(RecordMsg[lastindex - i]))
                {
                    tempList.Add(RecordMsg[lastindex - i]);
                }
                else
                {
                    break;
                }
            }
            ResetRecordList();
            RecordMsg = tempList;
        }
    }
    //도배 여부 파악
    public bool IsOverlapChatting(int limit)
    {
        
        string prev = null;
        int count = 0;

        if(RecordMsg.Count == limit)
        {
            string current = RecordMsg[RecordMsg.Count - 1];
            int lastindex = RecordMsg.Count - 1;
            
            int num = current.Length / 2;
            string halfstr = current.IndexOf(0, num);

            for (int i = 1; i < limit + 1; i++)
            {
                prev = RecordMsg[lastindex - i];
                if(prev.Contains(halfstr)){
                    count += 1;
                }
                else
                {
                    return false;
                }
            }

            if(count == limit)
            {
                return true;
            }
        }

        return false;
    }

    public void ResetRecordList()
    {
        RecordMsg.Clear();
    }
}
