using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Breakable : MonoBehaviour
{
    public List<GameObject> breakablePieces;
    float timeToBreak = 2;
    float timer = 0;

    public UnityEvent OnBreak;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var item in breakablePieces)
        {
            item.SetActive(false);
        }
    }

    public void Break()
    {
        timer += Time.deltaTime;
        if (timer >= timeToBreak)
        {
            foreach (var item in breakablePieces)
            {
                item.SetActive(true);
            }

            OnBreak.Invoke();

            gameObject.SetActive(false);
        }
    }
}
