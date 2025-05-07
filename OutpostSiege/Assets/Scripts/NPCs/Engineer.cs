using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class Engineer : MonoBehaviour
{
    private Vector3 basePosition;
    private bool isHandlingQueue = false;

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
    }

    public void CutTree(GameObject tree, Action<GameObject> onTreeCut)
    {
        if (!TreeAlreadyQueued(tree))
        {
            treeQueue.Enqueue((tree, onTreeCut));
            Debug.Log($"? Copac ad?ugat: {tree.name}");

            // Afi??m coada actual?
            string queueContents = "?? Coada curent?:";
            foreach (var item in treeQueue)
            {
                queueContents += $" {item.tree.name}";
            }
            Debug.Log(queueContents);
        }
        else
        {
            Debug.Log($"?? Copacul {tree.name} este deja în coad?!");
        }

        if (!isHandlingQueue)
        {
            Debug.Log("?? Pornim procesarea cozii de copaci...");
            StartCoroutine(HandleQueue());
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
        isHandlingQueue = true;

        while (treeQueue.Count > 0)
        {
            var (tree, callback) = treeQueue.Peek(); // Ob?inem primul element

            if (tree == null)
            {
                treeQueue.Dequeue(); // Dac? copacul a fost distrus între timp
                continue;
            }

            // Mergem la copac
            yield return StartCoroutine(MoveToPosition(tree.transform.position));
            animator.SetBool("running", false);
            animator.SetBool("engineering", true);

            // Simul?m t?ierea
            yield return new WaitForSeconds(5f);
            animator.SetBool("engineering", false);

            // Distrugem copacul ?i anun??m callback-ul
            Destroy(tree);
            callback?.Invoke(tree);

            treeQueue.Dequeue();
            yield return new WaitForSeconds(0.2f); // Pauz? mic? între t?ieri
        }

        // Întoarcerea spre baz?, cu verificare pe drum
        Vector3 baseTarget = new Vector3(basePosition.x, transform.position.y, basePosition.z);
        animator.SetBool("running", true);
        FlipSprite(baseTarget.x);

        while (Vector3.Distance(transform.position, baseTarget) > stopDistance)
        {
            // Dac? apare un copac nou în timp ce se întoarce, ne întoarcem din drum
            if (treeQueue.Count > 0)
            {
                Debug.Log("?? Copac nou detectat în drum spre baz?! Ne întoarcem!");
                StartCoroutine(HandleQueue());
                yield break;
            }

            transform.position = Vector3.MoveTowards(transform.position, baseTarget, Time.deltaTime * moveSpeed);
            yield return null;
        }

        animator.SetBool("running", false);
        isHandlingQueue = false;
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

    public bool IsBusy() => isHandlingQueue;
}
