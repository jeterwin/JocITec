using UnityEngine;
using System.Collections.Generic;

public class CharacterFollower : MonoBehaviour
{
    public string AbilityName { get => abilityName; set => abilityName = value; }

    private Transform playerTransform;
    private List<Vector3> positionHistory = new List<Vector3>();

    [SerializeField] private float spacing = 0.15f;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private string abilityName;

    public void SetTarget(Transform target)
    {
        playerTransform = target;
        positionHistory.Add(playerTransform.position);
        spacing = spacing * (CharacterUnlockerManager.Instance.ActiveFollowers.Count + 1);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        if (Vector3.Distance(positionHistory[positionHistory.Count - 1], playerTransform.position) > 0.05f)
        {
            positionHistory.Add(playerTransform.position);
        }

        if (positionHistory.Count > 1)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > spacing)
            {
                transform.position = Vector3.MoveTowards(transform.position, positionHistory[0], moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, positionHistory[0]) < 0.05f)
                {
                    positionHistory.RemoveAt(0);
                }
            }
        }
    }
}