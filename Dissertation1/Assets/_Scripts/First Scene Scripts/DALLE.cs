using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.IO;
using System.Net.Http;

public class DALLE : MonoBehaviour
{

	private string IMAGE_GENERTION_API_URL = "https://api.openai.com/v1/images/generations";
	private string IMAGE_EDIT_API_URL = "https://api.openai.com/v1/images/edits";
    [SerializeField] string ApiKey = "";
	[SerializeField] string ORGANIZATION_KEY = "org-wr7avNWc2Yg2hctjjcNVstKb";
	[SerializeField] Texture2D image;
	[SerializeField] Texture2D mask;
	public static DALLE instance;

    private void Awake()
    {
		instance = this;
	}

	public IEnumerator GenerateImage(string description, string resolution, Action<CoroutineReturner> action)
	{
		GenerateImageRequestModel reqModel = new GenerateImageRequestModel();
		reqModel.model = "dall-e-3";
		reqModel.prompt = description;
		reqModel.n = 1;
		reqModel.size = resolution;

		string jsonn = JsonUtility.ToJson(reqModel);
		Debug.Log(jsonn);

		using (UnityWebRequest request = UnityWebRequest.PostWwwForm(IMAGE_GENERTION_API_URL, "POST"))
		{
			byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonn);
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-type", "application/json; charset=UTF-8");
			request.SetRequestHeader("Authorization", "Bearer " + ApiKey);
			request.SetRequestHeader("OpenAI-Organization", ORGANIZATION_KEY);
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
				MyJsonData myData = JsonUtility.FromJson<MyJsonData>(request.downloadHandler.text);

				// Example usage of the deserialized object
				Debug.Log($"Created: {myData.created}");
				foreach (var item in myData.data)
				{
					Debug.Log($"URL: {item.url}");
					StartCoroutine(LoadTexture(item.url, (obj) =>
					{
						action(obj);
					}));
					break;
				}
			}
        }
	}


    public IEnumerator EditImage(string imageUrl, string description, string resolution, Action<CoroutineReturner> action)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("prompt", description);

		//byte[] imageData = image.GetRawTextureData();
		//byte[] imageData = File.ReadAllBytes(Application.dataPath + "/maskk.png");
		byte[] imageData = File.ReadAllBytes(Application.dataPath + "/img.png");
		//byte[] maskData = File.ReadAllBytes(Application.dataPath + "/mask.png");



		formData.AddBinaryData("image", imageData, "image.png", "image/png");
        //formData.AddBinaryData("mask", maskData, "mask.png", "image/png");

        UnityWebRequest request = UnityWebRequest.Post(IMAGE_EDIT_API_URL, formData);
        request.SetRequestHeader("Authorization", "Bearer " + ApiKey);
        request.SetRequestHeader("OpenAI-Organization", ORGANIZATION_KEY);
        Debug.Log("Request sent");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + request.error);
            Debug.Log("Response code: " + request.responseCode);
            Debug.Log("Response body: " + request.downloadHandler.text);
			CoroutineReturner temp = new CoroutineReturner();
			temp.isSuccess = false;
			temp.errorMessage = request.error;
			action(temp);
		}
		else
        {
            // Request successful, handle response here
            Debug.Log("Form request successful");
            Debug.Log("Response body: " + request.downloadHandler.text);
            Debug.Log(request.downloadHandler.text);
            MyJsonData myData = JsonUtility.FromJson<MyJsonData>(request.downloadHandler.text);

            // Example usage of the deserialized object
            Debug.Log($"Created: {myData.created}");
            foreach (var item in myData.data)
            {
                Debug.Log($"URL: {item.url}");
                StartCoroutine(LoadTexture(item.url, (obj) =>
				{
					action(obj);
				}));
                break;
            }
        }
    }

	string GetStringFromTexture2D(Texture2D target)
	{
		byte[] bArray = target.GetRawTextureData();
		return Convert.ToBase64String(bArray);
	}

	IEnumerator LoadTexture(string _url, Action<CoroutineReturner> actionLT)
	{
		Debug.Log("Loading Texture");
		using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url))
		{
			yield return uwr.SendWebRequest();

			CoroutineReturner temp = new CoroutineReturner();

			if (uwr.result == UnityWebRequest.Result.Success)
			{
				Debug.Log("Success");
				Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
				//targetQuad.GetComponent<Renderer>().material.mainTexture = texture;
				//Utility.WriteImageOnDisk(_texture, System.DateTime.Now.Millisecond + "_createImg_" + i + "_.jpg");
				temp.isSuccess = true;
				temp.generatedTexture = texture;
				Debug.Log("Alsmost acitjo");
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

	//private void WriteImageOnDisk(Texture2D texture, string fileName)
	//{
	//	byte[] textureBytes = texture.EncodeToPNG();
	//	string path = Application.persistentDataPath + fileName;
	//	File.WriteAllBytes(path, textureBytes);
	//	Debug.Log("File Written On Disk! " + path);
	//}
}


[Serializable]
public class GenerateImageRequestModel
{
	public string model;
	public string prompt;
	public int n;
	public string size;
}

public class EditImageRequestModel
{
	public byte[] image;
	public byte[] mask;
    public string model;
    public string prompt;
    public int n;
    public string size;
}

[Serializable]
public class DataItem
{
	public string url;
}

[Serializable]
public class MyJsonData
{
	public long created;
	public DataItem[] data;
}