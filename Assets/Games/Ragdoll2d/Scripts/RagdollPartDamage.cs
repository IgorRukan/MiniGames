using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollPartDamage : MonoBehaviour
{
    public string bodyPartName; // Название части тела (например, "Head", "Arm")
    public float damageMultiplier = 1.0f; // Множитель урона для разных частей тела
    public float force;
    private Rigidbody2D rb; // Rigidbody части тела

    private void Start()
    {
        force = damageMultiplier;
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, столкнулись ли с землёй или другим объектом
        if (collision.relativeVelocity.magnitude > 20f) // Минимальная сила удара
        {
            float impactForce = collision.relativeVelocity.magnitude;

            // Вычисляем урон
            float damage = impactForce * damageMultiplier;

            rb.AddForce(Vector2.down * force * Time.deltaTime);
            
            // Сообщаем корневому объекту (персонажу) о нанесённом уроне
            //SendMessageUpwards("ApplyDamage", new DamageInfo(damage, bodyPartName, collision.contacts[0].normal), SendMessageOptions.DontRequireReceiver);

            Debug.Log($"{bodyPartName} получил удар силой {impactForce}, нанесён урон {damage}");
        }
    }
}
