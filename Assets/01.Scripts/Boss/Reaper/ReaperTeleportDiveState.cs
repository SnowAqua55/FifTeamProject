public class ReaperTeleportDiveState : IBossState
{
    private Reaper boss;

    public void Enter(BossBase b)
    {
        boss = (Reaper)b;
        boss.StartCoroutine(boss.TeleportDiveRoutine());
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}