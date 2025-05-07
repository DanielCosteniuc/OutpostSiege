using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class Engineer : MonoBehaviour
{
    private Vector3 basePosition;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 0.05f;

    private Queue<(GameObject tree, Action<GameObject> callback)> treeQueue = new();

    private void Start()
    {
        basePosition = transform.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(HandleQueue()); // Rulează mereu în fundal
    }

    public void CutTree(GameObject tree, Action<GameObject> onTreeCut)
    {
        if (!TreeAlreadyQueued(tree))
        {
            treeQueue.Enqueue((tree, onTreeCut));
            Debug.Log($"🌲 Copac adăugat în coadă: {tree.name}");
        }
        else
        {
            Debug.Log($"⚠️ Copacul {tree.name} este deja în coadă!");
        }
    }

    private bool TreeAlreadyQueued(GameObject tree)
    {
        foreach (var item in treeQueue)
        {
            if (item.tree == tree)
                return true;
        }
        return false;
    }

    private IEnumerator HandleQueue()
    {
        while (true)
        {
            if (treeQueue.Count == 0)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            // Procesăm toți copacii unul după altul
            while (treeQueue.Count > 0)
            {
                var (tree, callback) = treeQueue.Dequeue();

                if (tree == null) continue;

                yield return MoveToPosition(tree.transform.position);

                animator.SetBool("running", false);
                animator.SetBool("engineering", true);

                yield return new WaitForSeconds(5f); // tăiere

                animator.SetBool("engineering", false);
                Destroy(tree);
                callback?.Invoke(tree);
            }

            // Toți copacii au fost procesați – acum se întoarce la bază
            yield return MoveToPosition(basePosition);
            animator.SetBool("running", false);
        }
    }


    private IEnumerator MoveToPosition(Vector3 destination)
    {
        Vector3 target = new Vector3(destination.x, transform.position.y, destination.z);

        animator.SetBool("running", true);
        FlipSprite(target.x);

        while (Vector3.Distance(transform.position, target) > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * moveSpeed);
            yield return null;
        }
    }

    private void FlipSprite(float targetX)
    {
        spriteRenderer.flipX = targetX < transform.position.x;
    }

    public bool IsBusy() => treeQueue.Count > 0;
}
