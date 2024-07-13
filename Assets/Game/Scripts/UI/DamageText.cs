using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private Image background;
    private Color color;

    public void ShowDamage(float damage)
    {
        damageText.text = "-" + damage.ToString();
        color = background.color;
        StartCoroutine(StartAnimation());
    }

    IEnumerator StartAnimation()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 initialPosition = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.5f, t);
            transform.position = Vector3.Lerp(initialPosition, initialPosition - Vector3.forward + new Vector3(0, 1.5f, 0), t);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(AnimateDamageText());
    }

    IEnumerator AnimateDamageText()
    {
        float duration = 2f;
        float elapsed = 0f;
        Vector3 initialPosition = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t);
            transform.position = Vector3.Lerp(initialPosition, initialPosition + new Vector3(0, 1f, 0), t);

            damageText.alpha = Mathf.Lerp(damageText.alpha, 0, t);
            color.a = Mathf.Lerp(color.a, 0, t);
            background.color = color;
            yield return null;
        }
        Destroy(gameObject);
    }
}
