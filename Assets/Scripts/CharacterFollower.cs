using UnityEngine;
using System.Collections.Generic;

public class CharacterFollower : MonoBehaviour
{
    public string AbilityName { get => abilityName; set => abilityName = value; }

    [SerializeField] private float spacing = 0.15f;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private string abilityName;

    private Transform playerTransform;
    private Animator playerAnimator;
    private Animator myAnimator;
    private List<Snapshot> history = new List<Snapshot>();

    private struct Snapshot
    {
        public Vector3 position;
        public Vector3 scale;
        public int animHash;
        public float animTime;

        public Snapshot(Vector3 pos, Vector3 scl, int hash, float time)
        {
            position = pos;
            scale = scl;
            animHash = hash;
            animTime = time;
        }
    }

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void SetTarget(Transform target)
    {
        playerTransform = target;
        playerAnimator = target.GetComponent<Animator>();
        
        spacing = spacing * (CharacterUnlockerManager.Instance.ActiveFollowers.Count + 1);
        
        RecordFrame();
    }

    private void Update()
    {
        if (playerTransform == null || playerAnimator == null) return;

        RecordFrame();

        if (history.Count > 1)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > spacing)
            {
                Snapshot targetFrame = history[0];
                
                transform.position = Vector3.MoveTowards(transform.position, targetFrame.position, moveSpeed * Time.deltaTime);
                transform.localScale = targetFrame.scale;

                if (myAnimator != null)
                {
                    myAnimator.Play(targetFrame.animHash, 0, targetFrame.animTime);
                }

                if (Vector3.Distance(transform.position, targetFrame.position) < 0.05f)
                {
                    history.RemoveAt(0);
                }
            }
        }
    }

    private void RecordFrame()
    {
        var state = playerAnimator.GetCurrentAnimatorStateInfo(0);
        
        Snapshot currentFrame = new Snapshot(
            playerTransform.position,
            playerTransform.localScale,
            state.fullPathHash,
            state.normalizedTime
        );

        if (history.Count == 0 || Vector3.Distance(history[history.Count - 1].position, currentFrame.position) > 0.05f)
        {
            history.Add(currentFrame);
        }
    }
}