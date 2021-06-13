using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMatchSpawner : MonoBehaviour
{
    const int SCREEN_BOUNDS = 12;

    void Awake()
    {
        int matchCount = Random.Range(5, transform.childCount);

        for (int i = 0; i < matchCount; i++) {
            Transform child = gameObject.transform.GetChild(Random.Range(0, transform.childCount));
            if (child.gameObject.activeSelf) {
                i--;
            }
            child.gameObject.SetActive(true);

            child.GetChild(0).transform.position = new Vector2(
                Random.Range(-SCREEN_BOUNDS, SCREEN_BOUNDS),
                Random.Range(-SCREEN_BOUNDS, SCREEN_BOUNDS)
            );
            child.GetChild(1).transform.position = new Vector2(
                Random.Range(-SCREEN_BOUNDS, SCREEN_BOUNDS),
                Random.Range(-SCREEN_BOUNDS, SCREEN_BOUNDS)
            );
        }
    }
}
