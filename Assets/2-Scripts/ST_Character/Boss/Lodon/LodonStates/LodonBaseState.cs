public class LodonBaseState : State<LodonBaseState>
{
    protected LodonBoss lodonBossCharacter;

    public LodonBaseState(LodonBoss lodonBossCharacter)
    {
        this.lodonBossCharacter = lodonBossCharacter;
    }

    virtual public void EndAnimation() { }
}