using Lab6.Geometry;
using Lab6.Rendering;
using Lab6.Transformations;

namespace Lab6.Advanced;

public sealed class AxonometricProjection : IProjection
{
    public double AngleXDegrees { get; }

    public double AngleYDegrees { get; }

    public AxonometricProjection(double angleXDegrees = 30, double angleYDegrees = 45)
    {
        if (!double.IsFinite(angleXDegrees) || !double.IsFinite(angleYDegrees))
            throw new ArgumentOutOfRangeException(nameof(angleXDegrees), "Углы должны быть конечными числами.");

        AngleXDegrees = angleXDegrees;
        AngleYDegrees = angleYDegrees;
    }

    public string Name => $"Аксонометрическая ({AngleXDegrees:0.##}°; {AngleYDegrees:0.##}°)";

    public Matrix4 GetMatrix()
    {
        Matrix4 orthographic = Matrix4.Identity();
        orthographic[2, 2] = 0;

        return AffineTransformations.Rotation(CoordinateAxis.X, AngleXDegrees)
             * AffineTransformations.Rotation(CoordinateAxis.Y, AngleYDegrees)
             * orthographic;
    }

    public override string ToString() => Name;
}

public sealed class PerspectiveProjection : IProjection
{
    public double Distance { get; }

    public PerspectiveProjection(double distance = 6)
    {
        if (!double.IsFinite(distance) || distance <= 0)
            throw new ArgumentOutOfRangeException(nameof(distance), "Расстояние до камеры должно быть положительным.");

        Distance = distance;
    }

    public string Name => $"Перспективная (d = {Distance:0.##})";

    public Matrix4 GetMatrix()
    {
        Matrix4 m = Matrix4.Identity();

        m[2, 3] = -1.0 / Distance;
        m[3, 3] = 1.0;
        return m;
    }

    public override string ToString() => Name;
}
