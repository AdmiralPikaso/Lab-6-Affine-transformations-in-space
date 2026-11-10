using System.Drawing;
using Lab6.Geometry;

namespace Lab6.Transformations;

public sealed class AffineTransformPanel : UserControl
{
    private readonly Scene scene;
    private readonly NumericUpDown shiftX = Number(0);
    private readonly NumericUpDown shiftY = Number(0);
    private readonly NumericUpDown shiftZ = Number(0);
    private readonly ComboBox axisChoice = Choice("X", "Y", "Z");
    private readonly NumericUpDown angleChoice = Number(30);
    private readonly NumericUpDown scaleX = Number(1);
    private readonly NumericUpDown scaleY = Number(1);
    private readonly NumericUpDown scaleZ = Number(1);
    private readonly ComboBox planeChoice = Choice("XY", "XZ", "YZ");
    private readonly Label centerLabel = new Label();
    private readonly TextBox matrixPreview = new TextBox();

    public AffineTransformPanel(Scene scene)
    {
        this.scene = scene ?? throw new ArgumentNullException(nameof(scene));
        Name = nameof(AffineTransformPanel);
        Size = new Size(270, 624);
        Margin = new Padding(3, 3, 3, 8);

        GroupBox shift = Group("1. Смещение", 0, 105);
        AddXYZ(shift, shiftX, shiftY, shiftZ);
        AddButton(shift, "Применить смещение", 72, 238,
            () => AffineTransformations.Translation((double)shiftX.Value, (double)shiftY.Value, (double)shiftZ.Value));

        GroupBox rotation = Group("2. Поворот вокруг оси координат", 111, 105);
        AddLabel(rotation, "Ось", 12, 22);
        AddLabel(rotation, "Угол, °", 130, 22);
        axisChoice.SetBounds(12, 41, 104, 25);
        angleChoice.SetBounds(130, 41, 116, 25);
        rotation.Controls.Add(axisChoice);
        rotation.Controls.Add(angleChoice);
        AddButton(rotation, "Применить поворот", 72, 238,
            () => AffineTransformations.Rotation((CoordinateAxis)axisChoice.SelectedIndex, (double)angleChoice.Value));

        GroupBox scaling = Group("3. Масштабирование", 222, 160);
        AddXYZ(scaling, scaleX, scaleY, scaleZ);
        AddButton(scaling, "От начала координат", 71, 238,
            () => AffineTransformations.Scaling((double)scaleX.Value, (double)scaleY.Value, (double)scaleZ.Value));
        AddButton(scaling, "От центра многогранника", 103, 238,
            () => AffineTransformations.ScalingAroundCenter(this.scene.Model,
                (double)scaleX.Value, (double)scaleY.Value, (double)scaleZ.Value));
        centerLabel.AutoSize = false;
        centerLabel.SetBounds(12, 133, 238, 16);
        centerLabel.Font = new Font("Segoe UI", 8.2f);
        centerLabel.ForeColor = SystemColors.GrayText;
        scaling.Controls.Add(centerLabel);

        GroupBox reflection = Group("4. Отражение от плоскости", 388, 91);
        AddLabel(reflection, "Плоскость", 12, 22);
        planeChoice.SetBounds(130, 19, 116, 25);
        reflection.Controls.Add(planeChoice);
        AddButton(reflection, "Отразить", 52, 238,
            () => AffineTransformations.Reflection((CoordinatePlane)planeChoice.SelectedIndex));

        GroupBox matrixGroup = Group("Матрица последней операции (4×4)", 485, 135);
        matrixPreview.ReadOnly = true;
        matrixPreview.Multiline = true;
        matrixPreview.WordWrap = false;
        matrixPreview.ScrollBars = ScrollBars.Horizontal;
        matrixPreview.Font = new Font("Consolas", 8f);
        matrixPreview.SetBounds(10, 24, 240, 99);
        matrixPreview.Text = Matrix4.Identity().ToString();
        matrixGroup.Controls.Add(matrixPreview);

        scene.Changed += SceneChanged;
        Disposed += (_, _) => scene.Changed -= SceneChanged;
        UpdateCenter();
    }

    private void SceneChanged(object sender, EventArgs e) => UpdateCenter();

    private void UpdateCenter()
    {
        Point3D center = scene.Model.Center();
        centerLabel.Text = $"Центр: ({center.X:0.##}; {center.Y:0.##}; {center.Z:0.##})";
    }

    private GroupBox Group(string caption, int top, int height)
    {
        var group = new GroupBox { Text = caption, Bounds = new Rectangle(0, top, 263, height) };
        Controls.Add(group);
        return group;
    }

    private static NumericUpDown Number(decimal value) => new NumericUpDown
    {
        DecimalPlaces = 2,
        Minimum = -10000,
        Maximum = 10000,
        Increment = 0.25m,
        Value = value,
        TextAlign = HorizontalAlignment.Center,
    };

    private static ComboBox Choice(params string[] items)
    {
        var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        combo.Items.AddRange(items);
        combo.SelectedIndex = 0;
        return combo;
    }

    private static void AddLabel(Control parent, string text, int x, int y)
    {
        var label = new Label { Text = text, AutoSize = true, Location = new Point(x, y) };
        parent.Controls.Add(label);
    }

    private static void AddXYZ(GroupBox group, NumericUpDown x, NumericUpDown y, NumericUpDown z)
    {
        AddLabel(group, "X", 12, 22);
        AddLabel(group, "Y", 94, 22);
        AddLabel(group, "Z", 176, 22);
        x.SetBounds(10, 41, 77, 25);
        y.SetBounds(92, 41, 77, 25);
        z.SetBounds(174, 41, 77, 25);
        group.Controls.Add(x);
        group.Controls.Add(y);
        group.Controls.Add(z);
    }

    private void AddButton(GroupBox group, string caption, int y, int width, Func<Matrix4> getMatrix)
    {
        var button = new Button { Text = caption, Bounds = new Rectangle(12, y, width, 27) };
        button.Click += (_, _) =>
        {
            try
            {
                Matrix4 matrix = getMatrix();
                AffineTransformations.Apply(scene, matrix);
                matrixPreview.Text = matrix.ToString();
            }
            catch (Exception ex) when (ex is ArgumentException or OverflowException)
            {
                MessageBox.Show(this, ex.Message, "Невозможно применить преобразование",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
        group.Controls.Add(button);
    }
}
