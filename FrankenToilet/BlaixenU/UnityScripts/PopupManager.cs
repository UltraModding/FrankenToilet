using UnityEngine;

namespace FrankenToilet.BlaixenU.UnityScripts;

public class PopupManager : MonoBehaviour
{
    private GameObject popupObject;

    private float timeOfLastPopup;

    public float TimeSincePopup => Time.realtimeSinceStartup - timeOfLastPopup;

    private void Update()
    {
        if (TimeSincePopup > Random.Range(5, 15))
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
        var canvas = UnityPathHelper.FindCanvas();
        var popupTrans = popupObject.transform;
        popupTrans.SetParent(canvas.transform);
        var rectTrans = popupObject.GetComponent<RectTransform>();

        float posX = 900 + Random.Range(-500, 500); // please dont fucking play on lower resolutions
        float posY = 400 + Random.Range(-200, 200);

        Vector3 pos = new(posX, posY, 0);
        
        rectTrans.SetPositionAndRotation(pos, rectTrans.rotation);

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