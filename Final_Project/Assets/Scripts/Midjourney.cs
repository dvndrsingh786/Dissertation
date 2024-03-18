using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;


public class Midjourney : MonoBehaviour
{
    [SerializeField] string ApiKey = "";

    private void Start()
    {
        //StartCoroutine(GenerateImageFromPrompt("Cat Taking Bath"));
        StartCoroutine(FetchTaskStatus("c29e520d-2da6-40b2-a546-802ad3b85e20"));
        //StartCoroutine(Upscale("f213efbd-68a5-46c8-88ed-a2df9bd2b307"));
    }

    IEnumerator GenerateImageFromPrompt(string prompt)
    {
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
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(request.downloadHandler.text);
                Debug.Log("Task ID: " + response.task_id);
                Temp(response.task_id);
                // Process the response here
            }
        }
    }

    void Temp(string _id)
    {
        StartCoroutine(FetchTaskStatus(_id));
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
                ApiResponse1 response = JsonUtility.FromJson<ApiResponse1>(request.downloadHandler.text);
                Debug.Log("Status: " + response.status);
                if (response.task_result != null && response.task_result.image_urls != null && response.task_result.image_urls.Count > 0)
                {
                    Debug.Log("First Image URL: " + response.task_result.image_urls[0]);
                    LoadAndApplyTexture(response.task_result.image_urls[0]);
                }
                else
                {
                    Debug.Log(response.task_result);
                    Debug.Log(response.task_result.image_urls);
                    Debug.Log(response.task_result.image_urls.Count);
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
                ApiResponse1 response = JsonUtility.FromJson<ApiResponse1>(request.downloadHandler.text);
                Debug.Log("Status: " + response.status);
                if (response.task_result != null && response.task_result.image_urls != null && response.task_result.image_urls.Count > 0)
                {
                    Debug.Log("First Image URL: " + response.task_result.image_urls[0]);
                    LoadAndApplyTexture(response.task_result.image_urls[0]);
                }
                else
                {
                    Debug.Log(response.task_result);
                    Debug.Log(response.task_result.image_urls);
                    Debug.Log(response.task_result.image_urls.Count);
                }
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
public class ApiResponse
{
    public string task_id;
    public string status;
    public string message;
}

[System.Serializable]
public class ApiResponse1
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

