using UnityEngine.UI;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public Transform healthBar;
    public Transform backgroundBar;


    private void OnEnable()
    {
        backgroundBar.transform.localScale = new Vector3(0, 0, 0);

    }


 


    public void UpdateEnnemyHealth(float normalizedHealth)
    {
        Vector3 scale = Vector3.one;


        if (healthBar != null)
        {
            scale.x = normalizedHealth;
            healthBar.transform.localScale = scale;
        }

        if (backgroundBar != null)
        {
            scale.x = 1 - normalizedHealth;
            backgroundBar.transform.localScale = scale;
        }


    }
}
