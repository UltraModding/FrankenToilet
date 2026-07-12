using System.Collections;
using UnityEngine;

namespace FrankenToilet.prideunique;

public sealed class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            var go = new GameObject("[CoroutineRunner]");
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;

            _instance = go.AddComponent<CoroutineRunner>();
            return _instance;
        }
    }

    public static Coroutine Run(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    public static Coroutine RunDelayed(float delaySeconds, System.Action action)
    {
        return Run(RunDelayedInternal(delaySeconds, action));
    }

    private static IEnumerator RunDelayedInternal(float delaySeconds, System.Action action)
    {
        yield return new WaitForSeconds(delaySeconds);
        action?.Invoke();
    }
}
