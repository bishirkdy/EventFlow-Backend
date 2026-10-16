namespace EventFlow.Operations.Infrastructure.Tests;

// Lets a test make the next N SaveChanges calls fail to simulate a database outage.
public sealed class FailureController
{
    public int RemainingFailures { get; set; }

    public bool ShouldFail()
    {
        if (RemainingFailures <= 0)
        {
            return false;
        }

        RemainingFailures--;
        return true;
    }
}
