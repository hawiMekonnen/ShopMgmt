namespace ShopMgmt.Domain.Enums
{
    public enum MaterialStatus
    {
        Pending,        // just received, not yet inspected
        Serviceable,    // passed the 104 check – safe to use
        Quarantined,   // failed the check – isolated
        Condemned      // written‑off / destroyed
    }
}
