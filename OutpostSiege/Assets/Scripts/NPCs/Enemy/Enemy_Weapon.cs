using UnityEngine;
using System.Collections;

public class Enemy_Weapon : MonoBehaviour
{
    public Transform firePoint;
    [SerializeField] private int damage = 20;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask raycastLayers;

    public IEnumerator Shoot()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, firePoint.right, Mathf.Infinity, raycastLayers);

        if (hitInfo)
        {
            Debug.Log(hitInfo.transform.name);

            // Damage Infantry
            Infantry enemy = hitInfo.transform.GetComponent<Infantry>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Damage Wall
            Wall_Health wall = hitInfo.transform.GetComponent<Wall_Health>();
            if (wall != null)
            {
                wall.TakeDamage(damage);
            }

            // Set line renderer to the hit point
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, firePoint.position + firePoint.right * 100);
        }

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.02f);
        lineRenderer.enabled = false;
    }
}
