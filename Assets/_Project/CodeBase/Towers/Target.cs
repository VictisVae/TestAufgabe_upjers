using CodeBase.Infrastructure.Services.Player;
using UnityEngine;

namespace CodeBase.Towers {
  public class Target : MonoBehaviour {
    private Health _health;
    public void Construct(int health) => _health = new Health(health);
    public void GetHit(int damage) => _health.TakeDamage(damage);
    public Vector3 Position => transform.localPosition;
    public bool IsAlive => _health.IsAlive;
  }
}