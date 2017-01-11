using UnityEngine;
using DatabaseControl;
using System.Collections;
using UnityEngine.SceneManagement;

public class UserAccountManager : MonoBehaviour {

    public static UserAccountManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public static string LoggedInUsername { get; protected set; }
    private static string LoggedInPassword = "";

    public static string LoggedInData { get; protected set; }

    public static bool IsLoggedIn { get; protected set; }

    public string logInSceneName = "Lobby";
    public string logOutSceneName = "LoginMenu";

    public delegate void OnDataReceivedCallBack(string data);

    public void LogOut()
    {
        LoggedInUsername = "";
        LoggedInPassword = "";

        IsLoggedIn = false;

        SceneManager.LoadScene(logOutSceneName);
    }

    public void LogIn(string username, string password)
    {
        LoggedInUsername = username;
        LoggedInPassword = password;

        IsLoggedIn = true;

        SceneManager.LoadScene(logInSceneName);
    }

    public void SendData(string data)
    { //called when the 'Send Data' button on the data part is pressed
        if (IsLoggedIn)
        {
            //ready to send request
            StartCoroutine(sendSendDataRequest(LoggedInUsername, LoggedInPassword, data)); //calls function to send: send data request
        }
    }

    IEnumerator sendSendDataRequest(string username, string password, string data)
    {
        IEnumerator eee = DC.SetUserData(username, password, data);
        while (eee.MoveNext())
        {
            yield return eee.Current;
        }
        WWW returneddd = eee.Current as WWW;
        if (returneddd.text == "ContainsUnsupportedSymbol")
        {
            //One of the parameters contained a - symbol
            Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
        }
        if (returneddd.text == "Error")
        {
            //Error occurred. For more information of the error, DC.Login could
            //be used with the same username and password
            Debug.Log("Data Upload Error: Contains Unsupported Symbol '-'");
        }
    }

    public void GetData(OnDataReceivedCallBack onDataReceived)
    { //called when the 'Get Data' button on the data part is pressed

        if (IsLoggedIn)
        {
            //ready to send request
            StartCoroutine(sendGetDataRequest(LoggedInUsername, LoggedInPassword, onDataReceived)); //calls function to send get data request
        }
    }

    IEnumerator sendGetDataRequest(string username, string password, OnDataReceivedCallBack onDataReceived)
    {
        string data = "Error";

        IEnumerator eeee = DC.GetUserData(username, password);
        while (eeee.MoveNext())
        {
            yield return eeee.Current;
        }
        WWW returnedddd = eeee.Current as WWW;
        if (returnedddd.text == "Error")
        {
            //Error occurred. For more information of the error, DC.Login could
            //be used with the same username and password
            Debug.Log("Data Upload Error. Could be a server error. To check try again, if problem still occurs, contact us.");
        }
        else
        {
            if (returnedddd.text == "ContainsUnsupportedSymbol")
            {
                //One of the parameters contained a - symbol
                Debug.Log("Get Data Error: Contains Unsupported Symbol '-'");
            }
            else
            {
                //Data received in returned.text variable
                string DataRecieved = returnedddd.text;
                data = DataRecieved;
            }
        }

        if(onDataReceived != null)
        {
            onDataReceived.Invoke(data);
        }

    }
}
