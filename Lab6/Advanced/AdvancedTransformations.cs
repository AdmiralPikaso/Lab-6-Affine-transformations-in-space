using Lab6.Geometry;
using Lab6.Transformations;

namespace Lab6.Advanced;

public static class AdvancedTransformations
{
    public static Matrix4 RotationAroundCenterParallelToAxis(
        Polyhedron model, CoordinateAxis axis, double angleDegrees)
    {
        ArgumentNullException.ThrowIfNull(model);

        Point3D center = model.Center();
        return AroundPoint(center, AffineTransformations.Rotation(axis, angleDegrees));
    }

    public static Matrix4 RotationAroundLine(Point3D a, Point3D b, double angleDegrees)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        RequireFinite(angleDegrees);
        RequireFinite(a.X, a.Y, a.Z);
        RequireFinite(b.X, b.Y, b.Z);

        Point3D direction = b - a;
        if (direction.Length < 1e-9)
            throw new ArgumentException("Точки, задающие прямую, не должны совпадать.");

        Matrix4 rotation = RotationAroundDirection(direction.Normalized(), angleDegrees);

        Point3D rotatedA = rotation.Transform(a);
        rotation[3, 0] = a.X - rotatedA.X;
        rotation[3, 1] = a.Y - rotatedA.Y;
        rotation[3, 2] = a.Z - rotatedA.Z;
        return rotation;
    }

    public static Matrix4 RotationAroundDirection(Point3D direction, double angleDegrees)
    {
        ArgumentNullException.ThrowIfNull(direction);
        RequireFinite(angleDegrees);
        RequireFinite(direction.X, direction.Y, direction.Z);

        Point3D axis = direction.Normalized();
        if (axis.Length < 1e-9)
            throw new ArgumentException("Направление оси поворота не должно быть нулевым.");

        double angle = angleDegrees * Math.PI / 180.0;
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);
        double t = 1.0 - c;

        double x = axis.X;
        double y = axis.Y;
        double z = axis.Z;

        Matrix4 m = Matrix4.Identity();
        m[0, 0] = t * x * x + c;
        m[0, 1] = t * x * y + s * z;
        m[0, 2] = t * x * z - s * y;

        m[1, 0] = t * x * y - s * z;
        m[1, 1] = t * y * y + c;
        m[1, 2] = t * y * z + s * x;

        m[2, 0] = t * x * z + s * y;
        m[2, 1] = t * y * z - s * x;
        m[2, 2] = t * z * z + c;

        return m;
    }

    private static Matrix4 AroundPoint(Point3D pivot, Matrix4 linear)
    {
        Point3D moved = linear.Transform(pivot);
        Matrix4 result = linear;
        result[3, 0] = pivot.X - moved.X;
        result[3, 1] = pivot.Y - moved.Y;
        result[3, 2] = pivot.Z - moved.Z;
        return result;
    }

    private static void RequireFinite(params double[] values)
    {
        if (values.Any(value => !double.IsFinite(value)))
            throw new ArgumentOutOfRangeException(nameof(values), "Параметры должны быть конечными числами.");
    }
}
