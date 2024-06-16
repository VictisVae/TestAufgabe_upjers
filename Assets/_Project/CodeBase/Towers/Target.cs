using System;
using CodeBase.Infrastructure.Services.Player;
using UnityEngine;

namespace CodeBase.Towers {
  public class Target : MonoBehaviour {
    public event Action NoHealthEvent = delegate {  };
    
    private Health _health;
    public void Construct(int health) => _health = new Health(health);
    public void GetHit(int damage) {
      _health.TakeDamage(damage);

      if (NoHealth) {
        NoHealthEvent();
      }
    }

    public Vector3 Position => transform.localPosition;
    public bool NoHealth => _health.IsAlive == false;
  }
}