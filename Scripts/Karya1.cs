namespace Godot;

using Godot;
using System;
using System.Collections.Generic;

public partial class Karya1 : Node2D
{
	private BentukDasar _bentukDasar = new BentukDasar();
	private Transformasi _transformasi = new Transformasi();

	public override void _Ready()
    {
		ScreenUtils.Initialize(GetViewport()); // Initialize ScreenUtils
		QueueRedraw();
    }

	public override void _Draw()
	{
		MarginPixel();
		MyPersegi();
		MyLingkaranDanElips();
		MyTransformation();
	}

	private void MyTransformation()
	{
		Vector2 myPoint = new Vector2(50, 50);
		float[,] myMatrix = new float[3, 3];
		Transformasi.Matrix3x3Identity(myMatrix); // Corrected line
		_transformasi.Translation(myMatrix, 10, 20, ref myPoint);
		List<Vector2> points = new List<Vector2>() { myPoint };
		PrintUtils.PrintVector2List(points, "Koordinat Awal");
		var transformedPoints = _transformasi.GetTransformPoint(myMatrix, points);
		PrintUtils.PrintVector2List(transformedPoints, "Koordinat Transformasi");
	}

	private void MyPersegi(){
		var persegi1 = _bentukDasar.Persegi(100, 100, 50); // Gambar persegi di posisi (100, 100) dengan ukuran 50
		GraphicsUtils.PutPixelAll(this, persegi1, GraphicsUtils.DrawStyle.DotStripDot, ColorUtils.ColorStorage(4), 3, 2);

    	var persegipanjang1 = _bentukDasar.PersegiPanjang(200, 150, 80, 40); // Gambar persegi panjang
		GraphicsUtils.PutPixelAll(this, persegipanjang1, GraphicsUtils.DrawStyle.StripStrip, ColorUtils.ColorStorage(3), 3, 2);
	}

	private void MyLingkaranDanElips()
    {
        // Draw a circle
        var lingkaranPoints = _bentukDasar.Lingkaran(new Vector2(400, 100), 40);
        GraphicsUtils.PutPixelAll(this, lingkaranPoints, GraphicsUtils.DrawStyle.CircleDot, ColorUtils.ColorStorage(2), gap: 2);

        // Draw an ellipse
        var elipsPoints = _bentukDasar.Elips(new Vector2(550, 250), 60, 30);
        GraphicsUtils.PutPixelAll(this, elipsPoints, GraphicsUtils.DrawStyle.EllipseStrip, ColorUtils.ColorStorage(1), gap: 2);
    }

	private void MarginPixel(){
		var margin = _bentukDasar.Margin();
		GraphicsUtils.PutPixelAll(this, margin, GraphicsUtils.DrawStyle.DotDot, ColorUtils.ColorStorage(6));
	}

	public override void _ExitTree()
    {
		NodeUtils.DisposeAndNull(_bentukDasar, "_bentukDasar");
        NodeUtils.DisposeAndNull(_transformasi, "_transformasi");
        base._ExitTree();
    }
	
}	
