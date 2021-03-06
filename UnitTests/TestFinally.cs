﻿using System;
using NUnit.Framework;

namespace UnitTests
{

    public class TestFinally
    {
        [TestCase()]
        public void TestThrowingEx()
        {
            try
            {
                InternalMeth();
            }
            catch (Exception)
            {
                Console.WriteLine("in outer catch");
            }
            
        }

        public void InternalMeth()
        {
            try
            {
                throw new Exception();
            }
            catch (Exception)
            {
                Console.WriteLine("in Catch");
                throw;
            }
            finally
            {
                Console.WriteLine("in finally");
            }

        }

    }
}
