using UnityEngine;

namespace FrankenToilet.BlaixenU.UnityScripts;

public class PopupManager : MonoBehaviour
{
    private GameObject popupObject;

    private float timeOfLastPopup;

    public float TimeSincePopup => Time.realtimeSinceStartup - timeOfLastPopup;

    private void Update()
    {
        if (TimeSincePopup > 5)
        {
            timeOfLastPopup = Time.realtimeSinceStartup;
            Popup();
        }
    }

    private void Popup()
    {
        switch (Random.Range(1, 4))
        {
            case 1:
            popupObject = Instantiate(AssetMan.Popup1);
            break;
            case 2:
            popupObject = Instantiate(AssetMan.Popup2);
            break;
            case 3:
            popupObject = Instantiate(AssetMan.Popup3);
            break;
        }
        var popupTrans = popupObject.transform;
        popupTrans.SetParent(UnityPathHelper.FindCanvas().transform);
        var rectTrans = popupObject.GetComponent<RectTransform>();

        rectTrans.SetPositionAndRotation(new Vector3(900 + Random.Range(-700, 700), 400 + Random.Range(-300, 300), 0), rectTrans.rotation);

        rectTrans.SetAsLastSibling();
    }
}

public static class UnityPathHelper
{
    public static Canvas FindCanvas()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
        {
            var canvas = root.GetComponent<Canvas>();
            if (canvas != null)
                return canvas;
        }
        return null;
    }
}