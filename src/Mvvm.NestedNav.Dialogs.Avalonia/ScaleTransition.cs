using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace NestedNav.Avalonia.Dialogs;

public class ScaleTransition : IPageTransition
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(300);
    public double StartX { get; set; } = 0;
    public double StartY { get; set; } = 0;
    public double TargetX { get; set; } = 1.0;
    public double TargetY { get; set; } = 1.0;
    public Easing Easing { get; set; } = new CubicEaseInOut();
    public bool Fade { get; set; } = true;

    public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
    {
        var tasks = new List<Task>();
        if (to != null)
        {
            to.IsVisible = false;
        }
        if (from != null)
        {
            var scale = forward
                ? new ScaleTransform(TargetX, TargetY)
                : new ScaleTransform(StartX, StartY);
            from.RenderTransform = scale;
            from.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            
            var animation = GetAnimation(GetKeyFrames(forward));
            tasks.Add(animation.RunAsync(from, cancellationToken));
        }
        if (to != null)
        {
            var scale = forward
                ? new ScaleTransform(StartX, StartY)
                : new ScaleTransform(TargetX, TargetY);
            to.RenderTransform = scale;
            to.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
            to.IsVisible = true;

            var animation = GetAnimation(GetKeyFrames(!forward));
            tasks.Add(animation.RunAsync(to, cancellationToken));
        }
        await Task.WhenAll(tasks);
    }

    private Animation GetAnimation(KeyFrames keyFrames)
    {
        var animation = new Animation
        {
            Duration = Duration,
            Easing = Easing,
            FillMode = FillMode.Forward
        };
        animation.Children.AddRange(keyFrames);
        return animation;
    }
    
    private KeyFrames GetKeyFrames(bool forward)
    {
        // KeyFrame 0
        var cue0Setters = new List<IAnimationSetter>
        {
            new Setter(ScaleTransform.ScaleXProperty, forward ? TargetX : StartX),
            new Setter(ScaleTransform.ScaleYProperty, forward ? TargetY : StartY),
        };
        if (Fade) 
            cue0Setters.Add(new Setter(Visual.OpacityProperty, forward ? 1.0 : 0.0));
        var keyFrame0 = new KeyFrame
        {
            Cue = new Cue(0)
        };
        keyFrame0.Setters.AddRange(cue0Setters);
        
        // KeyFrame 1
        var keyFrame1Setters = new List<IAnimationSetter>
        {
            new Setter(ScaleTransform.ScaleXProperty, forward ? StartX : TargetX),
            new Setter(ScaleTransform.ScaleYProperty, forward ? StartY : TargetY),
        };
        if (Fade) 
            keyFrame1Setters.Add(new Setter(Visual.OpacityProperty, forward ? 0.0 : 1.0));
        var keyFrame1 = new KeyFrame
        {
            Cue = new Cue(1)
        };
        keyFrame1.Setters.AddRange(keyFrame1Setters);
        
        return [keyFrame0, keyFrame1];
    }
}