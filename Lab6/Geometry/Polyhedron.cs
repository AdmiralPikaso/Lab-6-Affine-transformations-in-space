namespace Lab6.Geometry;

public class Polyhedron
{
    public string Name { get; set; }

    public List<Point3D> Vertices { get; }

    public List<Face> Faces { get; }

    public Polyhedron(string name, IEnumerable<Point3D> vertices, IEnumerable<Face> faces)
    {
        Name = name;
        Vertices = new List<Point3D>(vertices);
        Faces = new List<Face>(faces);

        foreach (Face face in Faces)
        {
            foreach (int i in face.Indices)
            {
                if (i < 0 || i >= Vertices.Count)
                    throw new ArgumentException($"Грань ссылается на несуществующую вершину {i}");
            }
        }
    }

    
    public Point3D Center()
    {
        if (Vertices.Count == 0)
            return new Point3D(0, 0, 0);

        double x = 0, y = 0, z = 0;
        foreach (Point3D v in Vertices)
        {
            x += v.X;
            y += v.Y;
            z += v.Z;
        }
        int n = Vertices.Count;
        return new Point3D(x / n, y / n, z / n);
    }

   
    public void Apply(Matrix4 matrix)
    {
        for (int i = 0; i < Vertices.Count; i++)
            Vertices[i] = matrix.Transform(Vertices[i]);
    }

    
    public List<(int A, int B)> Edges()
    {
        var seen = new HashSet<(int, int)>();
        var edges = new List<(int A, int B)>();
        foreach (Face face in Faces)
        {
            for (int k = 0; k < face.Count; k++)
            {
                int a = face.Indices[k];
                int b = face.Indices[(k + 1) % face.Count];
                var key = a < b ? (a, b) : (b, a);
                if (seen.Add(key))
                    edges.Add(key);
            }
        }
        return edges;
    }

    
    public void OrientFacesOutward()
    {
        Point3D center = Center();
        foreach (Face face in Faces)
        {
            Point3D normal = face.Normal(Vertices);
            Point3D outward = face.Center(Vertices) - center;
            if (Point3D.Dot(normal, outward) < 0)
                face.Reverse();
        }
    }

    
    public Polyhedron Clone() =>
        new Polyhedron(Name, Vertices.Select(v => v.Clone()), Faces.Select(f => f.Clone()));

    public override string ToString() =>
        $"{Name}: вершин {Vertices.Count}, граней {Faces.Count}";
}
