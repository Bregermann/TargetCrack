using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TQGLeadboard : MonoBehaviour
{
    public List<TQGHighScoreUI> scoreUI = new List<TQGHighScoreUI>();

    public void Awake()
    {
        int i = 0;
        foreach (TQGHighScoreUI ui in scoreUI)
        {
            if (PlayArcadeIntegration.Instance.CoinScores.Count > i)
            {
                ui.gameObject.SetActive(true);
                ui.number.text = (i + 1).ToString();
                ui.playerName.text = ToTitleCase(PlayArcadeIntegration.Instance.CoinScores[i].user_name);
                ui.score.text = PlayArcadeIntegration.Instance.CoinScores[i].score.ToString();
                i++;
            }
            else
            {
                ui.gameObject.SetActive(false);
            }
        }
    }
    
    public string ToTitleCase(string str)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
}

