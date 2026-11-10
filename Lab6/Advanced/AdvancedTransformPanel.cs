using System.Drawing;
using Lab6.Geometry;
using Lab6.Rendering;
using Lab6.Transformations;

namespace Lab6.Advanced;

public sealed class AdvancedTransformPanel : UserControl
{
    private readonly Scene scene;

    private readonly ComboBox centerAxisChoice = Choice("X", "Y", "Z");
    private readonly NumericUpDown centerAngle = Number(30);

    private readonly NumericUpDown lineP1X = Number(-1);
    private readonly NumericUpDown lineP1Y = Number(0);
    private readonly NumericUpDown lineP1Z = Number(0);
    private readonly NumericUpDown lineP2X = Number(1);
    private readonly NumericUpDown lineP2Y = Number(0);
    private readonly NumericUpDown lineP2Z = Number(0);
    private readonly NumericUpDown lineAngle = Number(45);

    private readonly ComboBox projectionChoice = Choice(
        "Кабинетная (косоугольная)",
        "Аксонометрическая (ортогональная)",
        "Перспективная");
    private readonly Label projectionParam1Label = new Label { AutoSize = true };
    private readonly Label projectionParam2Label = new Label { AutoSize = true };
    private readonly NumericUpDown projectionParam1 = Number(30);
    private readonly NumericUpDown projectionParam2 = Number(45);
    private readonly TextBox matrixPreview = new TextBox();

    public AdvancedTransformPanel(Scene scene)
    {
        this.scene = scene ?? throw new ArgumentNullException(nameof(scene));
        Name = nameof(AdvancedTransformPanel);
        Size = new Size(270, 650);
        Margin = new Padding(3, 0, 3, 8);

        GroupBox centerRotation = Group("5. Поворот вокруг оси через центр", 0, 105);
        AddLabel(centerRotation, "Ось", 12, 22);
        AddLabel(centerRotation, "Угол, °", 130, 22);
        centerAxisChoice.SetBounds(12, 41, 104, 25);
        centerAngle.SetBounds(130, 41, 116, 25);
        centerRotation.Controls.Add(centerAxisChoice);
        centerRotation.Controls.Add(centerAngle);
        AddButton(centerRotation, "Применить поворот", 12, 72, 238,
            () => AdvancedTransformations.RotationAroundCenterParallelToAxis(
                this.scene.Model,
                (CoordinateAxis)centerAxisChoice.SelectedIndex,
                (double)centerAngle.Value));

        GroupBox lineRotation = Group("6. Поворот вокруг произвольной прямой", 111, 228);
        AddLabel(lineRotation, "Точка 1", 12, 20);
        AddXYZ(lineRotation, 38, lineP1X, lineP1Y, lineP1Z);
        AddLabel(lineRotation, "Точка 2", 12, 92);
        AddXYZ(lineRotation, 110, lineP2X, lineP2Y, lineP2Z);
        AddLabel(lineRotation, "Угол, °", 12, 164);
        lineAngle.SetBounds(130, 161, 116, 25);
        lineRotation.Controls.Add(lineAngle);
        AddButton(lineRotation, "Повернуть вокруг прямой", 12, 195, 238,
            () => AdvancedTransformations.RotationAroundLine(
                new Point3D((double)lineP1X.Value, (double)lineP1Y.Value, (double)lineP1Z.Value),
                new Point3D((double)lineP2X.Value, (double)lineP2Y.Value, (double)lineP2Z.Value),
                (double)lineAngle.Value));

        GroupBox projectionGroup = Group("7. Проекция", 345, 157);
        projectionChoice.SetBounds(10, 22, 243, 25);
        projectionGroup.Controls.Add(projectionChoice);
        projectionParam1Label.SetBounds(12, 55, 112, 18);
        projectionParam2Label.SetBounds(12, 85, 112, 18);
        projectionParam1.SetBounds(130, 52, 116, 25);
        projectionParam2.SetBounds(130, 82, 116, 25);
        projectionGroup.Controls.Add(projectionParam1Label);
        projectionGroup.Controls.Add(projectionParam2Label);
        projectionGroup.Controls.Add(projectionParam1);
        projectionGroup.Controls.Add(projectionParam2);
        AddButton(projectionGroup, "Применить проекцию", 12, 116, 238, null);

        GroupBox matrixGroup = Group("Матрица последней операции (4×4)", 508, 135);
        matrixPreview.ReadOnly = true;
        matrixPreview.Multiline = true;
        matrixPreview.WordWrap = false;
        matrixPreview.ScrollBars = ScrollBars.Horizontal;
        matrixPreview.Font = new Font("Consolas", 8f);
        matrixPreview.SetBounds(10, 24, 240, 99);
        matrixPreview.Text = Matrix4.Identity().ToString();
        matrixGroup.Controls.Add(matrixPreview);

        projectionChoice.SelectedIndexChanged += ProjectionChoiceChanged;
        projectionChoice.SelectedIndex = ProjectionChoiceIndex(scene.Projection);
        UpdateProjectionInputs();
        ApplyProjection();

        scene.Changed += SceneChanged;
        Disposed += (_, _) => scene.Changed -= SceneChanged;
    }

