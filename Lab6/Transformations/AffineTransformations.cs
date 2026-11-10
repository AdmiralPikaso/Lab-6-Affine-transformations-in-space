using Lab6.Geometry;

namespace Lab6.Transformations;

public enum CoordinateAxis { X, Y, Z }
public enum CoordinatePlane { XY, XZ, YZ }

public static class AffineTransformations
{
    public static Matrix4 Translation(double dx, double dy, double dz)
    {
        RequireFinite(dx, dy, dz);
        Matrix4 matrix = Matrix4.Identity();
        matrix[3, 0] = dx;
        matrix[3, 1] = dy;
        matrix[3, 2] = dz;
        return matrix;
    }

    public static Matrix4 Scaling(double sx, double sy, double sz)
    {
        RequireFinite(sx, sy, sz);
        Matrix4 matrix = Matrix4.Identity();
        matrix[0, 0] = sx;
        matrix[1, 1] = sy;
        matrix[2, 2] = sz;
        return matrix;
    }

    public static Matrix4 Rotation(CoordinateAxis axis, double angleDegrees)
    {
        RequireFinite(angleDegrees);
        double angle = angleDegrees * Math.PI / 180.0;
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);
        Matrix4 matrix = Matrix4.Identity();

        switch (axis)
        {
            case CoordinateAxis.X:
                matrix[1, 1] = c;
                matrix[1, 2] = s;
                matrix[2, 1] = -s;
                matrix[2, 2] = c;
                break;
            case CoordinateAxis.Y:
                matrix[0, 0] = c;
                matrix[0, 2] = -s;
                matrix[2, 0] = s;
                matrix[2, 2] = c;
                break;
            case CoordinateAxis.Z:
                matrix[0, 0] = c;
                matrix[0, 1] = s;
                matrix[1, 0] = -s;
                matrix[1, 1] = c;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis));
        }
        return matrix;
    }

    public static Matrix4 Reflection(CoordinatePlane plane)
    {
        Matrix4 matrix = Matrix4.Identity();
        switch (plane)
        {
            case CoordinatePlane.XY: matrix[2, 2] = -1; break; // z -> -z
            case CoordinatePlane.XZ: matrix[1, 1] = -1; break; // y -> -y
            case CoordinatePlane.YZ: matrix[0, 0] = -1; break; // x -> -x
            default: throw new ArgumentOutOfRangeException(nameof(plane));
        }
        return matrix;
    }
    
    public static Matrix4 ScalingAroundCenter(Polyhedron model, double sx, double sy, double sz)
    {
        ArgumentNullException.ThrowIfNull(model);
        Point3D center = model.Center();
        return Translation(-center.X, -center.Y, -center.Z)
               * Scaling(sx, sy, sz)
               * Translation(center.X, center.Y, center.Z);
    }

    public static void Apply(Scene scene, Matrix4 matrix)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(matrix);

        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                if (!double.IsFinite(matrix[r, c]))
                    throw new ArgumentException("Матрица содержит недопустимые значения.", nameof(matrix));

        foreach (Point3D vertex in scene.Model.Vertices)
            if (!matrix.Transform(vertex).IsFinite)
                throw new ArgumentException("Преобразование даёт некорректные координаты.", nameof(matrix));

        scene.Model.Apply(matrix);
        if (LinearDeterminant(matrix) < 0)
            foreach (Face face in scene.Model.Faces)
                face.Reverse();

        scene.Refresh();
    }

    private static double LinearDeterminant(Matrix4 m)
    {
        return m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1])
             - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
             + m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
    }

    private static void RequireFinite(params double[] parameters)
    {
        if (parameters.Any(value => !double.IsFinite(value)))
            throw new ArgumentOutOfRangeException(nameof(parameters), "Параметры должны быть конечными числами.");
    }
}
