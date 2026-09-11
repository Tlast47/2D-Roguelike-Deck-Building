using UnityEngine;

public class CharacterData : MonoBehaviour
{

    /////Fields/////

    [SerializeField] protected int maxHP = 80;
    [SerializeField] protected int currentHP = 80;
    [SerializeField] protected int block = 0;

    /////Combat Methods/////

    public void TakeDamage(int damage)
    {
        // Guard Clause
        if (damage <= 0)
        {
            return;
        }

        // ถ้ามี Block
        if (block > 0)
        {
            // Block มากกว่าหรือเท่ากับ Damage
            if (block >= damage)
            {
                block -= damage;
                damage = 0;
            }
            else
            {
                // Block ไม่พอ
                damage -= block;
                block = 0;
            }
        }

        // หัก HP
        currentHP -= damage;

        // HP ห้ามติดลบ
        if (currentHP < 0)
        {
            currentHP = 0;
        }
    }

    public void Heal(int amount)
    {
        // Guard Clause
        if (amount <= 0)
        {
            return;
        }

        currentHP += amount;

        // Heal เกิน MaxHP ไม่ได้ (การ clamp)
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void AddBlock(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        block += amount;
    }

    public void ResetBlock()
    {
        block = 0;
    }

    /////Getter/////

    // Read-only access for UI
    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetBlock()
    {
        return block;
    }

    /////State/////

    // ตรวจสอบว่าตัวละครตายแล้วหรือยัง
    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
