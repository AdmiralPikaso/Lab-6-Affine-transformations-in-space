using Lab6.Geometry;

namespace Lab6.Rendering;


public interface IProjection
{
    
    string Name { get; }

    
    Matrix4 GetMatrix();
}
