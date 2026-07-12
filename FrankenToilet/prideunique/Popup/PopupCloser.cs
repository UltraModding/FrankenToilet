using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrankenToilet.prideunique;

[ConfigureSingleton(SingletonFlags.DestroyDuplicates | SingletonFlags.PersistAutoInstance)]
public class PopupCloser : MonoSingleton<PopupCloser>
{
    public void Awake() {}
    public void Update()
    {
        if (!InputManager.Instance.InputSource.Fire1.WasPerformedThisFrame)
            return;

        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        if (results.Count <= 0)
            return;

        foreach(var result in results)
        {
            if (result.gameObject == null)
                continue;

            if (result.gameObject.layer == 13 /*Always on top*/
                && result.gameObject.TryGetComponent<Popup>(out var popup))
            {
                popup.Destroy();
            }
        }
    }
}
