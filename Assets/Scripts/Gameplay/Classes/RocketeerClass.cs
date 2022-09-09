

public class RocketeerClass : BaseClass
{
    public override void UpgradeRateOfFire(float value)
    {
        base.UpgradeRateOfFire(value);

        ReloadTime = value;
    }
}
