

public class RiflemanClass : BaseClass
{
    public override void UpgradeRateOfFire(float value)
    {
        base.UpgradeRateOfFire(value);

        RateOfFire = value;
    }
}
