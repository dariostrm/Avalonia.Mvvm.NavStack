

using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.Mvvm.NavStack;

public class ScaleTransition : IPageTransition
{
    private readonly Animation.Animation _fadeOutAnimation;
    private readonly Animation.Animation _fadeInAnimation;
    
    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public TimeSpan Duration
    {
        get => _fadeOutAnimation.Duration;
        set => _fadeOutAnimation.Duration = _fadeInAnimation.Duration = value;
    }

    /// <summary>
    /// Gets or sets element entrance easing.
    /// </summary>
    public Easing FadeInEasing
    {
        get => _fadeInAnimation.Easing;
        set => _fadeInAnimation.Easing = value;
    }

    /// <summary>
    /// Gets or sets element exit easing.
    /// </summary>
    public Easing FadeOutEasing
    {
        get => _fadeOutAnimation.Easing;
        set => _fadeOutAnimation.Easing = value;
    }
    
    public double StartScale { get; set; } = 0.8;
    public double EndScale { get; set; } = 1.0;

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossFade"/> class.
    /// </summary>
    public ScaleTransition()
        : this(TimeSpan.FromMilliseconds(200))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CrossFade"/> class.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    public ScaleTransition(TimeSpan duration)
    {
        _fadeOutAnimation = new Animation.Animation
        {
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame()
                {
                    Setters =
                    {
                        new Setter { Property = Visual.OpacityProperty, Value = 1d },
                        new Setter { Property = ScaleTransform.ScaleXProperty, Value = EndScale },
                        new Setter { Property = ScaleTransform.ScaleYProperty, Value = EndScale }
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame()
                {
                    Setters =
                    {
                        new Setter { Property = Visual.OpacityProperty, Value = 0d },
                        new Setter { Property = ScaleTransform.ScaleXProperty, Value = StartScale },
                        new Setter { Property = ScaleTransform.ScaleYProperty, Value = StartScale }
                    },
                    Cue = new Cue(1d)
                }

            }
        };
        _fadeInAnimation = new Animation.Animation
        {
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame()
                {
                    Setters =
                    {
                        new Setter { Property = Visual.OpacityProperty, Value = 0d },
                        new Setter { Property = ScaleTransform.ScaleXProperty, Value = StartScale },
                        new Setter { Property = ScaleTransform.ScaleYProperty, Value = StartScale }
                    },
                    Cue = new Cue(0d)
                },
                new KeyFrame()
                {
                    Setters =
                    {
                        new Setter { Property = Visual.OpacityProperty, Value = 1d },
                        new Setter { Property = ScaleTransform.ScaleXProperty, Value = EndScale },
                        new Setter { Property = ScaleTransform.ScaleYProperty, Value = EndScale }
                    },
                    Cue = new Cue(1d)
                }

            }
        };
        _fadeOutAnimation.Duration = _fadeInAnimation.Duration = duration;
        FadeInEasing = new CubicEaseInOut();
        FadeOutEasing = new CubicEaseInOut();
    }

    /// <inheritdoc cref="Start(Visual, Visual, CancellationToken)" />
    public async Task Start(Visual? from, Visual? to, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var tasks = new List<Task>();

        if (from != null)
        {
            tasks.Add(_fadeOutAnimation.RunAsync(from, cancellationToken));
        }

        if (to != null)
        {
            to.IsVisible = true;
            tasks.Add(_fadeInAnimation.RunAsync(to, cancellationToken));
        }

        await Task.WhenAll(tasks);

        if (from != null && !cancellationToken.IsCancellationRequested)
        {
            from.IsVisible = false;
            from.Opacity = 1;
            from.RenderTransform = new ScaleTransform(1, 1);
        }
    }


    Task IPageTransition.Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
    {
        return Start(from, to, cancellationToken);
    }
}