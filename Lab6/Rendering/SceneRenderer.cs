using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Lab6.Geometry;

namespace Lab6.Rendering;


public static class SceneRenderer
{
    
    public const double AxisLength = 2.2;

  
    private const double UnitsPerMinSide = 5.0;

    private const float MaxScreenCoordinate = 1e6f;

    private static readonly Color BackgroundColor = Color.White;
    private static readonly Color EdgeColor = Color.FromArgb(35, 35, 35);
    private static readonly Color VertexColor = Color.FromArgb(200, 40, 40);
    private static readonly Color HiddenEdgeColor = Color.FromArgb(165, 165, 170);

    private static readonly (Point3D Direction, Color Color, string Label)[] Axes =
    {
        (new Point3D(1, 0, 0), Color.FromArgb(215, 45, 45), "X"),
        (new Point3D(0, 1, 0), Color.FromArgb(30, 150, 55), "Y"),
        (new Point3D(0, 0, 1), Color.FromArgb(40, 95, 225), "Z"),
    };

   
    public static double PixelsPerUnit(Rectangle area) => Math.Min(area.Width, area.Height) / UnitsPerMinSide;

    public static void Draw(Graphics g, Rectangle area, Scene scene)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.Clear(BackgroundColor);
        if (area.Width <= 0 || area.Height <= 0)
            return;

        Matrix4 projection = scene.Projection.GetMatrix();
        double scale = PixelsPerUnit(area);
        double cx = area.Left + area.Width / 2.0;
        double cy = area.Top + area.Height / 2.0;

       
        PointF? ToScreen(Point3D p)
        {
            Point3D q = projection.Transform(p);
            if (!q.IsFinite)
                return null;

            float x = (float)(cx + q.X * scale);
            float y = (float)(cy - q.Y * scale);
            if (Math.Abs(x) > MaxScreenCoordinate || Math.Abs(y) > MaxScreenCoordinate)
                return null;
            return new PointF(x, y);
        }

        
        if (scene.ShowAxes)
            DrawAxisLines(g, ToScreen, scale);

        DrawPolyhedron(g, scene.Model, ToScreen, scene.DashHiddenEdges);

        if (scene.ShowAxes)
            DrawAxisLabels(g, ToScreen, scale);

