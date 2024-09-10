using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BounceObject : MonoBehaviour
{
    [FoldoutGroup("Bounce Object")][SerializeField] private float bounceMultiplier;
    [FoldoutGroup("Bounce Object")] private float fallVelocity; // The velocity of the player when hitting the trampoline

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                Vector3 playerVelocity = playerRb.velocity;

                // Cek apakah player datang dari atas (jika velocity.y negatif)
                if (playerVelocity.y < 0)
                {
                    // Pemain datang dari atas, hitung gaya pantul
                    fallVelocity = Mathf.Abs(playerVelocity.y);
                    float bounceForce = fallVelocity * bounceMultiplier;
                    playerRb.velocity = new Vector3(playerVelocity.x, bounceForce, playerVelocity.z);
                }
                else
                {
                    // Pemain datang dari samping atau dari bawah, tidak ada pantulan
                    Debug.Log("Pemain datang dari samping atau tidak jatuh");
                }
            }
        }
    }
}
