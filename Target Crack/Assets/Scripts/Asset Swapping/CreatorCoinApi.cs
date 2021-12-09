using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CreatorCoinApi : MonoBehaviour
{
	[SerializeField]
	private string m_CreatorCoin;

	[SerializeField]
	private SpriteRenderer m_SpriteRenderer;

	private void Start()
	{
		StartCoroutine(GetCreatorCoin());
	}

	// Get data about the creator coin. Including the location of its coin image.
	private IEnumerator GetCreatorCoin()
	{
		using var webRequest = UnityWebRequest.Get($"https://api.rally.io/api/creator-coins/{m_CreatorCoin}");

		yield return webRequest.SendWebRequest();

		switch (webRequest.result)
		{
			case UnityWebRequest.Result.ConnectionError:
			case UnityWebRequest.Result.DataProcessingError:
				Debug.LogError("Error: " + webRequest.error);
				break;

			case UnityWebRequest.Result.ProtocolError:
				Debug.LogError("HTTP Error: " + webRequest.error);
				break;

			case UnityWebRequest.Result.Success:
				Debug.Log("Received: " + webRequest.downloadHandler.text);
				var creatorCoin = JsonConvert.DeserializeObject<CreatorCoinData>(webRequest.downloadHandler.text);
				Debug.Log(creatorCoin.ToString());
				// Handle creator coin data.
				OnCreatorCoinDataReceived(creatorCoin);
				break;
		}
	}

	// Get the creator coin image.
	private IEnumerator GetCreatorCoinSprite(CreatorCoinData creatorCoin)
	{
		using var webRequest = UnityWebRequestTexture.GetTexture(creatorCoin.ImageUrl);

		yield return webRequest.SendWebRequest();

		if (webRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.Log(webRequest.error);
		}
		else
		{
			var texture = ((DownloadHandlerTexture) webRequest.downloadHandler).texture;
			// Convert downloaded texture to a Sprite.
			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2, 100.0f);
			// Handle new sprite data.
			OnCreatorCoinSpriteReceived(creatorCoin, sprite);
		}
	}

	private void OnCreatorCoinDataReceived(CreatorCoinData creatorCoin)
	{
		StartCoroutine(GetCreatorCoinSprite(creatorCoin));
	}

	private void OnCreatorCoinSpriteReceived(CreatorCoinData creatorCoin, Sprite sprite)
	{
		// Assign sprite to any SpriteRenderer.
		m_SpriteRenderer.sprite = sprite;
	}
}
