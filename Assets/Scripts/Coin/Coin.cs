using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Collect Sound")]

    [SerializeField]
    private AudioClip collectSound;

    [Range(0f, 1f)]
    [SerializeField]
    private float volume = 1f;


    // =====================================================
    // GAME MANAGER
    // =====================================================

    private GameManager gameManager;

    private bool collected = false;


    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        gameManager =
            FindFirstObjectByType<GameManager>();
    }


    // =====================================================
    // COLLECT COIN
    // =====================================================

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (gameManager == null)
        {
            Debug.LogWarning(
                "Coin could not find GameManager."
            );

            return;
        }


        // Prevent double collection
        collected = true;


        // =================================================
        // REGISTER COIN
        // =================================================

        gameManager.CollectCoin();


        // =================================================
        // PLAY COLLECT SOUND
        // =================================================

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(
                collectSound,
                transform.position,
                volume
            );
        }


        // =================================================
        // DESTROY COIN
        // =================================================

        Destroy(gameObject);
    }
}