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

    public void GenerateImageStart()
	{
		GameManager.instance.ShowLoadingPanel();

		string description = GameManager.instance.promptInputField.text;
		string resolution = "1024x1024"; // Possible Resolution 256x256, 512x512, or 1024x1024.
										 //string resolution = "256x256"; // Possible Resolution 256x256, 512x512, or 1024x1024.
		//resolution = "256x256"; //if DALL E 2
		if (GameManager.instance.currentRoom == 1)
		{
			StartCoroutine(GenerateImage(description, resolution));
		}
		else if(GameManager.instance.currentRoom == 2)
		{
			StartCoroutine(EditImage(GameManager.instance.imageUrlField.text, description, resolution));
		}
		else
		{

		}
	}

	IEnumerator GenerateImage(string description, string resolution)
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
				GameManager.instance.HideLoadingPanel();
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
					LoadAndApplyTexture(item.url);
					break;
				}
			}
        }
	}


    IEnumerator EditImage(string imageUrl, string description, string resolution)
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
			GameManager.instance.HideLoadingPanel();
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
                LoadAndApplyTexture(item.url);
                break;
            }
        }
    }

	string GetStringFromTexture2D(Texture2D target)
	{
		byte[] bArray = target.GetRawTextureData();
		return Convert.ToBase64String(bArray);
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
				GameManager.instance.DALLEImageReceived(texture);
				//Utility.WriteImageOnDisk(_texture, System.DateTime.Now.Millisecond + "_createImg_" + i + "_.jpg");
			}
			else
			{
				Debug.LogError($"Failed to load texture from {_url}: {uwr.error}");
			}
			GameManager.instance.HideLoadingPanel();
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