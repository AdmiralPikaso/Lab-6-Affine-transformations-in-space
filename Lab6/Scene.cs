using Lab6.Geometry;
using Lab6.Rendering;

namespace Lab6;


public class Scene
{
    private Polyhedron original;
    private IProjection projection = new CabinetProjection();
    private bool showAxes = true;
    private bool dashHiddenEdges = true;

  
    public Polyhedron Model { get; private set; }

    
    public IProjection Projection
    {
        get => projection;
        set
        {
            projection = value ?? throw new ArgumentNullException(nameof(value));
            OnChanged();
        }
    }

    
    public bool ShowAxes
    {
        get => showAxes;
        set
        {
            showAxes = value;
            OnChanged();
        }
    }

    
    public bool DashHiddenEdges
    {
        get => dashHiddenEdges;
        set
        {
            dashHiddenEdges = value;
            OnChanged();
        }
    }

    
    public event EventHandler Changed;

    public Scene()
    {
        SetModel(PolyhedronFactory.Create(PolyhedronKind.Tetrahedron));
    }

    
    public void SetModel(Polyhedron polyhedron)
    {
        original = polyhedron.Clone();
        Model = polyhedron;
        OnChanged();
    }


    public void Reset()
    {
        Model = original.Clone();
        OnChanged();
    }

    
    public void Apply(Matrix4 matrix)
    {
        Model.Apply(matrix);
        OnChanged();
    }

   
    public void Refresh() => OnChanged();

    protected virtual void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);
}