        using var font = new Font("Segoe UI", 9f);
        using var brush = new SolidBrush(Color.DimGray);
        g.DrawString($"{scene.Model.Name}  •  {scene.Projection.Name}", font, brush, area.Left + 8, area.Bottom - 24);
    }

    private static void DrawPolyhedron(Graphics g, Polyhedron polyhedron, Func<Point3D, PointF?> toScreen, bool dashHidden)
    {
        PointF?[] screen = polyhedron.Vertices.Select(toScreen).ToArray();

    
        var visibleEdges = new HashSet<(int, int)>();
        foreach (Face face in polyhedron.Faces)
        {
            if (dashHidden && !IsFrontFacing(face, screen))
                continue;
            for (int k = 0; k < face.Count; k++)
            {
                int a = face.Indices[k], b = face.Indices[(k + 1) % face.Count];
                visibleEdges.Add(a < b ? (a, b) : (b, a));
            }
        }

        var visibleVertices = new HashSet<int>();
        using (var hiddenPen = new Pen(HiddenEdgeColor, 1.3f) { DashStyle = DashStyle.Dash })
        using (var pen = new Pen(EdgeColor, 2f) { LineJoin = LineJoin.Round })
        {
            List<(int A, int B)> edges = polyhedron.Edges();

            
            foreach (var (a, b) in edges)
            {
                if (!visibleEdges.Contains((a, b)) && screen[a] is PointF pa && screen[b] is PointF pb)
                    g.DrawLine(hiddenPen, pa, pb);
            }
            foreach (var (a, b) in edges)
            {
                if (visibleEdges.Contains((a, b)) && screen[a] is PointF pa && screen[b] is PointF pb)
                {
                    g.DrawLine(pen, pa, pb);
                    visibleVertices.Add(a);
                    visibleVertices.Add(b);
                }
            }
        }

        using var brush = new SolidBrush(VertexColor);
        using var hiddenBrush = new SolidBrush(HiddenEdgeColor);
        for (int i = 0; i < screen.Length; i++)
        {
            if (screen[i] is not PointF p)
                continue;
            if (visibleVertices.Contains(i))
                g.FillEllipse(brush, p.X - 3, p.Y - 3, 6, 6);
            else
                g.FillEllipse(hiddenBrush, p.X - 2, p.Y - 2, 4, 4);
        }
    }


    private static bool IsFrontFacing(Face face, PointF?[] screen)
    {
        double doubledArea = 0;
        for (int k = 0; k < face.Count; k++)
        {
            if (screen[face.Indices[k]] is not PointF p || screen[face.Indices[(k + 1) % face.Count]] is not PointF q)
                return true; 
            doubledArea += (double)p.X * q.Y - (double)q.X * p.Y;
        }
        return doubledArea <= 1e-6; 
    }

   

    private static void DrawAxisLines(Graphics g, Func<Point3D, PointF?> toScreen, double scale)
    {
        if (toScreen(new Point3D(0, 0, 0)) is not PointF origin)
            return;

        foreach (var (direction, color, _) in Axes)
        {
            double length = AxisWorldLength(direction, origin, toScreen, scale);

            
            if (toScreen(direction * -length) is PointF negative)
            {
                using var dashed = new Pen(Color.FromArgb(140, color), 1.5f) { DashStyle = DashStyle.Dash };
                g.DrawLine(dashed, origin, negative);
            }

      
            if (toScreen(direction * length) is PointF positive)
            {
                using var pen = new Pen(color, 2.5f) { CustomEndCap = new AdjustableArrowCap(4, 5) };
                g.DrawLine(pen, origin, positive);

                float dx = positive.X - origin.X, dy = positive.Y - origin.Y;
                float len = MathF.Sqrt(dx * dx + dy * dy);
                if (len < 1)
                    continue;
                float nx = -dy / len * 5, ny = dx / len * 5; 

                using var tickPen = new Pen(color, 2f);
                for (int unit = 1; unit < length - 0.3; unit++)
                {
                    if (toScreen(direction * unit) is PointF t)
                        g.DrawLine(tickPen, t.X - nx, t.Y - ny, t.X + nx, t.Y + ny);
                }
            }
        }

        using var originBrush = new SolidBrush(Color.FromArgb(90, 90, 90));
        g.FillEllipse(originBrush, origin.X - 4, origin.Y - 4, 8, 8);
    }

    private static void DrawAxisLabels(Graphics g, Func<Point3D, PointF?> toScreen, double scale)
    {
        if (toScreen(new Point3D(0, 0, 0)) is not PointF origin)
            return;

        using var font = new Font("Segoe UI", 13f, FontStyle.Bold);
        foreach (var (direction, color, label) in Axes)
        {
            double length = AxisWorldLength(direction, origin, toScreen, scale);
            if (toScreen(direction * length) is PointF tip)
                DrawLabelBeyond(g, label, origin, tip, 16, font, color);
        }
    }

    
    private static double AxisWorldLength(Point3D direction, PointF origin, Func<Point3D, PointF?> toScreen, double scale)
    {
        if (toScreen(direction) is not PointF unitEnd)
            return AxisLength;

        float dx = unitEnd.X - origin.X, dy = unitEnd.Y - origin.Y;
        double ratio = Math.Sqrt(dx * dx + dy * dy) / scale; 
        if (ratio < 0.05)
            return AxisLength;                               
        return Math.Clamp(AxisLength / ratio, AxisLength * 0.5, AxisLength * 3);
    }

    
    private static void DrawLabelBeyond(Graphics g, string text, PointF from, PointF to, float offset, Font font, Color color)
    {
        float dx = to.X - from.X, dy = to.Y - from.Y;
        float length = MathF.Sqrt(dx * dx + dy * dy);
        if (length < 1)
            return; 

        var position = new PointF(to.X + dx / length * offset, to.Y + dy / length * offset);

        using var path = new GraphicsPath();
        using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        float emSize = g.DpiY * font.SizeInPoints / 72f;
        path.AddString(text, font.FontFamily, (int)font.Style, emSize, position, format);

        using var halo = new Pen(Color.White, 4f) { LineJoin = LineJoin.Round };
        using var fill = new SolidBrush(color);
        g.DrawPath(halo, path);
        g.FillPath(fill, path);
    }
}
