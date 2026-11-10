using Lab6.Geometry;

namespace Lab6.Rendering;


public class CabinetProjection : IProjection
{

    public double Shortening { get; }

   
    public double AngleDegrees { get; }

    public CabinetProjection(double shortening = 0.5, double angleDegrees = 30)
    {
        Shortening = shortening;
        AngleDegrees = angleDegrees;
    }

    public string Name => "Кабинетная (по умолчанию)";

    public Matrix4 GetMatrix()
    {
        double a = AngleDegrees * Math.PI / 180;
        var m = Matrix4.Identity();
        m[2, 0] = -Shortening * Math.Cos(a);
        m[2, 1] = -Shortening * Math.Sin(a);
        return m;
    }

    public override string ToString() => Name;
}
