using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gm =
                FindFirstObjectByType<GameManager>();

            if (gm != null)
            {
                gm.CollectCoin();

                // سکه حذف نمی‌شود؛ فقط غیرفعال می‌شود
                gameObject.SetActive(false);
            }
        }
    }
}