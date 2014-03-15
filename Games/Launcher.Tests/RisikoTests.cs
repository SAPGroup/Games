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
            Games.LauncherControl b;
            var a = new Risiko.Player("Peter", false, Color.Black);
            
            //Risiko.GameControl target;
            //PrivateObject obj = new PrivateObject(target);
            //var retVal = obj.Invoke("PrivateMethod");
            //Assert.AreEqual(retVal);
        }
    }
}
