using UnityEngine;

public class AzureOpenURL : MonoBehaviour
{
    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }
}