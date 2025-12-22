using System.Collections;
using UnityEngine;

public interface IPlayable
{
    public IEnumerator Play();
}

public class Attack : IPlayable
{
    IBattleUnit _target;
    int _damage;
    //string effectKey;
    //string soundKey;

    public Attack(IBattleUnit target, int damage )
    {
        _target = target;
        _damage = damage;
    }

    public IEnumerator Play()
    {
        yield return new WaitForSeconds(0.1f);
        _target.TakeDamage( _damage);
    }
}

public class GetHp : IPlayable
{
    IBattleUnit _target;
    int _Hp;
    //string effectKey;
    //string soundKey;

    public GetHp(IBattleUnit target, int hp)
    {
        _target = target;
        _Hp = hp;
    }
    public IEnumerator Play()
    {
        yield return new WaitForSeconds(0.1f);
        _target.GetHp(_Hp);
    }
}

public class GetShield : IPlayable
{ 
    IBattleUnit _target;
    int _Shield;
    //string effectKey;
    //string soundKey;

    public GetShield(IBattleUnit target, int shield)
    {
        _target = target;
        _Shield = shield;
    }
    public IEnumerator Play()
    {
        yield return new WaitForSeconds(0.1f);
        _target.GetShield(_Shield);
    }
}

public class AddStatusEffect : IPlayable
{
    public IBattleUnit _target;
    string _effectKey;
    int _duration;
    int _stack;
    //string effectKey;
    //string soundKey;

    public AddStatusEffect(IBattleUnit target,  string effectKey, int duration, int stack)
    {
        _target = target;
        _effectKey = effectKey;
        _duration = duration;
        _stack = stack;
    }

    public IEnumerator Play()
    {
        yield return new WaitForSeconds(0.1f);
        _target.AddStatusEffect(_effectKey, _duration, _stack);
    }
}
