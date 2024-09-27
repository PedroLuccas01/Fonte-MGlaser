using System;
using System.Windows.Forms;

public class CircularPanel : Panel
{
    public CircularPanel()
    {
        this.Width = 100; // Defina o tamanho desejado para o círculo
        this.Height = 100; // Defina o tamanho desejado para o círculo
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        int size = Math.Min(this.Width, this.Height);

        path.AddEllipse(0, 0, size, size);

        this.Region = new System.Drawing.Region(path);
    }
}
