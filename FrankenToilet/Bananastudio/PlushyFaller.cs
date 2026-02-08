using FrankenToilet.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrankenToilet.Bananastudio;

public class PlushyFaller : MonoBehaviour
{
    

    float t;
    float delayBetweenFalling = 3;

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        if(t >= delayBetweenFalling)
        {
            // get random plush
            GameObject plush = MainThingy.plushieList[Random.Range(0, MainThingy.plushieList.Count)];

            Vector3 position = CameraController.Instance.GetDefaultPos();
            position += new Vector3(0, 25, 0);
            position += CameraController.Instance.transform.forward * 2;
            GameObject clone = Instantiate(plush, position, Quaternion.identity);
            clone.GetComponent<Rigidbody>().AddForce(Vector3.down * 250);
            clone.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 250;

            LogHelper.LogInfo("Fall plush! " + plush.name);
            clone.GetComponent<ItemIdentifier>().onPutDown.Invoke();
            t = 0;
            delayBetweenFalling = Random.Range(1, 10);
            Destroy(clone, 25); // Cleanup to make it not lag as hard.
        }
    }
}