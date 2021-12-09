using UnityEngine;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;

public class MobileManagement : MonoBehaviour
{
    public string appleId;
    public string androidAppUrl;
    private int bannerId1;

    public GameObject noAds;

    // Use this for initialization
    private void Awake()
    {
        UM_AdManager.instance.Init();

        UM_InAppPurchaseManager.instance.Init();
        UM_InAppPurchaseManager.instance.addEventListener(UM_InAppPurchaseManager.ON_PURCHASE_FLOW_FINISHED, OnPurchaseFinished);

        UM_GameServiceManager.instance.Connect();
    }

    private void Start()
    {
        //showAds
        //0 = show ads
        //1 = don't show ads
        if (!PlayerPrefs.HasKey("showAds"))
        {
            PlayerPrefs.SetInt("showAds", 1);
        }

        if (PlayerPrefs.GetInt("showAds") == 1)
        {
            bannerId1 = UM_AdManager.instance.CreateAdBanner(TextAnchor.LowerCenter);
            ShowBannerAD();
        }
        else
        {
            HideBannerAD();
            noAds.SetActive(false);
        }
    }

    private void Update()
    {
        if (noAds.activeSelf)
        {
            if (PlayerPrefs.GetInt("showAds") == 0)
            {
                noAds.SetActive(false);
            }
        }
    }

    //Used to detect if the no ad purchase succeeded
    private void OnPurchaseFinished(CEvent e)
    {
        UM_PurchaseResult result = e.data as UM_PurchaseResult;
        if (result.isSuccess)
        {
            //UM_ExampleStatusBar.text = "Product " + result.product.id + " purchase Success";
            PlayerPrefs.SetInt("showAds", 0); //If they purchased it then stop showing ads
            HideBannerAD();
        }
        else
        {
            //UM_ExampleStatusBar.text = "Product " + result.product.id + " purchase Failed";
            //Do Nothing
        }
    }

    public void ShowBannerAD()
    {
        UM_AdManager.instance.ShowBanner(bannerId1);
    }

    public void HideBannerAD()
    {
        UM_AdManager.instance.HideBanner(bannerId1);
    }

    public void NoADs()
    {
        UM_InAppPurchaseManager.instance.Purchase("Adfree");
    }

    public void ShowLeaderBoard()
    {
        UM_GameServiceManager.instance.ShowLeaderBoardsUI();
    }

    public void ShareTargetCrack()
    {
        UM_ShareUtility.ShareMedia("Check out Target Crack on iOS and Android! My High score is " + PlayerPrefs.GetInt("best").ToString());
    }

    public void RateTargetCrack()
    {
        MobileNativeRateUs ratePopUp = new MobileNativeRateUs("Like Target Crack?", "Help support us by rating below!", "Sure!", "Not now", "no");
        ratePopUp.SetAppleId(appleId);
        ratePopUp.SetAndroidAppUrl(androidAppUrl);

        ratePopUp.Start();
    }
}