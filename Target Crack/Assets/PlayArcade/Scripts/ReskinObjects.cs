using UnityEngine;
using UnityEngine.UI;

public class ReskinObjects : MonoBehaviour
{
    public PlayArcadeIntegration master;
    public string coinName;
    public Image CoinImage;
    public Texture2D CoinTexture;
    public Sprite CoinSprite;
    public Material CoinMaterial;
    
    void OnEnable()
    {
        master = PlayArcadeIntegration.Instance;
        master.DownloadMaterialTexture(coinName, CoinTexture, CoinMaterial);
        //master.TestTextureCustomization();
        // reskin everything we had saved
        CoinImage = master.savedCoinImage;
        //CoinMaterial.mainTexture = CoinImage.mainTexture;
        //CoinMaterial.SetTexture("_MainTex", CoinImage.mainTexture);
    }

    // credit button was main menu, now we're in-game and just restarting. No freebies...
    public void ButtonChargePlayerAgain()
    {
        master.GetComponent<PlayArcade>().StartGameRequest();
    }
}
