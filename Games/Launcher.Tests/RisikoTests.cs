using System;
using System.Drawing;
using System.Windows;
using Games;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Risiko; 

namespace Launcher.Tests
{
    [TestClass]
    public class RisikoTests
    {
        [TestMethod]
        public void PointInPolygon_PositivePoint_True()
        {
            //Games.LauncherControl b;
            //var a = new Risiko.Player("Peter", false, Color.Black);

            Point[] Poly = new Point[4];
            Poly[0].X = 0;
            Poly[0].Y = 0;
            Poly[1].X = 2;
            Poly[1].Y = 0;
            Poly[2].X = 2;
            Poly[2].Y = 2;
            Poly[3].X = 0;
            Poly[3].Y = 2;

            Point P = new Point(1,1);

            GameControl c = new GameControl();
            bool temp = false;
            temp = c.PointInPolygon(P, Poly);

            Assert.AreEqual(temp, true);

            //Risiko.GameControl target;
            //PrivateObject obj = new PrivateObject(target);
            //var retVal = obj.Invoke("PrivateMethod");
            //Assert.AreEqual(retVal);
        }

        [TestMethod]
        public void PointInPolygon_PointOutOfPolygon_False()
        {
            //Games.LauncherControl b;
            //var a = new Risiko.Player("Peter", false, Color.Black);

            Point[] Poly = new Point[4];
            Poly[0].X = 0;
            Poly[0].Y = 0;
            Poly[1].X = 2;
            Poly[1].Y = 0;
            Poly[2].X = 2;
            Poly[2].Y = 2;
            Poly[3].X = 0;
            Poly[3].Y = 2;

            Point P = new Point(3, 1);

            GameControl c = new GameControl();
            bool temp = false;
            temp = c.PointInPolygon(P, Poly);

            Assert.AreEqual(temp, false);

            //Risiko.GameControl target;
            //PrivateObject obj = new PrivateObject(target);
            //var retVal = obj.Invoke("PrivateMethod");
            //Assert.AreEqual(retVal);
        }

        [TestMethod]
        public void DrawMap_CorrectDrawing()
        {
            
        }
    }
}
