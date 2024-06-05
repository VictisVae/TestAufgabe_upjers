namespace CodeBase.Units {
  public class GroundUnit : UnitBase {
    public override void Recycle() => _unitFactory.Reclaim(this);
  }
}