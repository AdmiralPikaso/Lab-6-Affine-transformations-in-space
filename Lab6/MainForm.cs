using Lab6.Geometry;
using Lab6.Rendering;

namespace Lab6;

public partial class MainForm : Form
{
    private readonly Scene scene = new Scene();

    public MainForm()
    {
        InitializeComponent();

        foreach (PolyhedronKind kind in Enum.GetValues<PolyhedronKind>())
            polyhedronCombo.Items.Add(PolyhedronFactory.DisplayName(kind));

        scene.Changed += Scene_Changed;
        polyhedronCombo.SelectedIndex = (int)PolyhedronKind.Tetrahedron;

        controlsPanel.Controls.Add(new Lab6.Transformations.AffineTransformPanel(scene));
    }

    private void Scene_Changed(object sender, EventArgs e)
    {
        Polyhedron model = scene.Model;
        infoLabel.Text = $"Вершин: {model.Vertices.Count}, рёбер: {model.Edges().Count}, граней: {model.Faces.Count}";
        canvas.Invalidate();
    }

    private void polyhedronCombo_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (polyhedronCombo.SelectedIndex < 0)
            return;
        scene.SetModel(PolyhedronFactory.Create((PolyhedronKind)polyhedronCombo.SelectedIndex));
    }

    private void resetButton_Click(object sender, EventArgs e) => scene.Reset();

    private void axesCheckBox_CheckedChanged(object sender, EventArgs e) => scene.ShowAxes = axesCheckBox.Checked;

    private void hiddenEdgesCheckBox_CheckedChanged(object sender, EventArgs e) =>
        scene.DashHiddenEdges = hiddenEdgesCheckBox.Checked;

    private void canvas_Paint(object sender, PaintEventArgs e) =>
        SceneRenderer.Draw(e.Graphics, canvas.ClientRectangle, scene);

    private void canvas_Resize(object sender, EventArgs e) => canvas.Invalidate();
}
