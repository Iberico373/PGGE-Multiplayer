using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
  private void Start()
  {
    StartCoroutine(Coroutine_Delete(5));
  }

  private void OnCollisionEnter(Collision collision)
  {
    IDamageable obj = collision.gameObject.GetComponent<IDamageable>();
    if (obj != null)
    {
      obj.TakeDamage();
    }
  }

  IEnumerator Coroutine_Delete(int sec)
  {
    yield return new WaitForSeconds(sec);
    Destroy(gameObject);
  }
}
