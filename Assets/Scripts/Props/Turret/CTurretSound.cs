public class CTurretSound : CComponent
{
    private System.Random rand;

    public override void Awake()
    {
        base.Awake();
        rand = new System.Random();
    }

    public void PlayTurretActiveVoiceSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.turretActive, this.transform.position);
    }

    public void PlayTurretDIsableVoiceSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.turretDisabled, this.transform.position);
    }

    public void PlayTurretSearchingVoiceSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.turretSearch, this.transform.position);
    }

    public void PlayTurretSearchingRetireVoiceSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.turretRetire, this.transform.position);
    }

    public void PlayTurretPickUpVoiceSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.turretPickup, this.transform.position);
    }

    public void PlayTurretFallDownVoiceSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.turretTipped, this.transform.position);
    }

    public void PlayTurretGunSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.shoot, this.transform.position);
    }

    public void PlayTurretGunRotationSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.active, this.transform.position);
    }

    public void PlayTurretPingSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.ping, this.transform.position);
    }
    public void PlayTurretDeploySound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.deploy, this.transform.position);
    }
    public void PlayTurretRetractSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.retract, this.transform.position);
    }

    public void PlayTurretDieSound()
    {
        CAudioManager.Instance.PlayOneShot(CFMODEventsTurret.Instance.die, this.transform.position);
    }
}
