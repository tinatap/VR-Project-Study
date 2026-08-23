using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Collect Sound")]
    public AudioClip collectSound;

    [Range(0f, 1f)]
    public float volume = 1f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gm =
                FindFirstObjectByType<GameManager>();

            if (gm != null)
            {
                gm.CollectCoin();

                // =========================================
                // PLAY COLLECT SOUND
                // =========================================

                if (collectSound != null)
                {
                    AudioSource.PlayClipAtPoint(
                        collectSound,
                        transform.position,
                        volume
                    );
                }


                // =========================================
                // DISABLE COIN
                // =========================================

                gameObject.SetActive(false);
            }
        }
    }
}