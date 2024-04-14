using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

public class DALLE : MonoBehaviour
{

	private string IMAGE_GENERTION_API_URL = "https://api.openai.com/v1/images/generations";
	private string IMAGE_EDIT_API_URL = "https://api.openai.com/v1/images/edits";
	private string MODERATiON_API_URL = "https://api.openai.com/v1/moderations";
	//[SerializeField] string ApiKey = "";
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
			request.SetRequestHeader("Authorization", "Bearer " + Keys.DALLE);
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
        request.SetRequestHeader("Authorization", "Bearer " + Keys.DALLE);
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

	public void ValidateContent(string _contentToCheck, Action<CoroutineReturner> _action)
	{
		StartCoroutine(CheckContentModeration(_contentToCheck, action =>
		{
			CoroutineReturner temp = new CoroutineReturner();
			if (action.isSuccess)
			{
				temp.isSuccess = true;
				action.successMsg = action.successMsg.Replace("\"sexual/minors\":", "\"sexualminors\":")
									   .Replace("\"hate/threatening\":", "\"hatethreatening\":")
									   .Replace("\"violence/graphic\":", "\"violencegraphic\":")
									   .Replace("\"self-harm/intent\":", "\"selfharmintent\":")
									   .Replace("\"self-harm/instructions\":", "\"selfharminstructions\":")
									   .Replace("\"harassment/threatening\":", "\"harassmentthreatening\":");
				ModerationResponse moderationResponse = JsonUtility.FromJson<ModerationResponse>(action.successMsg);
				
				if (moderationResponse.results[0].flagged)
				{
					ModerationResponse.Result.Categories category = moderationResponse.results[0].categories;
					string flagCategory = "";
					int categoriesCount = 0;
					if (category.harassment) { flagCategory = "Harassment"; categoriesCount++; }
					if (category.harassmentthreatening) { flagCategory += AddFlagCategory(categoriesCount, "Harassment Threatening"); categoriesCount++; }
					if (category.hate) { flagCategory += AddFlagCategory(categoriesCount, "Hate"); categoriesCount++; }
					if (category.hatethreatening) { flagCategory += AddFlagCategory(categoriesCount, "Hate Threatening"); categoriesCount++; }
					if (category.selfharm) { flagCategory += AddFlagCategory(categoriesCount, "Self-harm"); categoriesCount++; }
					if (category.selfharminstructions) { flagCategory += AddFlagCategory(categoriesCount, "Self-harm Instructions"); categoriesCount++; }
					if (category.selfharmintent) { flagCategory += AddFlagCategory(categoriesCount, "Self Harm Intent"); categoriesCount++; }
					if (category.sexual) { flagCategory += AddFlagCategory(categoriesCount, "Sexual"); categoriesCount++; }
					if (category.sexualminors) { flagCategory += AddFlagCategory(categoriesCount, "Sexual Minors"); categoriesCount++; }
					if (category.violence) { flagCategory += AddFlagCategory(categoriesCount, "Violence"); categoriesCount++; }
					if (category.violencegraphic) { flagCategory += AddFlagCategory(categoriesCount, "Violence Graphic"); categoriesCount++; }
					Debug.Log(flagCategory);
					string flagDescription = "";
					if (categoriesCount > 1)
					{
						flagDescription = "Content flagged under categories: " + flagCategory;
					}
					else flagDescription = "Content flagged under category: " + flagCategory;
					Debug.Log(flagDescription);
					temp.successMsg = flagDescription;
					temp.isValidContent = false;
				}
				else
				{
					Debug.Log("Content passed moderation checks.");
					temp.successMsg = "Content passed moderation checks.";
					temp.isValidContent = true;
				}
				_action(temp);
			}
			else
			{
				Debug.Log(action.errorMessage);
				temp.isSuccess = false;
				temp.errorMessage = action.errorMessage;
				_action(temp);
			}
		}));
	}

	string AddFlagCategory(int _categoriesCount, string _categoryName)
    {
		string temp = "";
		if (_categoriesCount > 0) temp = ", " + _categoryName;
		else temp = _categoryName;
		return temp;
    }

    IEnumerator CheckContentModeration(string contentToCheck, Action<CoroutineReturner> action)
    {
        var request = new UnityWebRequest(MODERATiON_API_URL, "POST");
        string jsonData = $"{{\"input\":\"{contentToCheck}\"}}";
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {Keys.DALLE}");

        yield return request.SendWebRequest();

		CoroutineReturner temp = new CoroutineReturner();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
			temp.isSuccess = false;
			temp.errorMessage = request.error + "  " + request.downloadHandler.text;
			action(temp);
        }
        else
        {
            Debug.Log("Moderation response received: " + request.downloadHandler.text);
			temp.isSuccess = true;
			temp.successMsg = request.downloadHandler.text;
			action(temp);
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
				Debug.Log("Almost acitjo");
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

[System.Serializable]
public class ModerationResponse
{
	public string id;
	public string model;
	public Result[] results;

	[System.Serializable]
	public class Result
	{
		public bool flagged;
		public Categories categories;
		public CategoryScores category_scores;

		[System.Serializable]
		public class Categories
		{
			public bool sexual;
			public bool hate;
			public bool harassment;
			public bool selfharm;
			public bool sexualminors;
			public bool hatethreatening;
			public bool violencegraphic;
			public bool selfharmintent;
			public bool selfharminstructions;
			public bool harassmentthreatening;
			public bool violence;
		}

		[System.Serializable]
		public class CategoryScores
		{
			public float sexual;
			public float hate;
			public float harassment;
			public float selfHarm;
			public float sexualMinors;
			public float hateThreatening;
			public float violenceGraphic;
			public float selfHarmIntent;
			public float selfHarmInstructions;
			public float harassmentThreatening;
			public float violence;
		}
	}
}