    private void SceneChanged(object sender, EventArgs e)
    {
        if (projectionChoice.SelectedIndex == ProjectionChoiceIndex(scene.Projection))
            return;

        projectionChoice.SelectedIndexChanged -= ProjectionChoiceChanged;
        projectionChoice.SelectedIndex = ProjectionChoiceIndex(scene.Projection);
        projectionChoice.SelectedIndexChanged += ProjectionChoiceChanged;
        UpdateProjectionInputs();
    }

    private void ProjectionChoiceChanged(object sender, EventArgs e)
    {
        UpdateProjectionInputs();
        ApplyProjection();
    }

    private void UpdateProjectionInputs()
    {
        switch (projectionChoice.SelectedIndex)
        {
            case 0:
                projectionParam1Label.Text = "Параметры не нужны";
                projectionParam2Label.Text = string.Empty;
                projectionParam1.Enabled = false;
                projectionParam2.Enabled = false;
                break;

            case 1:
                projectionParam1Label.Text = "Поворот X, °";
                projectionParam2Label.Text = "Поворот Y, °";
                projectionParam1.Enabled = true;
                projectionParam2.Enabled = true;
                projectionParam1.Value = 30;
                projectionParam2.Value = 45;
                break;

            case 2:
                projectionParam1Label.Text = "Расстояние d";
                projectionParam2Label.Text = string.Empty;
                projectionParam1.Enabled = true;
                projectionParam2.Enabled = false;
                projectionParam1.Value = 6;
                break;
        }
    }

    private void ApplyProjection()
    {
        try
        {
            IProjection projection = projectionChoice.SelectedIndex switch
            {
                0 => new CabinetProjection(),
                1 => new AxonometricProjection(
                    (double)projectionParam1.Value,
                    (double)projectionParam2.Value),
                2 => new PerspectiveProjection((double)projectionParam1.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(projectionChoice)),
            };

            scene.Projection = projection;
            matrixPreview.Text = projection.GetMatrix().ToString();
        }
        catch (Exception ex) when (ex is ArgumentException or OverflowException)
        {
            MessageBox.Show(this, ex.Message, "Невозможно применить проекцию",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static int ProjectionChoiceIndex(IProjection projection) => projection switch
    {
        PerspectiveProjection => 2,
        AxonometricProjection => 1,
        _ => 0,
    };

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

    private static void AddXYZ(GroupBox group, int top, NumericUpDown x, NumericUpDown y, NumericUpDown z)
    {
        AddLabel(group, "X", 12, top + 2);
        AddLabel(group, "Y", 94, top + 2);
        AddLabel(group, "Z", 176, top + 2);
        x.SetBounds(10, top + 20, 77, 25);
        y.SetBounds(92, top + 20, 77, 25);
        z.SetBounds(174, top + 20, 77, 25);
        group.Controls.Add(x);
        group.Controls.Add(y);
        group.Controls.Add(z);
    }

    private void AddButton(GroupBox group, string caption, int x, int y, int width, Func<Matrix4> getMatrix)
    {
        var button = new Button { Text = caption, Bounds = new Rectangle(x, y, width, 27) };
        button.Click += (_, _) =>
        {
            if (getMatrix == null)
            {
                ApplyProjection();
                return;
            }

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
