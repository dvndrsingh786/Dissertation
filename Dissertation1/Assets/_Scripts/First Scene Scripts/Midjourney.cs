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

    public IEnumerator GenerateImageFromPrompt(string prompt, Action<CoroutineReturner> action)
    {
        currentStage = Stage.Generating;
        string url = "https://api.midjourneyapi.xyz/mj/v2/imagine";
        string apiKey = ApiKey;

        ImagineJson abc = new ImagineJson();
        abc.prompt = prompt;
        abc.process_mode = "relax";
        abc.aspect_ratio = "7:4";

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
                CoroutineReturner temp = new CoroutineReturner();
                temp.isSuccess = false;
                temp.errorMessage = request.error;
                action(temp);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseImagine response = JsonUtility.FromJson<ApiResponseImagine>(request.downloadHandler.text);
                Debug.Log("Task ID: " + response.task_id);
                currentStage = Stage.CheckingStatus;
                StartCoroutine(FetchTaskStatus(response.task_id, (obj) =>
                {
                    action(obj);
                }));
            }
        }
    }

    IEnumerator EditAnImage(string imageUrl, string prompt, Action<CoroutineReturner> action)
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
                Debug.Log("Response code: " + request.responseCode);
                Debug.Log("Response body: " + request.downloadHandler.text);
                CoroutineReturner temp = new CoroutineReturner();
                temp.isSuccess = false;
                temp.errorMessage = request.error;
                action(temp);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseImagine response = JsonUtility.FromJson<ApiResponseImagine>(request.downloadHandler.text);
                Debug.Log("Task ID: " + response.task_id);
                currentStage = Stage.CheckingStatus;
                StartCoroutine(FetchTaskStatus(response.task_id,(obj) =>
                {
                    action(obj);
                }));
            }
        }
    }

    IEnumerator Describe(Action<CoroutineReturner> action)
    {
        string imageUrl = "https://davdissertation.s3.eu-west-2.amazonaws.com/s-l1200-ezgif.com-webp-to-png-converter.png";
        currentStage = Stage.Generating;
        string url = "https://api.midjourneyapi.xyz/mj/v2/describe";
        string apiKey = ApiKey;

        DescribeJson abc = new DescribeJson();
        //abc.prompt = prompt + " Image Url: " + imageUrl;
        abc.image_url = imageUrl;
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
                Debug.Log("Response code: " + request.responseCode);
                Debug.Log("Response body: " + request.downloadHandler.text);
                CoroutineReturner temp = new CoroutineReturner();
                temp.isSuccess = false;
                temp.errorMessage = request.error;
                action(temp);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseImagine response = JsonUtility.FromJson<ApiResponseImagine>(request.downloadHandler.text);
                Debug.Log("Task ID: " + response.task_id);
                currentStage = Stage.CheckingStatus;
                StartCoroutine(FetchTaskStatus(response.task_id,(obj) =>
                {
                    action(obj);
                }));
            }
        }
    }

    IEnumerator FetchTaskStatus(string taskId, Action<CoroutineReturner> actionFTS)
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
                CoroutineReturner temp = new CoroutineReturner();
                temp.isSuccess = false;
                temp.errorMessage = request.error;
                actionFTS(temp);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseFetch response = JsonUtility.FromJson<ApiResponseFetch>(request.downloadHandler.text);
                Debug.Log("Status: " + response.status);
                if (response.status == "finished" && currentStage == Stage.CheckingStatus)
                {
                    currentStage = Stage.Upscaling;
                    StartCoroutine(Upscale(taskId, (obj) =>
                    {
                        actionFTS(obj);
                    }));
                }
                else if (currentStage == Stage.FetchingFinalImage)
                {
                    if (response.status == "finished")
                    {
                        currentStage = Stage.LoadingImage;
                        StartCoroutine(LoadTexture(response.task_result.image_url,(obj) =>
                        {
                            actionFTS(obj);
                        }));
                    }
                    else
                    {
                        yield return new WaitForSeconds(5);
                        StartCoroutine(FetchTaskStatus(taskId, (obj) =>
                        {
                            actionFTS(obj);
                        }));
                    }
                }
                else
                {
                    yield return new WaitForSeconds(5);
                    StartCoroutine(FetchTaskStatus(taskId,(obj) =>
                    {
                        actionFTS(obj);
                    }));
                }
            }
        }
    }

    IEnumerator Upscale(string taskId, Action<CoroutineReturner> actionUS)
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
                CoroutineReturner temp = new CoroutineReturner();
                temp.isSuccess = false;
                temp.errorMessage = request.error;
                actionUS(temp);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                ApiResponseUpscale response = JsonUtility.FromJson<ApiResponseUpscale>(request.downloadHandler.text);
                Debug.Log("Status: " + response.status);
                currentStage = Stage.FetchingFinalImage;
                StartCoroutine(FetchTaskStatus(response.task_id, (obj) =>
                {
                    actionUS(obj);
                }));
            }
        }
    }

    IEnumerator LoadTexture(string _url, Action<CoroutineReturner> actionLT)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return uwr.SendWebRequest();

            CoroutineReturner temp = new CoroutineReturner();
            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                //targetQuad.GetComponent<Renderer>().material.mainTexture = texture;
                temp.isSuccess = true;
                temp.generatedTexture = texture;
                actionLT(temp);
            }
            else
            {
                Debug.LogError($"Failed to load texture from {_url}: {uwr.error}");
                temp.isSuccess = false;
                temp.errorMessage = uwr.error;
                actionLT(temp);
            }
        }
    }

}

[System.Serializable]
public class ImagineJson
{
    public string prompt;
    public string process_mode;
    public string aspect_ratio;
}

[System.Serializable]
public class DescribeJson
{
    public string image_url;
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

