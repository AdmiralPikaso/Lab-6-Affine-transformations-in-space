namespace Lab6.Geometry;


public class Face
{
    public List<int> Indices { get; }

    public Face(params int[] indices)
        : this((IEnumerable<int>)indices)
    {
    }

    public Face(IEnumerable<int> indices)
    {
        Indices = new List<int>(indices);
        if (Indices.Count < 3)
            throw new ArgumentException("У грани должно быть хотя бы 3 вершины", nameof(indices));
    }

    public int Count => Indices.Count;


    public Point3D Center(IList<Point3D> vertices)
    {
        double x = 0, y = 0, z = 0;
        foreach (int i in Indices)
        {
            x += vertices[i].X;
            y += vertices[i].Y;
            z += vertices[i].Z;
        }
        return new Point3D(x / Count, y / Count, z / Count);
    }

    public Point3D Normal(IList<Point3D> vertices)
    {
        double nx = 0, ny = 0, nz = 0;
        for (int k = 0; k < Count; k++)
        {
            Point3D a = vertices[Indices[k]];
            Point3D b = vertices[Indices[(k + 1) % Count]];
            nx += (a.Y - b.Y) * (a.Z + b.Z);
            ny += (a.Z - b.Z) * (a.X + b.X);
            nz += (a.X - b.X) * (a.Y + b.Y);
        }
        return new Point3D(nx, ny, nz).Normalized();
    }

   
    public void Reverse() => Indices.Reverse();

    public Face Clone() => new Face(Indices);

    public override string ToString() => "f " + string.Join(" ", Indices);
}
