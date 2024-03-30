using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using TMPro;

public enum Stage
{
    Generating,
    CheckingStatus,
    Upscaling,
    FetchingFinalImage,
    LoadingImage,
    ERROR
}

public class Midjourney : MonoBehaviour
{
    [SerializeField] string ApiKey = "";
    [SerializeField] Stage currentStage;
    public static Midjourney instance;

    private void Awake()
    {
        instance = this;
    }

    public void GenerateImageStart()
    {
        GameManager.instance.ShowLoadingPanel();
        if (GameManager.instance.currentRoom == 1)
        {
            StartCoroutine(GenerateImageFromPrompt(GameManager.instance.promptInputField.text));
        }
        else if (GameManager.instance.currentRoom == 2)
        {
            StartCoroutine(EditAnImage(GameManager.instance.imageUrlField.text, GameManager.instance.promptInputField.text));
        }
        else
        {

        }
    }

    IEnumerator GenerateImageFromPrompt(string prompt)
    {
        currentStage = Stage.Generating;
        string url = "https://api.midjourneyapi.xyz/mj/v2/imagine";
        string apiKey = ApiKey;

        ImagineJson abc = new ImagineJson();
        abc.prompt = prompt;
        abc.process_mode = "relax";

        string jsonn = JsonUtility.ToJson(abc);
        Debug.Log(jsonn);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonn);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("X-API-KEY", apiKey);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseImagine response = JsonUtility.FromJson<ApiResponseImagine>(request.downloadHandler.text);
                Debug.Log("Task ID: " + response.task_id);
                currentStage = Stage.CheckingStatus;
                StartCoroutine(FetchTaskStatus(response.task_id));
            }
        }
    }

    IEnumerator EditAnImage(string imageUrl, string prompt)
    {
        imageUrl = "https://davdissertation.s3.eu-west-2.amazonaws.com/1.png";
        currentStage = Stage.Generating;
        string url = "https://api.midjourneyapi.xyz/mj/v2/imagine";
        string apiKey = ApiKey;

        ImagineJson abc = new ImagineJson();
        //abc.prompt = prompt + " Image Url: " + imageUrl;
        abc.prompt = imageUrl + " " + prompt;
        abc.process_mode = "relax";

        string jsonn = JsonUtility.ToJson(abc);
        Debug.Log(jsonn);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonn);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("X-API-KEY", apiKey);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseImagine response = JsonUtility.FromJson<ApiResponseImagine>(request.downloadHandler.text);
                Debug.Log("Task ID: " + response.task_id);
                currentStage = Stage.CheckingStatus;
                StartCoroutine(FetchTaskStatus(response.task_id));
            }
        }
    }

    IEnumerator FetchTaskStatus(string taskId)
    {
        string url = "https://api.midjourneyapi.xyz/mj/v2/fetch";

        FetchJson abc = new FetchJson();
        abc.task_id = taskId;
        string jsonn = JsonUtility.ToJson(abc);
        Debug.Log(jsonn);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonn);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseFetch response = JsonUtility.FromJson<ApiResponseFetch>(request.downloadHandler.text);
                Debug.Log("Status: " + response.status);
                if (response.status == "finished" && currentStage == Stage.CheckingStatus)
                {
                    currentStage = Stage.Upscaling;
                    StartCoroutine(Upscale(taskId));
                }
                else if (currentStage == Stage.FetchingFinalImage)
                {
                    if (response.status == "finished")
                    {
                        currentStage = Stage.LoadingImage;
                        LoadAndApplyTexture(response.task_result.image_url);
                    }
                    else
                    {
                        yield return new WaitForSeconds(5);
                        StartCoroutine(FetchTaskStatus(taskId));
                    }
                }
                else
                {
                    yield return new WaitForSeconds(5);
                    StartCoroutine(FetchTaskStatus(taskId));
                }
            }
        }
    }

    IEnumerator Upscale(string taskId)
    {
        string url = "https://api.midjourneyapi.xyz/mj/v2/upscale";
        string apiKey = ApiKey;

        UpscaleJson abc = new UpscaleJson();
        abc.origin_task_id = taskId;
        abc.index = "1";
        string jsonn = JsonUtility.ToJson(abc);
        Debug.Log(jsonn);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonn);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("X-API-KEY", apiKey);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseUpscale response = JsonUtility.FromJson<ApiResponseUpscale>(request.downloadHandler.text);
                Debug.Log("Status: " + response.status);
                currentStage = Stage.FetchingFinalImage;
                StartCoroutine(FetchTaskStatus(response.task_id));
            }
        }
    }

    
    public void LoadAndApplyTexture(string url)
    {
        StartCoroutine(LoadTexture(url));
    }

    IEnumerator LoadTexture(string _url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                //targetQuad.GetComponent<Renderer>().material.mainTexture = texture;
                GameManager.instance.MidjourneyImageReceived(texture);
            }
            else
            {
                Debug.LogError($"Failed to load texture from {_url}: {uwr.error}");
            }
            GameManager.instance.HideLoadingPanel();
        }
    }

}

[System.Serializable]
public class ImagineJson
{
    public string prompt;
    public string process_mode;
}

[System.Serializable]
public class FetchJson
{
    public string task_id;
}

[System.Serializable]
public class UpscaleJson
{
    public string origin_task_id;
    public string index;
}

[System.Serializable]
public class ApiResponseImagine
{
    public string task_id;
    public string status;
    public string message;
}

[System.Serializable]
public class ApiResponseUpscale
{
    public string task_id;
    public string status;
    public TaskResult task_result;
}

[System.Serializable]
public class ApiResponseFetch
{
    public string status;
    public TaskResult task_result;
}

[System.Serializable]
public class TaskResult
{
    public string discord_image_url;
    public string image_url;
    public List<string> image_urls;
    // Add other fields as necessary
}

