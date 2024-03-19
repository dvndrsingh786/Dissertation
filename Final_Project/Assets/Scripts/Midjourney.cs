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
    [SerializeField] TMP_InputField inputText;

    public void GenerateImageStart()
    {
        StartCoroutine(GenerateImageFromPrompt(inputText.text));
    }

    IEnumerator GenerateImageFromPrompt(string prompt)
    {
        currentStage = Stage.Generating;
        string url = "https://api.midjourneyapi.xyz/mj/v2/imagine";
        string apiKey = ApiKey;

        ImagineJson abc = new ImagineJson();
        abc.prompt = prompt;

        string jsonn = JsonUtility.ToJson(abc);
        Debug.Log(jsonn);
        //WWWForm form = new WWWForm();
        //form.AddField("prompt", prompt);
        //form.AddField("aspect_ratio", "4:3");
        //form.AddField("process_mode", "relax");
        //form.AddField("webhook_endpoint", "");
        //form.AddField("webhook_secret", "");

        using (UnityWebRequest request = UnityWebRequest.Post(url, "POST"))
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

        using (UnityWebRequest request = UnityWebRequest.Post(url, "POST"))
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

        using (UnityWebRequest request = UnityWebRequest.Post(url, "POST"))
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

    [SerializeField] GameObject previewObj;
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
                    previewObj.GetComponent<Renderer>().material.mainTexture = texture;
                }
                else
                {
                    Debug.LogError($"Failed to load texture from {_url}: {uwr.error}");
                }
            }
    }

}

[System.Serializable]
public class ImagineJson
{
    public string prompt;
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

