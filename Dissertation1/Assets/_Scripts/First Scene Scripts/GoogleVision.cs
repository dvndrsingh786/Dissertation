using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SimpleJSON;


public class GoogleVision : MonoBehaviour
{
    public static GoogleVision instance;
    public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
    public string apiKey = ""; //Put your google cloud vision api key here
    public FeatureType featureType = FeatureType.TEXT_DETECTION;
    public int maxResults = 10;
    public GameObject resPanel;
    public Text responseText, responseArray;

    public Texture2D texture2D;
    Dictionary<string, string> headers;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json; charset=UTF-8");

        if (apiKey == null || apiKey == "")
            Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");

        //StartCoroutine(Capture());
    }

    private IEnumerator Capture()
    {
        yield return new WaitForSeconds(2);

        byte[] jpg = texture2D.EncodeToJPG();
        string base64 = System.Convert.ToBase64String(jpg);
        // #if UNITY_WEBGL  
        //          Application.ExternalCall("post", this.gameObject.name, "OnSuccessFromBrowser", "OnErrorFromBrowser", this.url + this.apiKey, base64, this.featureType.ToString(), this.maxResults);
        // #else

        AnnotateImageRequests requests = new AnnotateImageRequests();
        requests.requests = new List<AnnotateImageRequest>();

        AnnotateImageRequest request = new AnnotateImageRequest();
        request.image = new ImageForGoogleVision();
        request.image.content = base64;
        request.features = new List<Feature>();
        Feature feature = new Feature();
        feature.type = this.featureType.ToString();
        feature.maxResults = this.maxResults;
        request.features.Add(feature);
        requests.requests.Add(request);

        string jsonData = JsonUtility.ToJson(requests, false);
        if (jsonData != string.Empty)
        {
            string url = this.url + this.apiKey;
            byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
            using (WWW www = new WWW(url, postData, headers))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    Debug.Log(www.text);
                    string responses = www.text.Replace("\n", "").Replace(" ", "");
                    SimpleVisionApiResponse apiResponse = JsonUtility.FromJson<SimpleVisionApiResponse>(responses);
                    if (apiResponse != null && apiResponse.responses != null && apiResponse.responses.Length > 0
    && apiResponse.responses[0].textAnnotations != null && apiResponse.responses[0].textAnnotations.Length > 0)
                    {
                        string extractedText = apiResponse.responses[0].textAnnotations[0].description;
                        Debug.Log("Extracted Text: " + extractedText);
                    }

                    //JSONNode res = JSON.Parse(responses);
                    //string fullText = res["responses"][0]["textAnnotations"][0]["description"].ToString().Trim('"');
                    //if (fullText != "")
                    //{
                    //    Debug.Log("OCR Response: " + fullText);
                    //    resPanel.SetActive(true);
                    //    responseText.text = fullText.Replace("\\n", " ");
                    //    fullText = fullText.Replace("\\n", ";");
                    //    string[] texts = fullText.Split(';');
                    //    responseArray.text = "";
                    //    for (int i = 0; i < texts.Length; i++)
                    //    {
                    //        responseArray.text += texts[i];
                    //        if (i != texts.Length - 1)
                    //            responseArray.text += ", ";
                    //    }
                    //}
                }
                else
                {
                    Debug.Log("Error: " + www.error);
                }
            }
        }
        // #endif

    }

}



[System.Serializable]
public class SimpleVisionApiResponse
{
    public Response[] responses;
}

[System.Serializable]
public class Response
{
    public TextAnnotation[] textAnnotations;
}

[System.Serializable]
public class TextAnnotation
{
    public string description;
}

[System.Serializable]
public class AnnotateImageRequests
{
    public List<AnnotateImageRequest> requests;
}

[System.Serializable]
public class AnnotateImageRequest
{
    public ImageForGoogleVision image;
    public List<Feature> features;
}

[System.Serializable]
public class ImageForGoogleVision
{
    public string content;
}

[System.Serializable]
public class Feature
{
    public string type;
    public int maxResults;
}

public enum FeatureType
{
    TYPE_UNSPECIFIED,
    FACE_DETECTION,
    LANDMARK_DETECTION,
    LOGO_DETECTION,
    LABEL_DETECTION,
    TEXT_DETECTION,
    SAFE_SEARCH_DETECTION,
    IMAGE_PROPERTIES
}