using Microsoft.VisualStudio.TestTools.UnitTesting;
using VolleyballTools.PDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolleyballTools.PDF.Tests
{
    [TestClass()]
    public class PDFTests
    {
        [TestMethod()]
        public void Generate5SETTest()
        {
            var pdf = new PDF();

            pdf.Generate5SET("試合名","開催地名","会場名",DateTime.Now,"A-12","ATeam","BTeam",true,DateTime.Now);
            
            
            Assert.Fail();
        }
    }
}