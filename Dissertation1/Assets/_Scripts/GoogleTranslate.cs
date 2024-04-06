using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class GoogleTranslate : MonoBehaviour
{

    public static GoogleTranslate instance;
    public string apiKey = "";

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(TranslateText("Wood", "en", "fr"))
    }

    IEnumerator TranslateText(string textToTranslate, string sourceLang, string targetLang)
    {
        string url = $"https://translation.googleapis.com/language/translate/v2?key={apiKey}";

        TranslationRequest request = new TranslationRequest
        {
            q = textToTranslate,
            source = sourceLang,
            target = targetLang,
            format = "text"
        };

        string json = JsonUtility.ToJson(request);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] jsonToSend = new UTF8Encoding().GetBytes(json);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Translation Error: " + webRequest.error);
            }
            else
            {
                TranslationResponse response = JsonUtility.FromJson<TranslationResponse>(webRequest.downloadHandler.text);
                Debug.Log("Translated Text: " + response.data.translations[0].translatedText);
            }
        }
    }
}

[System.Serializable]
public class TranslationRequest
{
    public string q;
    public string source;
    public string target;
    public string format;
}

[System.Serializable]
public class TranslationResponse
{
    public Data data;
}

[System.Serializable]
public class Data
{
    public Translation[] translations;
}

[System.Serializable]
public class Translation
{
    public string translatedText;
    public string detectedSourceLanguage;
}