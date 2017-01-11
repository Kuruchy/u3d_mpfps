using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Text killCount;
    public Text deathCount;

    private void Start()
    {
        // We call the GetData but the data may take some seconds to arrive
        // therefor we use an OnReceivedData to receive it afterwards
        if (UserAccountManager.IsLoggedIn)
        {
            UserAccountManager.instance.GetData(OnReceivedData);
        }
    }

    void OnReceivedData(string data)
    {
        killCount.text = DataTranslator.DataToKills(data) + " KILLS";
        deathCount.text = DataTranslator.DataToDeaths(data) + " DEATHS";
    }

}
