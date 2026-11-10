namespace Lab6.Geometry;

public enum PolyhedronKind
{
    Tetrahedron,
    Hexahedron,
    Octahedron,
    Icosahedron,
    Dodecahedron,
}


public static class PolyhedronFactory
{
    public static string DisplayName(PolyhedronKind kind) => kind switch
    {
        PolyhedronKind.Tetrahedron => "Тетраэдр",
        PolyhedronKind.Hexahedron => "Гексаэдр (куб)",
        PolyhedronKind.Octahedron => "Октаэдр",
        PolyhedronKind.Icosahedron => "Икосаэдр",
        PolyhedronKind.Dodecahedron => "Додекаэдр",
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    public static Polyhedron Create(PolyhedronKind kind) => kind switch
    {
        PolyhedronKind.Tetrahedron => Tetrahedron(),
        PolyhedronKind.Hexahedron => Hexahedron(),
        PolyhedronKind.Octahedron => Octahedron(),
        PolyhedronKind.Icosahedron => Icosahedron(),
        PolyhedronKind.Dodecahedron => Dodecahedron(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    
    public static Polyhedron Tetrahedron()
    {
        var v = new List<Point3D>
        {
            new(1, 1, 1),
            new(1, -1, -1),
            new(-1, 1, -1),
            new(-1, -1, 1),
        };
        var f = new List<Face>
        {
            new(0, 1, 2),
            new(0, 1, 3),
            new(0, 2, 3),
            new(1, 2, 3),
        };
        return Build(DisplayName(PolyhedronKind.Tetrahedron), v, f);
    }

    public static Polyhedron Hexahedron()
    {
   
        var v = new List<Point3D>();
        for (int i = 0; i < 8; i++)
            v.Add(new Point3D((i & 1) != 0 ? 1 : -1, (i & 2) != 0 ? 1 : -1, (i & 4) != 0 ? 1 : -1));

        var f = new List<Face>
        {
            new(0, 2, 6, 4),
            new(1, 3, 7, 5), 
            new(0, 1, 5, 4), 
            new(2, 3, 7, 6), 
            new(0, 1, 3, 2), 
            new(4, 5, 7, 6), 
        };
        return Build(DisplayName(PolyhedronKind.Hexahedron), v, f);
    }

    
    public static Polyhedron Octahedron()
    {
        var v = new List<Point3D>
        {
            new(1, 0, 0), new(-1, 0, 0), 
            new(0, 1, 0), new(0, -1, 0), 
            new(0, 0, 1), new(0, 0, -1), 
        };

        
        var f = new List<Face>();
        foreach (int x in new[] { 0, 1 })
            foreach (int y in new[] { 2, 3 })
                foreach (int z in new[] { 4, 5 })
                    f.Add(new Face(x, y, z));

        return Build(DisplayName(PolyhedronKind.Octahedron), v, f);
    }

    
    public static Polyhedron Icosahedron()
    {
        double phi = (1 + Math.Sqrt(5)) / 2;

        var v = new List<Point3D>();
        foreach (double a in new[] { -1.0, 1.0 })
        {
            foreach (double b in new[] { -phi, phi })
            {
                v.Add(new Point3D(0, a, b));
                v.Add(new Point3D(a, b, 0));
                v.Add(new Point3D(b, 0, a));
            }
        }

        
        bool IsEdge(int i, int j) => Math.Abs((v[i] - v[j]).Length - 2) < 1e-9;

        var f = new List<Face>();
        for (int i = 0; i < v.Count; i++)
            for (int j = i + 1; j < v.Count; j++)
                for (int k = j + 1; k < v.Count; k++)
                    if (IsEdge(i, j) && IsEdge(j, k) && IsEdge(i, k))
                        f.Add(new Face(i, j, k));

        return Build(DisplayName(PolyhedronKind.Icosahedron), v, f);
    }

   
    public static Polyhedron Dodecahedron()
    {
        Polyhedron ico = Icosahedron();
        List<Point3D> v = ico.Faces.Select(face => face.Center(ico.Vertices)).ToList();

        var f = new List<Face>();
        for (int vi = 0; vi < ico.Vertices.Count; vi++)
        {
           
            List<int> around = Enumerable.Range(0, ico.Faces.Count)
                                         .Where(fi => ico.Faces[fi].Indices.Contains(vi))
                                         .ToList();

           
            Point3D axis = ico.Vertices[vi].Normalized();
            Point3D first = v[around[0]];
            Point3D u = (first - axis * Point3D.Dot(first, axis)).Normalized();
            Point3D w = Point3D.Cross(axis, u);

            List<int> ordered = around
                .OrderBy(i => Math.Atan2(Point3D.Dot(v[i], w), Point3D.Dot(v[i], u)))
                .ToList();
            f.Add(new Face(ordered));
        }

        return Build(DisplayName(PolyhedronKind.Dodecahedron), v, f);
    }

    
    private static Polyhedron Build(string name, List<Point3D> vertices, List<Face> faces)
    {
        double radius = vertices.Max(p => p.Length);
        List<Point3D> normalized = vertices.Select(p => p / radius).ToList();

        var polyhedron = new Polyhedron(name, normalized, faces);
        polyhedron.OrientFacesOutward();
        return polyhedron;
    }
}
