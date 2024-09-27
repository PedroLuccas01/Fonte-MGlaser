using System;
using System.Windows.Forms;

public class RoundedPanel : Panel
{
    public int CornerRadius { get; set; }

    public RoundedPanel()
    {
        CornerRadius = 10; // Define o raio da borda, você pode ajustar de acordo com sua preferência
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        int w = this.Width;
        int h = this.Height;
        int d = CornerRadius * 2;

        path.AddArc(0, 0, d, d, 180, 90); // Canto superior esquerdo
        path.AddArc(w - d, 0, d, d, 270, 90); // Canto superior direito
        path.AddArc(w - d, h - d, d, d, 0, 90); // Canto inferior direito
        path.AddArc(0, h - d, d, d, 90, 90); // Canto inferior esquerdo
        path.CloseFigure();

        this.Region = new System.Drawing.Region(path);
    }
}
