using UnityEngine;
using System.Collections;

public class WobbleAnimation : MonoBehaviour
{
    public bool vertical = true;
    public float distance = 1.0f;
    public float speed = 1.0f;
    private float time = 0;

    public void OnEnable()
    {
        time = 0;
        StartCoroutine("Wobble");
    }

    private IEnumerator Wobble()
    {
        Vector3 direction = vertical ? Vector3.up : Vector3.right;
        float offset;
        while (true)
        {
            time += Time.deltaTime;
            offset = distance * Mathf.Sin(speed * time);
            transform.localPosition += direction * offset;
            yield return null;
        }
    }

    public void OnDisable()
    {
        StopCoroutine("Wobble");
    }
}