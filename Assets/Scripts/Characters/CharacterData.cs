using UnityEngine;

public class CharacterData : MonoBehaviour
{

    /////Fields/////

    [SerializeField] protected int maxHP = 80;
    [SerializeField] protected int currentHP = 80;
    [SerializeField] protected int block = 0;

    // Combat-only stacks; never stored in prefabs or run data.
    private int burnStacks;
    private int poisonStacks;

    public int BurnStacks => burnStacks;
    public int PoisonStacks => poisonStacks;

    public void AddBurn(int amount)
    {
        if (amount <= 0 || IsDead()) return;
        burnStacks = (int)System.Math.Min((long)burnStacks + amount, int.MaxValue);
    }

    public void AddPoison(int amount)
    {
        if (amount <= 0 || IsDead()) return;
        poisonStacks = (int)System.Math.Min((long)poisonStacks + amount, int.MaxValue);
    }

    // Poison triggers at the start of the owner's turn and bypasses Block.
    public void ProcessPoison()
    {
        if (IsDead() || poisonStacks <= 0) return;
        int damage = poisonStacks;
        poisonStacks--;
        currentHP = Mathf.Max(0, currentHP - damage);
        Debug.Log(name + " Poison damage: " + damage);
    }

    // Burn triggers at the end of the owner's turn and can be blocked.
    public void ProcessBurn()
    {
        if (IsDead() || burnStacks <= 0) return;
        int damage = burnStacks;
        burnStacks--;
        TakeDamage(damage);
        Debug.Log(name + " Burn damage: " + damage);
    }

    public void ClearStatusEffects()
    {
        burnStacks = 0;
        poisonStacks = 0;
    }

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

    public int GetCurrentHP()
    {
        return currentHP;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public int GetBlock()
    {
        return block;
    }

    /////Setter/////
 
    public void SetMaxHP(int hp)
    {
        if (hp <= 0)
        {
            return;
        }

        maxHP = hp;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    public void SetCurrentHP(int hp)
    {
        currentHP = Mathf.Clamp(hp, 0, maxHP);
    }

    /////State/////

    // ตรวจสอบว่าตัวละครตายแล้วหรือยัง
    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
