﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using ViewpointSystems.Svn.SvnThings;

namespace ViewpointSystems.Svn.Core.Tests.SvnTests
{
    [TestClass]
    public class CheckoutTest : BaseTest
    {

        [TestMethod]
        public void SvnManagement_Checkout_IsValid()
        {
            // Arrange
            bool assertVal = false;
            SvnManagement.BuildUnitTestRepo(MainViewModel.LocalWorkingLocation, UnitTestFolder);

            // Act
            
            if (SvnManagement.CheckOut(UnitTestPath))
            {
                SvnManagement.LoadCurrentSvnItemsInLocalRepository(UnitTestPath);
                assertVal = true;
            }
            
            
            // Assert
            assertVal.Should().BeTrue();
        }
    }
}
