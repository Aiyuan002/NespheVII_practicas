public class HealthPotion : Item
{
    public int healthToRestore = 20;

    public override bool Use()
    {
        base.Use();

        UIController health = FindFirstObjectByType<UIController>();
        if (health != null && health.currentHealth >= health.maxHealth)
        {
            return false;
        }

        if (health != null)
        {
            health.RecoverHealth(healthToRestore);
        }
        return true;
    }
}
