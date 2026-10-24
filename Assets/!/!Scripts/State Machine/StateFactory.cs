public class StateFactory
{
    protected StateMachine _context;

    public StateFactory(StateMachine currentContext)
    {
        _context = currentContext;
    }

    public BaseState MainMenu()
    {
        return new MainMenuState(_context, this);
    }

    public BaseState Level()
    {
        return new LevelState(_context, this);
    }

    public BaseState Playing()
    {
        return new PlayingState(_context, this);
    }

    public BaseState Paused()
    {
        return new PausedState(_context, this);
    }

    public BaseState Upgrade()
    {
        return new UpgradeState(_context, this);
    }

    public BaseState GameOver()
    {
        return new GameOverState(_context, this);
    }

}
