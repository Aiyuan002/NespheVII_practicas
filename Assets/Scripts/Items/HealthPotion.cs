public class HealthPotion : Item
{
    public int healthToRestore = 20;

    public override void Use()
    {
        base.Use();

        UIController health = FindFirstObjectByType<UIController>();
        health.RecoverHealth(healthToRestore);
        Destroy(gameObject); // Elimina el ítem después de usarlo
    }
}
