using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    public LineRenderer lineRenderer; // 총알 궤적
    float duration = 1f; // 사라지는 속도

    public void ShowTracer(Vector3 startPos, Vector3 endPos)
	{
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        StartCoroutine(FadeTracer());
	}

    IEnumerator FadeTracer()
	{
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
	}
}
