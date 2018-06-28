public interface IDamageable
{
    Alignement GetAlignement();
    void TakeDamage(float amount);
    void ModifySpeed(float pct);
    void Heal(float amount);
    void TurnOnOffEffects(string effect, bool stateToTurn);
    void ChangeRes(float newRes, string modify);
}
