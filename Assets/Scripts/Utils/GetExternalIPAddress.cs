using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GetExternalIPAddress : MonoBehaviour
{
    string ip = "";

    void Start()
    {
        StartCoroutine(GetIPAddress());
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0,0,100,25),ip);
    }

    //Taken from Goodgulf
    IEnumerator GetIPAddress()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");
        yield return www.SendWebRequest();
        
            
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            ip = www.error;
            Debug.Log(www.error);
        }
        else
        {
            string result = www.downloadHandler.text;

            // This results in a string similar to this: <html><head><title>Current IP Check</title></head><body>Current IP Address: 123.123.123.123</body></html>
            // where 123.123.123.123 is your external IP Address.
            //  Debug.Log("" + result);

            string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
            string a2 = a[1].Substring(1);  // Get the substring after the :
            string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
            string a4 = a3[0];              // Get the substring before the tag.
            ip = a4;

            Debug.Log("External IP Address = " + a4);
        }
    }
}