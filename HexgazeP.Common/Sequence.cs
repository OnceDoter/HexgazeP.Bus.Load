using HexgazeP.RabbitMQMessageGenerator;

namespace HexgazeP.Common;

public class Sequence
{
    private long _currentValue;

    public Sequence(long initialValue = 0)
    {
        _currentValue = initialValue;
    }

    public long CurrentValue => _currentValue;

    public long NextValue()
    {
        return Interlocked.Add(ref _currentValue, int.Parse(Environment.GetEnvironmentVariable(EnvVars.BatchSize) ?? "5000"));
    }
}