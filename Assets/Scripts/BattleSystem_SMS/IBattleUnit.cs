public interface IBattleUnit
{
    public void TakeDamage(int damage);

    public void ApplyStatusEffect();

    public void GetHp(int hp);

    public void GetShield(int shield);

    public void AddStatusEffect(string effectKey, int duration, int stack);
}
