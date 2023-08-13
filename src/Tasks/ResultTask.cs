namespace CmlLib.Core.Tasks;

public abstract class ResultTask : LinkedTask
{
    public ResultTask(string name) : base(name)
    {

    }

    public ResultTask(TaskFile file) : base(file)
    {

    }

    public LinkedTask? OnTrue { get; set; }
    public LinkedTask? OnFalse { get; set; }

    protected override async ValueTask<LinkedTask?> OnExecuted(
        IProgress<ByteProgressEventArgs>? progress,
        CancellationToken cancellationToken)
    {
        var result = await OnExecutedWithResult(progress, cancellationToken);
        var nextTask = result ? OnTrue : OnFalse;

        if (nextTask != null)
            return InsertNextTask(nextTask);
        else
            return NextTask;
    }

    protected abstract ValueTask<bool> OnExecutedWithResult(
        IProgress<ByteProgressEventArgs>? progress,
        CancellationToken cancellationToken); 
}