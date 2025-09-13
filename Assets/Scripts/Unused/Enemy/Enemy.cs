using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject body;
    private Rigidbody rig;


    private void Awake()
	{
        rig = GetComponent<Rigidbody>();
        rig.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
	}

	private void OnCollisionEnter(Collision coll)
	{
		if (coll.gameObject.CompareTag("Bullet"))
		{
            Debug.Log("ÇÇ°Ý!");
            MeshRenderer meshRenderer = body.GetComponent<MeshRenderer>();
            meshRenderer.material.color = Color.red;
        }
	}
}
