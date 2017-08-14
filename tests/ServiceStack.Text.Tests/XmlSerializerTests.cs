using System;
using Northwind.Common.DataModel;
using NUnit.Framework;
using ServiceStack.Text.Tests.Support;

namespace ServiceStack.Text.Tests
{
    [TestFixture]
#if NETCORE_SUPPORT
    [Ignore("Fix Northwind.dll")]
#endif
    public class XmlSerializerTests
    {
        [OneTimeSetUp]
        public void TextFixtureSetup()
        {
            NorthwindData.LoadData(false);
        }

        [Test]
        public void Can_Serialize_Movie()
        {
            var xmlString = XmlSerializer.Serialize(MoviesData.Movies[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void Can_Serialize_Movies()
        {
            var xmlString = XmlSerializer.Serialize(MoviesData.Movies);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void Can_Serialize_MovieResponse_Dto()
        {
            var xmlString = XmlSerializer.Serialize(new MovieResponse {Movie = MoviesData.Movies[0]});
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Category()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Categories[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Categories()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Categories);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Customer()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Customers[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Customers()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Customers);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Employee()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Employees[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Employees()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Employees);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_EmployeeTerritory()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.EmployeeTerritories[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_EmployeeTerritories()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.EmployeeTerritories);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_OrderDetail()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.OrderDetails[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_OrderDetails()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.OrderDetails);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Order()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Orders[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Orders()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Orders);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Product()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Products[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Products()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Products);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Region()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Regions[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Regions()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Regions);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Shipper()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Shippers[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Shippers()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Shippers);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Supplier()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Suppliers[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Suppliers()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Suppliers);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Territory()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Territories[0]);
            Console.WriteLine(xmlString);
        }

        [Test]
        public void serialize_Territories()
        {
            var xmlString = XmlSerializer.Serialize(NorthwindData.Territories);
            Console.WriteLine(xmlString);
        }
    }
}