using UnityEngine;
using UnityEngine.UI;

public class UserAccountLobby : MonoBehaviour {

    public Text usernameText;

    private void Start()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            usernameText.text = UserAccountManager.LoggedInUsername;
        }
    }

    public void LogOut()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            UserAccountManager.instance.LogOut();
        }
    }
}
