using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
  
    void Update()
    {
        transform.position += Vector3.down *1000 *Time.deltaTime;
        StartCoroutine(nameof(DestroyDrop));
    }
   IEnumerator DestroyDrop()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
