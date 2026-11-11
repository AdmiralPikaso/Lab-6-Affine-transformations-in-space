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
        (int first, int second) = axis switch
        {
            CoordinateAxis.X => (1, 2),
            CoordinateAxis.Y => (2, 0),
            CoordinateAxis.Z => (0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(axis))
        };

        double angle = angleDegrees * Math.PI / 180.0;
        double c = Math.Cos(angle);
        double s = Math.Sin(angle);
        Matrix4 matrix = Matrix4.Identity();
        matrix[first, first] = c;
        matrix[first, second] = s;
        matrix[second, first] = -s;
        matrix[second, second] = c;
        return matrix;
    }

    public static Matrix4 Reflection(CoordinatePlane plane) => plane switch
    {
        CoordinatePlane.XY => Scaling(1, 1, -1),
        CoordinatePlane.XZ => Scaling(1, -1, 1),
        CoordinatePlane.YZ => Scaling(-1, 1, 1),
        _ => throw new ArgumentOutOfRangeException(nameof(plane))
    };

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
